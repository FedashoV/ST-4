using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class BugWorkflowTests
    {
        [TestMethod] public void T01_StartState() => Assert.AreEqual(State.NewDefect, new Bug().CurrentState);
        [TestMethod] public void T02_New_Analyze() { var b = new Bug(); b.Analyze(); Assert.AreEqual(State.Analysis, b.CurrentState); }
        [TestMethod] public void T03_Analysis_Reject() { var b = new Bug(); b.Analyze(); b.Reject(); Assert.AreEqual(State.Returned, b.CurrentState); }
        [TestMethod] public void T04_Analysis_StartFix() { var b = new Bug(); b.Analyze(); b.StartFix(); Assert.AreEqual(State.Resolution, b.CurrentState); }
        [TestMethod] public void T05_Analysis_AskInfo() { var b = new Bug(); b.Analyze(); b.AskInfo(); Assert.AreEqual(State.NeedMoreInfo, b.CurrentState); }
        [TestMethod] public void T06_NeedMoreInfo_To_Analysis() { var b = new Bug(); b.Analyze(); b.AskInfo(); b.ProvideInfo(); Assert.AreEqual(State.Analysis, b.CurrentState); }
        [TestMethod] public void T07_NeedMoreInfo_To_Resolution() { var b = new Bug(); b.Analyze(); b.AskInfo(); b.ContinueFix(); Assert.AreEqual(State.Resolution, b.CurrentState); }
        [TestMethod] public void T08_Resolution_Success() { var b = new Bug(); b.Analyze(); b.StartFix(); b.VerifySuccess(); Assert.AreEqual(State.Closed, b.CurrentState); }
        [TestMethod] public void T09_Resolution_Fail() { var b = new Bug(); b.Analyze(); b.StartFix(); b.VerifyFailure(); Assert.AreEqual(State.Returned, b.CurrentState); }
        [TestMethod] public void T10_Resolution_CantRepro() { var b = new Bug(); b.Analyze(); b.StartFix(); b.ReportCannotReproduce(); Assert.AreEqual(State.Review, b.CurrentState); }
        [TestMethod] public void T11_Resolution_To_NeedMoreInfo() { var b = new Bug(); b.Analyze(); b.StartFix(); b.ReturnForInfo(); Assert.AreEqual(State.NeedMoreInfo, b.CurrentState); }
        [TestMethod] public void T12_Review_Close() { var b = new Bug(); b.Analyze(); b.StartFix(); b.ReportCannotReproduce(); b.ConfirmNotRepro(); Assert.AreEqual(State.Closed, b.CurrentState); }
        [TestMethod] public void T13_Review_Return() { var b = new Bug(); b.Analyze(); b.StartFix(); b.ReportCannotReproduce(); b.ConfirmBugExists(); Assert.AreEqual(State.Returned, b.CurrentState); }
        [TestMethod] public void T14_Closed_Reopen() { var b = new Bug(); b.Analyze(); b.StartFix(); b.VerifySuccess(); b.Reopen(); Assert.AreEqual(State.Reopened, b.CurrentState); }
        [TestMethod] public void T15_Reopened_Analyze() { var b = new Bug(); b.Analyze(); b.StartFix(); b.VerifySuccess(); b.Reopen(); b.AnalyzeAgain(); Assert.AreEqual(State.Analysis, b.CurrentState); }
        [TestMethod] public void T16_Invalid_New_Reject() => Assert.ThrowsException<InvalidOperationException>(() => new Bug().Reject());
        [TestMethod] public void T17_Invalid_Analysis_Success() { var b = new Bug(); b.Analyze(); Assert.ThrowsException<InvalidOperationException>(() => b.VerifySuccess()); }
        [TestMethod] public void T18_Invalid_Resolution_DirectClose() { var b = new Bug(); b.Analyze(); b.StartFix(); Assert.ThrowsException<InvalidOperationException>(() => b.ConfirmNotRepro()); }
        [TestMethod] public void T19_Invalid_Analysis_CantRepro() { var b = new Bug(); b.Analyze(); Assert.ThrowsException<InvalidOperationException>(() => b.ReportCannotReproduce()); }
        [TestMethod] public void T20_Invalid_Returned_Reopen() { var b = new Bug(); b.Analyze(); b.Reject(); Assert.ThrowsException<InvalidOperationException>(() => b.Reopen()); }
        [TestMethod] public void T21_Invalid_New_StartFix() => Assert.ThrowsException<InvalidOperationException>(() => new Bug().StartFix());
        [TestMethod] public void T22_Invalid_Closed_VerifySuccess() { var b = new Bug(); b.Analyze(); b.StartFix(); b.VerifySuccess(); Assert.ThrowsException<InvalidOperationException>(() => b.VerifySuccess()); }
        [TestMethod] public void T23_Invalid_Review_StartFix() { var b = new Bug(); b.Analyze(); b.StartFix(); b.ReportCannotReproduce(); Assert.ThrowsException<InvalidOperationException>(() => b.StartFix()); }
        [TestMethod] public void T24_Invalid_NeedMoreInfo_Reject() { var b = new Bug(); b.Analyze(); b.AskInfo(); Assert.ThrowsException<InvalidOperationException>(() => b.Reject()); }
    }
}