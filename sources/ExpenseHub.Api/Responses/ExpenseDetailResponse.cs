using System;
using ExpenseHub.Api.Enums;

namespace ExpenseHub.Api.Responses;

/// <summary>Full detail view of an expense reimbursement request.</summary>
public class ExpenseDetailResponse
{
    /// <summary>Gets or sets the expense identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the owner user identifier.</summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>Gets or sets the owner email address.</summary>
    public string OwnerEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the expense description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the reimbursement amount in BRL.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets the date the expense was incurred.</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>Gets or sets the optional category identifier.</summary>
    public int? CategoryId { get; set; }

    /// <summary>Gets or sets the optional category name.</summary>
    public string? CategoryName { get; set; }

    /// <summary>Gets or sets the current workflow state.</summary>
    public ExpenseState State { get; set; }

    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Gets or sets the UTC last-update timestamp.</summary>
    public DateTime? UpdatedAtUtc { get; set; }
}
