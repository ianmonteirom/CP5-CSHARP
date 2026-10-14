namespace ExpenseHub.Api.Enums;

/// <summary>Actions recorded in the expense audit history.</summary>
public enum HistoryAction
{
    /// <summary>Expense draft was created.</summary>
    Created,

    /// <summary>Expense draft was edited.</summary>
    Edited,

    /// <summary>Expense was submitted for review.</summary>
    Submitted,

    /// <summary>Expense was approved.</summary>
    Approved,

    /// <summary>Expense was rejected.</summary>
    Rejected,

    /// <summary>Expense was paid.</summary>
    Paid
}
