using ExpenseHub.Api.Enums;
using ExpenseHub.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseHub.UnitTests;

/// <summary>Verifies the state machine transitions enforced by <see cref="ExpenseWorkflow"/>.</summary>
[TestClass]
public class ExpenseStateTransitionTests
{
    private const string OwnerId = "owner-1";
    private const string ActorId = "actor-2";

    // CanEdit

    [TestMethod]
    public void CanEdit_DraftAndOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanEdit(ExpenseState.Draft, OwnerId, OwnerId));

    [TestMethod]
    public void CanEdit_DraftButNotOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanEdit(ExpenseState.Draft, OwnerId, ActorId));

    [TestMethod]
    public void CanEdit_SubmittedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanEdit(ExpenseState.Submitted, OwnerId, OwnerId));

    [TestMethod]
    public void CanEdit_ApprovedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanEdit(ExpenseState.Approved, OwnerId, OwnerId));

    // CanSubmit

    [TestMethod]
    public void CanSubmit_DraftAndOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanSubmit(ExpenseState.Draft, OwnerId, OwnerId));

    [TestMethod]
    public void CanSubmit_DraftButNotOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanSubmit(ExpenseState.Draft, OwnerId, ActorId));

    [TestMethod]
    public void CanSubmit_AlreadySubmitted_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanSubmit(ExpenseState.Submitted, OwnerId, OwnerId));

    [TestMethod]
    public void CanSubmit_ApprovedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanSubmit(ExpenseState.Approved, OwnerId, OwnerId));

    [TestMethod]
    public void CanSubmit_RejectedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanSubmit(ExpenseState.Rejected, OwnerId, OwnerId));

    [TestMethod]
    public void CanSubmit_PaidState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanSubmit(ExpenseState.Paid, OwnerId, OwnerId));

    // CanApprove

    [TestMethod]
    public void CanApprove_SubmittedAndNotOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanApprove(ExpenseState.Submitted, OwnerId, ActorId));

    [TestMethod]
    public void CanApprove_DraftState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Draft, OwnerId, ActorId));

    [TestMethod]
    public void CanApprove_AlreadyApproved_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Approved, OwnerId, ActorId));

    // CanReject

    [TestMethod]
    public void CanReject_SubmittedAndNotOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanReject(ExpenseState.Submitted, OwnerId, ActorId));

    [TestMethod]
    public void CanReject_DraftState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Draft, OwnerId, ActorId));

    [TestMethod]
    public void CanReject_AlreadyRejected_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Rejected, OwnerId, ActorId));

    // CanPay

    [TestMethod]
    public void CanPay_ApprovedAndNotOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanPay(ExpenseState.Approved, OwnerId, ActorId));

    [TestMethod]
    public void CanPay_SubmittedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanPay(ExpenseState.Submitted, OwnerId, ActorId));

    [TestMethod]
    public void CanPay_AlreadyPaid_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanPay(ExpenseState.Paid, OwnerId, ActorId));

    // IsFinalState

    [TestMethod]
    public void IsFinalState_RejectedState_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.IsFinalState(ExpenseState.Rejected));

    [TestMethod]
    public void IsFinalState_PaidState_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.IsFinalState(ExpenseState.Paid));

    [TestMethod]
    public void IsFinalState_DraftState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsFinalState(ExpenseState.Draft));

    [TestMethod]
    public void IsFinalState_SubmittedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsFinalState(ExpenseState.Submitted));

    [TestMethod]
    public void IsFinalState_ApprovedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsFinalState(ExpenseState.Approved));
}
