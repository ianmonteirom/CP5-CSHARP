using System;
using ExpenseHub.Api.Enums;

namespace ExpenseHub.Api.Responses;

/// <summary>Compact view of an expense reimbursement request for list endpoints.</summary>
public class ExpenseSummaryResponse
{
    /// <summary>Gets or sets the expense identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the owner user identifier.</summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>Gets or sets the expense description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the reimbursement amount in BRL.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets the date the expense was incurred.</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>Gets or sets the current workflow state.</summary>
    public ExpenseState State { get; set; }

    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedAtUtc { get; set; }
}
