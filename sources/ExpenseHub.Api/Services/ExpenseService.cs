using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpenseHub.Api.Data;
using ExpenseHub.Api.DTOs;
using ExpenseHub.Api.Entities;
using ExpenseHub.Api.Enums;
using ExpenseHub.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseHub.Api.Services;

/// <summary>Implements expense lifecycle operations backed by EF Core.</summary>
public sealed class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance of the <see cref="ExpenseService"/> class.</summary>
    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> CreateAsync(
        CreateExpenseRequest request,
        string ownerId,
        CancellationToken cancellationToken = default)
    {
        if (request.ExpenseDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return ServiceResult<ExpenseDetailResponse>.BadRequest("Expense date must not be in the future.");
        }

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Description = request.Description,
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate,
            CategoryId = request.CategoryId,
            State = ExpenseState.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        var history = new ExpenseHistory
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = ownerId,
            Action = HistoryAction.Created,
            PreviousState = ExpenseState.Draft,
            NextState = ExpenseState.Draft,
            OccurredAtUtc = expense.CreatedAtUtc
        };

        _context.Expenses.Add(expense);
        _context.ExpenseHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        Expense loaded = (await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == expense.Id, cancellationToken))!;

        return ServiceResult<ExpenseDetailResponse>.Created(MapDetail(loaded));
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> UpdateAsync(
        Guid id,
        UpdateExpenseRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null)
        {
            return ServiceResult<ExpenseDetailResponse>.NotFound("Expense not found.");
        }

        if (expense.OwnerId != userId)
        {
            return ServiceResult<ExpenseDetailResponse>.Forbidden("Only the owner may edit this expense.");
        }

        if (expense.State != ExpenseState.Draft)
        {
            return ServiceResult<ExpenseDetailResponse>.Conflict("Only Draft expenses can be edited.");
        }

        if (request.ExpenseDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return ServiceResult<ExpenseDetailResponse>.BadRequest("Expense date must not be in the future.");
        }

        var changesParts = new List<string>();

        if (expense.Description != request.Description)
        {
            changesParts.Add(string.Format(CultureInfo.InvariantCulture,
                "Description: '{0}' → '{1}'", expense.Description, request.Description));
        }

        if (expense.Amount != request.Amount)
        {
            changesParts.Add(string.Format(CultureInfo.InvariantCulture,
                "Amount: {0} → {1}", expense.Amount, request.Amount));
        }

        if (expense.ExpenseDate != request.ExpenseDate)
        {
            changesParts.Add(string.Format(CultureInfo.InvariantCulture,
                "ExpenseDate: {0} → {1}", expense.ExpenseDate, request.ExpenseDate));
        }

        if (expense.CategoryId != request.CategoryId)
        {
            changesParts.Add(string.Format(CultureInfo.InvariantCulture,
                "CategoryId: {0} → {1}", expense.CategoryId?.ToString(CultureInfo.InvariantCulture) ?? "null",
                request.CategoryId?.ToString(CultureInfo.InvariantCulture) ?? "null"));
        }

        expense.Description = request.Description;
        expense.Amount = request.Amount;
        expense.ExpenseDate = request.ExpenseDate;
        expense.CategoryId = request.CategoryId;
        expense.UpdatedAtUtc = DateTime.UtcNow;

        string? changesDescription = changesParts.Count > 0
            ? string.Join("; ", changesParts)
            : null;

        var history = new ExpenseHistory
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = userId,
            Action = HistoryAction.Edited,
            PreviousState = ExpenseState.Draft,
            NextState = ExpenseState.Draft,
            Changes = changesDescription,
            OccurredAtUtc = expense.UpdatedAtUtc.Value
        };

        _context.ExpenseHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<ExpenseDetailResponse>.Ok(MapDetail(expense));
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> SubmitAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null)
        {
            return ServiceResult<ExpenseDetailResponse>.NotFound("Expense not found.");
        }

        if (expense.OwnerId != userId)
        {
            return ServiceResult<ExpenseDetailResponse>.Forbidden("Only the owner may submit this expense.");
        }

        if (!ExpenseWorkflow.CanSubmit(expense.State, expense.OwnerId, userId))
        {
            return ServiceResult<ExpenseDetailResponse>.Conflict("Expense is not in Draft state.");
        }

        ExpenseState previous = expense.State;
        expense.State = ExpenseState.Submitted;
        expense.UpdatedAtUtc = DateTime.UtcNow;

        var history = new ExpenseHistory
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = userId,
            Action = HistoryAction.Submitted,
            PreviousState = previous,
            NextState = ExpenseState.Submitted,
            OccurredAtUtc = expense.UpdatedAtUtc.Value
        };

        _context.ExpenseHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<ExpenseDetailResponse>.Ok(MapDetail(expense));
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> ApproveAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null)
        {
            return ServiceResult<ExpenseDetailResponse>.NotFound("Expense not found.");
        }

        if (expense.OwnerId == userId)
        {
            return ServiceResult<ExpenseDetailResponse>.Forbidden("Cannot approve your own expense.");
        }

        if (!ExpenseWorkflow.CanApprove(expense.State, expense.OwnerId, userId))
        {
            return ServiceResult<ExpenseDetailResponse>.Conflict("Expense is not in Submitted state.");
        }

        ExpenseState previous = expense.State;
        expense.State = ExpenseState.Approved;
        expense.UpdatedAtUtc = DateTime.UtcNow;

        var history = new ExpenseHistory
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = userId,
            Action = HistoryAction.Approved,
            PreviousState = previous,
            NextState = ExpenseState.Approved,
            OccurredAtUtc = expense.UpdatedAtUtc.Value
        };

        _context.ExpenseHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<ExpenseDetailResponse>.Ok(MapDetail(expense));
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> RejectAsync(
        Guid id,
        RejectExpenseRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null)
        {
            return ServiceResult<ExpenseDetailResponse>.NotFound("Expense not found.");
        }

        if (expense.OwnerId == userId)
        {
            return ServiceResult<ExpenseDetailResponse>.Forbidden("Cannot reject your own expense.");
        }

        if (!ExpenseWorkflow.CanReject(expense.State, expense.OwnerId, userId))
        {
            return ServiceResult<ExpenseDetailResponse>.Conflict("Expense is not in Submitted state.");
        }

        ExpenseState previous = expense.State;
        expense.State = ExpenseState.Rejected;
        expense.UpdatedAtUtc = DateTime.UtcNow;

        var history = new ExpenseHistory
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = userId,
            Action = HistoryAction.Rejected,
            PreviousState = previous,
            NextState = ExpenseState.Rejected,
            Justification = request.Justification,
            OccurredAtUtc = expense.UpdatedAtUtc.Value
        };

        _context.ExpenseHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<ExpenseDetailResponse>.Ok(MapDetail(expense));
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> PayAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null)
        {
            return ServiceResult<ExpenseDetailResponse>.NotFound("Expense not found.");
        }

        if (expense.OwnerId == userId)
        {
            return ServiceResult<ExpenseDetailResponse>.Forbidden("Cannot pay your own expense.");
        }

        if (!ExpenseWorkflow.CanPay(expense.State, expense.OwnerId, userId))
        {
            return ServiceResult<ExpenseDetailResponse>.Conflict("Expense is not in Approved state.");
        }

        DateTime paidAt = DateTime.UtcNow;

        expense.State = ExpenseState.Paid;
        expense.UpdatedAtUtc = paidAt;

        var payment = new PaymentRecord
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = userId,
            PaidAtUtc = paidAt
        };

        var history = new ExpenseHistory
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            ActorId = userId,
            Action = HistoryAction.Paid,
            PreviousState = ExpenseState.Approved,
            NextState = ExpenseState.Paid,
            OccurredAtUtc = paidAt
        };

        _context.PaymentRecords.Add(payment);
        _context.ExpenseHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<ExpenseDetailResponse>.Ok(MapDetail(expense));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ExpenseSummaryResponse>> GetListAsync(
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        bool isEmployee = roles.Contains("Employee");
        bool isApprover = roles.Contains("Approver");
        bool isFinance = roles.Contains("Finance");
        bool isAuditor = roles.Contains("Auditor");

        IQueryable<Expense> query = _context.Expenses;

        if (!isAuditor)
        {
            query = query.Where(e =>
                (isEmployee && e.OwnerId == userId) ||
                (isApprover && e.State == ExpenseState.Submitted) ||
                (isFinance && (e.State == ExpenseState.Approved || e.State == ExpenseState.Paid)));
        }

        List<Expense> expenses = await query
            .OrderByDescending(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return expenses.Select(MapSummary).ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<ExpenseDetailResponse>> GetByIdAsync(
        Guid id,
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Owner)
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null || !IsVisible(expense, userId, roles))
        {
            return ServiceResult<ExpenseDetailResponse>.NotFound("Expense not found or outside your scope.");
        }

        return ServiceResult<ExpenseDetailResponse>.Ok(MapDetail(expense));
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IReadOnlyList<ExpenseHistoryItemResponse>>> GetHistoryAsync(
        Guid id,
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        Expense? expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (expense is null || !IsVisible(expense, userId, roles))
        {
            return ServiceResult<IReadOnlyList<ExpenseHistoryItemResponse>>
                .NotFound("Expense not found or outside your scope.");
        }

        List<ExpenseHistory> entries = await _context.ExpenseHistories
            .Include(h => h.Actor)
            .Where(h => h.ExpenseId == id)
            .OrderBy(h => h.OccurredAtUtc)
            .ToListAsync(cancellationToken);

        IReadOnlyList<ExpenseHistoryItemResponse> items = entries.Select(MapHistoryItem).ToList().AsReadOnly();
        return ServiceResult<IReadOnlyList<ExpenseHistoryItemResponse>>.Ok(items);
    }

    private static bool IsVisible(Expense expense, string userId, IReadOnlyCollection<string> roles)
    {
        bool isEmployee = roles.Contains("Employee");
        bool isApprover = roles.Contains("Approver");
        bool isFinance = roles.Contains("Finance");
        bool isAuditor = roles.Contains("Auditor");

        if (isAuditor)
        {
            return true;
        }

        return (isEmployee && ExpenseWorkflow.IsVisibleToEmployee(expense.OwnerId, userId)) ||
               (isApprover && ExpenseWorkflow.IsVisibleToApprover(expense.State)) ||
               (isFinance && ExpenseWorkflow.IsVisibleToFinance(expense.State));
    }

    private static ExpenseSummaryResponse MapSummary(Expense expense) =>
        new()
        {
            Id = expense.Id,
            OwnerId = expense.OwnerId,
            Description = expense.Description,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            State = expense.State,
            CreatedAtUtc = expense.CreatedAtUtc
        };

    private static ExpenseDetailResponse MapDetail(Expense expense) =>
        new()
        {
            Id = expense.Id,
            OwnerId = expense.OwnerId,
            OwnerEmail = expense.Owner?.Email ?? string.Empty,
            Description = expense.Description,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            CategoryId = expense.CategoryId,
            CategoryName = expense.Category?.Name,
            State = expense.State,
            CreatedAtUtc = expense.CreatedAtUtc,
            UpdatedAtUtc = expense.UpdatedAtUtc
        };

    private static ExpenseHistoryItemResponse MapHistoryItem(ExpenseHistory h) =>
        new()
        {
            Id = h.Id,
            ActorId = h.ActorId,
            ActorEmail = h.Actor?.Email ?? string.Empty,
            Action = h.Action,
            PreviousState = h.PreviousState,
            NextState = h.NextState,
            Justification = h.Justification,
            Changes = h.Changes,
            OccurredAtUtc = h.OccurredAtUtc
        };
}
