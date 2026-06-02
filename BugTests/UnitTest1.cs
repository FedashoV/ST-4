using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using Stateless;

namespace BugTests;

[TestClass]
public class BugStateTests
{
    private Bug _bug;

    [TestInitialize]
    public void Setup()
    {
        _bug = new Bug("Test bug description");
    }

    [TestMethod]
    public void Bug_InitialState_ShouldBeOpen()
    {
        Assert.AreEqual(BugState.Open, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Assign_ShouldChangeStateToAssigned()
    {
        _bug.Assign();
        Assert.AreEqual(BugState.Assigned, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Assign_ShouldSetAssignee()
    {
        _bug.Assign();
        Assert.AreEqual("Developer", _bug.Assignee);
    }

    [TestMethod]
    public void Bug_FromOpen_StartProgress_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _bug.StartProgress());
    }

    [TestMethod]
    public void Bug_FromAssigned_StartProgress_ShouldChangeStateToInProgress()
    {
        _bug.Assign();
        _bug.StartProgress();
        Assert.AreEqual(BugState.InProgress, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromInProgress_Fix_ShouldChangeStateToFixed()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        Assert.AreEqual(BugState.Fixed, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromFixed_Verify_ShouldChangeStateToVerified()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        Assert.AreEqual(BugState.Verified, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromVerified_Close_ShouldChangeStateToClosed()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        Assert.AreEqual(BugState.Closed, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromClosed_Reopen_ShouldChangeStateToReopened()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        _bug.Reopen();
        Assert.AreEqual(BugState.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromOpen_Reject_ShouldChangeStateToRejected()
    {
        _bug.Reject();
        Assert.AreEqual(BugState.Rejected, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromRejected_Reopen_ShouldChangeStateToReopened()
    {
        _bug.Reject();
        _bug.Reopen();
        Assert.AreEqual(BugState.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromOpen_Defer_ShouldChangeStateToDeferred()
    {
        _bug.Defer();
        Assert.AreEqual(BugState.Deferred, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromDeferred_Assign_ShouldChangeStateToAssigned()
    {
        _bug.Defer();
        _bug.Assign();
        Assert.AreEqual(BugState.Assigned, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromAssigned_Reassign_ShouldKeepAssignedState()
    {
        _bug.Assign();
        _bug.Reassign();
        Assert.AreEqual(BugState.Assigned, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromInProgress_Reject_ShouldChangeStateToRejected()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Reject();
        Assert.AreEqual(BugState.Rejected, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromFixed_Reopen_ShouldChangeStateToReopened()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Reopen();
        Assert.AreEqual(BugState.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromReopened_Assign_ShouldChangeStateToAssigned()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        _bug.Reopen();
        _bug.Assign();
        Assert.AreEqual(BugState.Assigned, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromOpen_Close_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _bug.Close());
    }

    [TestMethod]
    public void Bug_Description_ShouldBeSetCorrectly()
    {
        string description = "Memory leak occurs after 2 hours";
        var newBug = new Bug(description);
        Assert.AreEqual(description, newBug.Description);
    }

    [TestMethod]
    public void Bug_CompleteWorkflow_ShouldEndInClosedState()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        Assert.AreEqual(BugState.Closed, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenAfterReject_ShouldBeReopened()
    {
        _bug.Reject();
        _bug.Reopen();
        Assert.AreEqual(BugState.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromVerified_Reopen_ShouldChangeStateToReopened()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Reopen();
        Assert.AreEqual(BugState.Reopened, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FromReopened_Reject_ShouldChangeStateToRejected()
    {
        _bug.Assign();
        _bug.StartProgress();
        _bug.Fix();
        _bug.Verify();
        _bug.Close();
        _bug.Reopen();
        _bug.Reject();
        Assert.AreEqual(BugState.Rejected, _bug.CurrentState);
    }

    [TestMethod]
    public void Bug_MultipleTransitions_ShouldMaintainCorrectState()
    {
        _bug.Assign();
        Assert.AreEqual(BugState.Assigned, _bug.CurrentState);
        
        _bug.StartProgress();
        Assert.AreEqual(BugState.InProgress, _bug.CurrentState);
        
        _bug.Fix();
        Assert.AreEqual(BugState.Fixed, _bug.CurrentState);
        
        _bug.Verify();
        Assert.AreEqual(BugState.Verified, _bug.CurrentState);
        
        _bug.Close();
        Assert.AreEqual(BugState.Closed, _bug.CurrentState);
    }
}