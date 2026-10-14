using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseHub.Api.DTOs;

/// <summary>Payload for creating a new expense draft.</summary>
public class CreateExpenseRequest
{
    /// <summary>Gets or sets the expense description.</summary>
    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the reimbursement amount in BRL.</summary>
    [Required]
    [Range(0.01, 2147483647.00)]
    public decimal Amount { get; set; }

    /// <summary>Gets or sets the date the expense was incurred (must not be future).</summary>
    [Required]
    public DateOnly ExpenseDate { get; set; }

    /// <summary>Gets or sets the optional category identifier.</summary>
    public int? CategoryId { get; set; }
}
