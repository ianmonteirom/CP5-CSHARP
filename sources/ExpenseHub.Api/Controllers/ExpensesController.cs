using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ExpenseHub.Api.DTOs;
using ExpenseHub.Api.Responses;
using ExpenseHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseHub.Api.Controllers;

/// <summary>Manages expense reimbursement requests and their lifecycle transitions.</summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    /// <summary>Initializes a new instance of the <see cref="ExpensesController"/> class.</summary>
    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>Creates a new expense draft owned by the authenticated employee.</summary>
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<ExpenseDetailResponse>> Create(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.CreateAsync(request, userId, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Updates a draft expense; only the owner may edit a Draft.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<ExpenseDetailResponse>> Update(
        Guid id,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.UpdateAsync(id, request, userId, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Returns expenses filtered by the caller's role(s).</summary>
    [HttpGet]
    [Authorize(Roles = "Employee,Approver,Finance,Auditor")]
    public async Task<ActionResult<IReadOnlyList<ExpenseSummaryResponse>>> GetList(CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        IReadOnlyCollection<string> roles = GetRoles();
        IReadOnlyList<ExpenseSummaryResponse> list = await _expenseService.GetListAsync(userId, roles, cancellationToken);
        return Ok(list);
    }

    /// <summary>Returns a single expense if visible to the caller.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Employee,Approver,Finance,Auditor")]
    public async Task<ActionResult<ExpenseDetailResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        IReadOnlyCollection<string> roles = GetRoles();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.GetByIdAsync(id, userId, roles, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Submits a Draft expense for review; only the owner may submit.</summary>
    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<ExpenseDetailResponse>> Submit(Guid id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.SubmitAsync(id, userId, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Approves a Submitted expense; the approver must not be the owner.</summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Approver")]
    public async Task<ActionResult<ExpenseDetailResponse>> Approve(Guid id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.ApproveAsync(id, userId, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Rejects a Submitted expense with a mandatory justification.</summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Approver")]
    public async Task<ActionResult<ExpenseDetailResponse>> Reject(
        Guid id,
        [FromBody] RejectExpenseRequest request,
        CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.RejectAsync(id, request, userId, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Records payment for an Approved expense; the Finance user must not be the owner.</summary>
    [HttpPost("{id:guid}/pay")]
    [Authorize(Roles = "Finance")]
    public async Task<ActionResult<ExpenseDetailResponse>> Pay(Guid id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        ServiceResult<ExpenseDetailResponse> result = await _expenseService.PayAsync(id, userId, cancellationToken);
        return MapResult(result);
    }

    /// <summary>Returns the audit history of an expense if visible to the caller.</summary>
    [HttpGet("{id:guid}/history")]
    [Authorize(Roles = "Employee,Approver,Finance,Auditor")]
    public async Task<ActionResult<IReadOnlyList<ExpenseHistoryItemResponse>>> GetHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        IReadOnlyCollection<string> roles = GetRoles();
        ServiceResult<IReadOnlyList<ExpenseHistoryItemResponse>> result =
            await _expenseService.GetHistoryAsync(id, userId, roles, cancellationToken);
        return MapResult(result);
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User identifier claim is missing.");

    private IReadOnlyCollection<string> GetRoles() =>
        User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

    private ActionResult<T> MapResult<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode == 201
                ? StatusCode(201, result.Value)
                : Ok(result.Value);
        }

        ProblemDetails problem = new() { Title = result.Error, Status = result.StatusCode };

        return result.StatusCode switch
        {
            400 => BadRequest(problem),
            403 => StatusCode(403, problem),
            404 => NotFound(problem),
            409 => Conflict(problem),
            _ => StatusCode(500, problem)
        };
    }
}
