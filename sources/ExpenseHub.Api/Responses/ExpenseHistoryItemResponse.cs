using System;
using ExpenseHub.Api.Enums;

namespace ExpenseHub.Api.Responses;

/// <summary>Single entry in the audit history of an expense.</summary>
public class ExpenseHistoryItemResponse
{
    /// <summary>Gets or sets the history entry identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the identifier of the user who performed the action.</summary>
    public string ActorId { get; set; } = string.Empty;

    /// <summary>Gets or sets the email of the user who performed the action.</summary>
    public string ActorEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the action that was performed.</summary>
    public HistoryAction Action { get; set; }

    /// <summary>Gets or sets the workflow state before the action.</summary>
    public ExpenseState PreviousState { get; set; }

    /// <summary>Gets or sets the workflow state after the action.</summary>
    public ExpenseState NextState { get; set; }

    /// <summary>Gets or sets the rejection justification when applicable.</summary>
    public string? Justification { get; set; }

    /// <summary>Gets or sets a summary of field changes during a Draft edit.</summary>
    public string? Changes { get; set; }

    /// <summary>Gets or sets the UTC timestamp when the action occurred.</summary>
    public DateTime OccurredAtUtc { get; set; }
}
