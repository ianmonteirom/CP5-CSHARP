using ExpenseHub.Api.Enums;

namespace ExpenseHub.Api.Services;

/// <summary>
/// Pure, stateless business rules for expense state transitions and visibility.
/// All methods are side-effect free and suitable for unit testing without infrastructure.
/// </summary>
public static class ExpenseWorkflow
{
    /// <summary>Returns true when a Draft expense can be edited by the given user.</summary>
    public static bool CanEdit(ExpenseState state, string ownerId, string userId)
        => state == ExpenseState.Draft && ownerId == userId;

    /// <summary>Returns true when a Draft expense can be submitted by the given user.</summary>
    public static bool CanSubmit(ExpenseState state, string ownerId, string userId)
        => state == ExpenseState.Draft && ownerId == userId;

    /// <summary>Returns true when an Approver (other than the owner) may approve the expense.</summary>
    public static bool CanApprove(ExpenseState state, string ownerId, string actorId)
        => state == ExpenseState.Submitted && ownerId != actorId;

    /// <summary>Returns true when an Approver (other than the owner) may reject the expense.</summary>
    public static bool CanReject(ExpenseState state, string ownerId, string actorId)
        => state == ExpenseState.Submitted && ownerId != actorId;

    /// <summary>Returns true when a Finance user (other than the owner) may record payment.</summary>
    public static bool CanPay(ExpenseState state, string ownerId, string actorId)
        => state == ExpenseState.Approved && ownerId != actorId;

    /// <summary>Returns true when the state is a terminal state (no further transitions allowed).</summary>
    public static bool IsFinalState(ExpenseState state)
        => state is ExpenseState.Rejected or ExpenseState.Paid;

    /// <summary>Returns true when an Employee can see the given expense.</summary>
    public static bool IsVisibleToEmployee(string ownerId, string userId)
        => ownerId == userId;

    /// <summary>Returns true when an Approver can see the given state.</summary>
    public static bool IsVisibleToApprover(ExpenseState state)
        => state == ExpenseState.Submitted;

    /// <summary>Returns true when a Finance user can see the given state.</summary>
    public static bool IsVisibleToFinance(ExpenseState state)
        => state is ExpenseState.Approved or ExpenseState.Paid;
}
