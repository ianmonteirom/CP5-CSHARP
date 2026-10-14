using System;

namespace ExpenseHub.Api.Entities;

/// <summary>Records a payment for an approved expense.</summary>
public class PaymentRecord
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the identifier of the paid expense.</summary>
    public Guid ExpenseId { get; set; }

    /// <summary>Gets or sets the expense navigation property.</summary>
    public Expense Expense { get; set; } = null!;

    /// <summary>Gets or sets the identifier of the Finance user who recorded the payment.</summary>
    public string ActorId { get; set; } = string.Empty;

    /// <summary>Gets or sets the Finance actor navigation property.</summary>
    public ApplicationUser Actor { get; set; } = null!;

    /// <summary>Gets or sets the UTC timestamp when the payment was recorded.</summary>
    public DateTime PaidAtUtc { get; set; }
}
