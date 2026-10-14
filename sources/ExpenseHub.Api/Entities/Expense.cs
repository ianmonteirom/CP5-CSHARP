using System;
using System.Collections.Generic;
using ExpenseHub.Api.Enums;

namespace ExpenseHub.Api.Entities;

/// <summary>Represents a single reimbursement request.</summary>
public class Expense
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the identifier of the owning user.</summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>Gets or sets the owner navigation property.</summary>
    public ApplicationUser Owner { get; set; } = null!;

    /// <summary>Gets or sets the human-readable description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the reimbursement amount in BRL.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets the date the expense was incurred.</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>Gets or sets the optional category identifier.</summary>
    public int? CategoryId { get; set; }

    /// <summary>Gets or sets the optional category navigation property.</summary>
    public ExpenseCategory? Category { get; set; }

    /// <summary>Gets or sets the current workflow state.</summary>
    public ExpenseState State { get; set; }

    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the last update.</summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>Gets or sets the history entries for this expense.</summary>
    public ICollection<ExpenseHistory> History { get; set; } = [];

    /// <summary>Gets or sets the payment record when the expense has been paid.</summary>
    public PaymentRecord? PaymentRecord { get; set; }
}
