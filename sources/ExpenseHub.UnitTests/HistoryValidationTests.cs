using ExpenseHub.Api.Enums;
using ExpenseHub.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseHub.UnitTests;

/// <summary>Verifies visibility rules for expense history access by each role.</summary>
[TestClass]
public class HistoryValidationTests
{
    private const string OwnerId = "owner-1";
    private const string OtherId = "other-2";

    // Employee visibility — only owns their own expenses

    [TestMethod]
    public void IsVisibleToEmployee_SameUser_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.IsVisibleToEmployee(OwnerId, OwnerId));

    [TestMethod]
    public void IsVisibleToEmployee_DifferentUser_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToEmployee(OwnerId, OtherId));

    // Approver visibility — only Submitted

    [TestMethod]
    public void IsVisibleToApprover_SubmittedState_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.IsVisibleToApprover(ExpenseState.Submitted));

    [TestMethod]
    public void IsVisibleToApprover_DraftState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToApprover(ExpenseState.Draft));

    [TestMethod]
    public void IsVisibleToApprover_ApprovedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToApprover(ExpenseState.Approved));

    [TestMethod]
    public void IsVisibleToApprover_RejectedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToApprover(ExpenseState.Rejected));

    [TestMethod]
    public void IsVisibleToApprover_PaidState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToApprover(ExpenseState.Paid));

    // Finance visibility — Approved and Paid

    [TestMethod]
    public void IsVisibleToFinance_ApprovedState_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.IsVisibleToFinance(ExpenseState.Approved));

    [TestMethod]
    public void IsVisibleToFinance_PaidState_ReturnsTrue()
        => Assert.IsTrue(ExpenseWorkflow.IsVisibleToFinance(ExpenseState.Paid));

    [TestMethod]
    public void IsVisibleToFinance_DraftState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToFinance(ExpenseState.Draft));

    [TestMethod]
    public void IsVisibleToFinance_SubmittedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToFinance(ExpenseState.Submitted));

    [TestMethod]
    public void IsVisibleToFinance_RejectedState_ReturnsFalse()
        => Assert.IsFalse(ExpenseWorkflow.IsVisibleToFinance(ExpenseState.Rejected));

    // Final state guards prevent further transitions

    [TestMethod]
    public void IsFinalState_PaidExpense_CannotBeApproved()
    {
        Assert.IsTrue(ExpenseWorkflow.IsFinalState(ExpenseState.Paid));
        Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Paid, OwnerId, OtherId));
    }

    [TestMethod]
    public void IsFinalState_RejectedExpense_CannotBeApprovedOrPaid()
    {
        Assert.IsTrue(ExpenseWorkflow.IsFinalState(ExpenseState.Rejected));
        Assert.IsFalse(ExpenseWorkflow.CanApprove(ExpenseState.Rejected, OwnerId, OtherId));
        Assert.IsFalse(ExpenseWorkflow.CanPay(ExpenseState.Rejected, OwnerId, OtherId));
    }

    // Non-final states

    [TestMethod]
    public void IsFinalState_DraftExpense_IsNotFinal()
        => Assert.IsFalse(ExpenseWorkflow.IsFinalState(ExpenseState.Draft));

    [TestMethod]
    public void IsFinalState_SubmittedExpense_IsNotFinal()
        => Assert.IsFalse(ExpenseWorkflow.IsFinalState(ExpenseState.Submitted));

    [TestMethod]
    public void IsFinalState_ApprovedExpense_IsNotFinal()
        => Assert.IsFalse(ExpenseWorkflow.IsFinalState(ExpenseState.Approved));
}
