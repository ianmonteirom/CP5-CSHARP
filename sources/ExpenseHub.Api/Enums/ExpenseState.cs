namespace ExpenseHub.Api.Enums;

/// <summary>Possible states for an expense reimbursement request.</summary>
public enum ExpenseState
{
    /// <summary>Editable by the owner before submission.</summary>
    Draft = 0,

    /// <summary>Awaiting approver review.</summary>
    Submitted = 1,

    /// <summary>Approved and awaiting payment.</summary>
    Approved = 2,

    /// <summary>Rejected — final state.</summary>
    Rejected = 3,

    /// <summary>Paid — final state.</summary>
    Paid = 4
}
