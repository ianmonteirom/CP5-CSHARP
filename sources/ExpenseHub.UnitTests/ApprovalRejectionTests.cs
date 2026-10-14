using ExpenseHub.Api.Enums;
using ExpenseHub.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseHub.UnitTests;

/// <summary>Verifies approval and rejection state machine rules.</summary>
[TestClass]
public class ApprovalRejectionTests
{
    private const string OwnerId = "owner-1";
    private const string ApproverId = "approver-2";

    // Approve requires Submitted state

    [TestMethod]
    public void CanApprove_ExpenseInSubmittedState_AllowsApproval()
        => Assert.IsTrue(ExpenseWorkflow.CanApprove(ExpenseState.Submitted, OwnerId, ApproverId));

    [TestMethod]
    public void CanApprove_ExpenseInDraftState_DeniesApproval()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Draft, OwnerId, ApproverId));

    [TestMethod]
    public void CanApprove_ExpenseAlreadyApproved_DeniesApproval()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Approved, OwnerId, ApproverId));

    [TestMethod]
    public void CanApprove_ExpenseRejected_DeniesApproval()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Rejected, OwnerId, ApproverId));

    [TestMethod]
    public void CanApprove_ExpensePaid_DeniesApproval()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Paid, OwnerId, ApproverId));

    // Reject requires Submitted state

    [TestMethod]
    public void CanReject_ExpenseInSubmittedState_AllowsRejection()
        => Assert.IsTrue(ExpenseWorkflow.CanReject(ExpenseState.Submitted, OwnerId, ApproverId));

    [TestMethod]
    public void CanReject_ExpenseInDraftState_DeniesRejection()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Draft, OwnerId, ApproverId));

    [TestMethod]
    public void CanReject_ExpenseAlreadyApproved_DeniesRejection()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Approved, OwnerId, ApproverId));

    [TestMethod]
    public void CanReject_ExpenseAlreadyRejected_DeniesRejection()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Rejected, OwnerId, ApproverId));

    [TestMethod]
    public void CanReject_ExpensePaid_DeniesRejection()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Paid, OwnerId, ApproverId));

    // After rejection, state is terminal

    [TestMethod]
    public void IsFinalState_AfterRejection_IsTerminal()
        => Assert.IsTrue(ExpenseWorkflow.IsFinalState(ExpenseState.Rejected));

    // After approval, payment can proceed

    [TestMethod]
    public void CanPay_AfterApproval_AllowsPayment()
        => Assert.IsTrue(ExpenseWorkflow.CanPay(ExpenseState.Approved, OwnerId, ApproverId));

    [TestMethod]
    public void CanPay_BeforeApproval_DeniesPayment()
        => Assert.IsFalse(ExpenseWorkflow.CanPay(ExpenseState.Submitted, OwnerId, ApproverId));

    // Approve and reject are mutually exclusive terminal paths

    [TestMethod]
    public void CanApprove_AlreadyRejected_DeniesApproval()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Rejected, OwnerId, ApproverId));

    [TestMethod]
    public void CanReject_AlreadyPaid_DeniesRejection()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Paid, OwnerId, ApproverId));
}
