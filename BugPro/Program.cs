using Stateless;

namespace BugPro
{
    public enum State
    {
        NewDefect,
        Analysis,
        Resolution,
        Returned,
        Closed,
        Reopened,
        NeedMoreInfo,
        Review
    }

    public enum Trigger
    {
        Analyze,
        Reject,
        AskInfo,
        ProvideInfo,
        StartFix,
        VerifySuccess,
        VerifyFailure,
        ReportCannotReproduce,
        ReturnForInfo,
        ContinueFix,
        ConfirmNotRepro,
        ConfirmBugExists,
        Reopen,
        AnalyzeAgain
    }

    public class Bug
    {
        private readonly StateMachine<State, Trigger> _machine;

        public Bug()
        {
            _machine = new StateMachine<State, Trigger>(State.NewDefect);

            _machine.Configure(State.NewDefect)
                .Permit(Trigger.Analyze, State.Analysis);

            _machine.Configure(State.Analysis)
                .Permit(Trigger.Reject, State.Returned)
                .Permit(Trigger.AskInfo, State.NeedMoreInfo)
                .Permit(Trigger.StartFix, State.Resolution);

            _machine.Configure(State.NeedMoreInfo)
                .Permit(Trigger.ProvideInfo, State.Analysis)
                .Permit(Trigger.ContinueFix, State.Resolution);

            _machine.Configure(State.Resolution)
                .Permit(Trigger.VerifySuccess, State.Closed)
                .Permit(Trigger.VerifyFailure, State.Returned)
                .Permit(Trigger.ReportCannotReproduce, State.Review)
                .Permit(Trigger.ReturnForInfo, State.NeedMoreInfo);

            _machine.Configure(State.Review)
                .Permit(Trigger.ConfirmNotRepro, State.Closed)
                .Permit(Trigger.ConfirmBugExists, State.Returned);

            _machine.Configure(State.Closed)
                .Permit(Trigger.Reopen, State.Reopened);

            _machine.Configure(State.Reopened)
                .Permit(Trigger.AnalyzeAgain, State.Analysis);
        }

        public void Analyze() => _machine.Fire(Trigger.Analyze);
        public void Reject() => _machine.Fire(Trigger.Reject);
        public void AskInfo() => _machine.Fire(Trigger.AskInfo);
        public void ProvideInfo() => _machine.Fire(Trigger.ProvideInfo);
        public void StartFix() => _machine.Fire(Trigger.StartFix);
        public void VerifySuccess() => _machine.Fire(Trigger.VerifySuccess);
        public void VerifyFailure() => _machine.Fire(Trigger.VerifyFailure);
        public void ReportCannotReproduce() => _machine.Fire(Trigger.ReportCannotReproduce);
        public void ConfirmNotRepro() => _machine.Fire(Trigger.ConfirmNotRepro);
        public void ConfirmBugExists() => _machine.Fire(Trigger.ConfirmBugExists);
        public void Reopen() => _machine.Fire(Trigger.Reopen);
        public void AnalyzeAgain() => _machine.Fire(Trigger.AnalyzeAgain);
        public void ReturnForInfo() => _machine.Fire(Trigger.ReturnForInfo);
        public void ContinueFix() => _machine.Fire(Trigger.ContinueFix);

        public State CurrentState => _machine.State;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Тест прохода по схеме:\n");
            var bug = new Bug();
            bug.Analyze();
            bug.StartFix();
            Console.WriteLine($"Current: {bug.CurrentState} (Resolution)");
            bug.ReturnForInfo();
            Console.WriteLine($"After Request: {bug.CurrentState} (NeedMoreInfo)");
            bug.ContinueFix();
            Console.WriteLine($"After Info: {bug.CurrentState} (Resolution)");
            bug.VerifySuccess();
            Console.WriteLine($"Final: {bug.CurrentState} (Closed)");
            Console.WriteLine("\n Дефект исправлен");
        }
    }
}