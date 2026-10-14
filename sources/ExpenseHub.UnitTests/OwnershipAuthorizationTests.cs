using ExpenseHub.Api.Enums;
using ExpenseHub.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseHub.UnitTests;

/// <summary>Verifies that ownership rules prevent self-approval and self-payment.</summary>
[TestClass]
public class OwnershipAuthorizationTests
{
    private const string UserId = "user-1";
    private const string OtherId = "user-2";

    // Self-approval prevention

    [TestMethod]
    public void CanApprove_SameUserAsOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Submitted, UserId, UserId));

    [TestMethod]
    public void CanApprove_DifferentUserFromOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanApprove(ExpenseState.Submitted, UserId, OtherId));

    // Self-rejection prevention

    [TestMethod]
    public void CanReject_SameUserAsOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanReject(ExpenseState.Submitted, UserId, UserId));

    [TestMethod]
    public void CanReject_DifferentUserFromOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanReject(ExpenseState.Submitted, UserId, OtherId));

    // Self-payment prevention

    [TestMethod]
    public void CanPay_SameUserAsOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanPay(ExpenseState.Approved, UserId, UserId));

    [TestMethod]
    public void CanPay_DifferentUserFromOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanPay(ExpenseState.Approved, UserId, OtherId));

    // Edit ownership

    [TestMethod]
    public void CanEdit_SameUserAsOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanEdit(ExpenseState.Draft, UserId, UserId));

    [TestMethod]
    public void CanEdit_DifferentUserFromOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanEdit(ExpenseState.Draft, UserId, OtherId));

    // Submit ownership

    [TestMethod]
    public void CanSubmit_SameUserAsOwner_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.CanSubmit(ExpenseState.Draft, UserId, UserId));

    [TestMethod]
    public void CanSubmit_DifferentUserFromOwner_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.CanSubmit(ExpenseState.Draft, UserId, OtherId));
}
