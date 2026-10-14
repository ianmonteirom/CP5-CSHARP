using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpenseHub.Api.DTOs;
using ExpenseHub.Api.Responses;

namespace ExpenseHub.Api.Services;

/// <summary>Manages expense lifecycle operations with role-based access control.</summary>
public interface IExpenseService
{
    /// <summary>Creates a new expense draft owned by the authenticated employee.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> CreateAsync(
        CreateExpenseRequest request,
        string ownerId,
        CancellationToken cancellationToken = default);

    /// <summary>Updates a draft expense; only the owner may edit and only while in Draft state.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> UpdateAsync(
        Guid id,
        UpdateExpenseRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Transitions a Draft expense to Submitted; only the owner may submit.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> SubmitAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Approves a Submitted expense; only an Approver who is not the owner may approve.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> ApproveAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Rejects a Submitted expense with a mandatory justification.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> RejectAsync(
        Guid id,
        RejectExpenseRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Records payment for an Approved expense; only a Finance user who is not the owner may pay.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> PayAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns expenses visible to the given user based on their roles.</summary>
    Task<IReadOnlyList<ExpenseSummaryResponse>> GetListAsync(
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a single expense by identifier if visible to the requesting user.</summary>
    Task<ServiceResult<ExpenseDetailResponse>> GetByIdAsync(
        Guid id,
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the audit history of an expense if visible to the requesting user.</summary>
    Task<ServiceResult<IReadOnlyList<ExpenseHistoryItemResponse>>> GetHistoryAsync(
        Guid id,
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);
}
