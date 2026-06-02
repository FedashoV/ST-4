using Stateless;

namespace BugPro;

public enum BugState
{
    Open,
    Assigned,
    InProgress,
    Fixed,
    Verified,
    Closed,
    Reopened,
    Rejected,
    Deferred
}

public enum BugTrigger
{
    Assign,
    StartProgress,
    Fix,
    Verify,
    Close,
    Reopen,
    Reject,
    Defer,
    Reassign  // Добавлен триггер Reassign
}

public class Bug
{
    private StateMachine<BugState, BugTrigger> _machine;
    private BugState _currentState;
    private string _description;
    private string _assignee;

    public Bug(string description)
    {
        _description = description;
        _machine = new StateMachine<BugState, BugTrigger>(() => _currentState, state => _currentState = state);
        _currentState = BugState.Open;

        ConfigureMachine();
    }

    public BugState CurrentState => _currentState;
    public string Description => _description;
    public string Assignee => _assignee;

    private void ConfigureMachine()
    {
        _machine.Configure(BugState.Open)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Reject, BugState.Rejected)
            .Permit(BugTrigger.Defer, BugState.Deferred);

        _machine.Configure(BugState.Assigned)
            .Permit(BugTrigger.StartProgress, BugState.InProgress)
            .PermitReentry(BugTrigger.Reassign)  // ← PermitReentry, а не PermitReentrant
            .OnEntryFrom(BugTrigger.Assign, () => _assignee = "Developer")
            .OnEntryFrom(BugTrigger.Reassign, () => _assignee = "Another Developer");

        _machine.Configure(BugState.InProgress)
            .Permit(BugTrigger.Fix, BugState.Fixed)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        _machine.Configure(BugState.Fixed)
            .Permit(BugTrigger.Verify, BugState.Verified)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Verified)
            .Permit(BugTrigger.Close, BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Reopened)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        _machine.Configure(BugState.Rejected)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Deferred)
            .Permit(BugTrigger.Assign, BugState.Assigned);
    }

    public void Assign() => _machine.Fire(BugTrigger.Assign);
    public void StartProgress() => _machine.Fire(BugTrigger.StartProgress);
    public void Fix() => _machine.Fire(BugTrigger.Fix);
    public void Verify() => _machine.Fire(BugTrigger.Verify);
    public void Close() => _machine.Fire(BugTrigger.Close);
    public void Reopen() => _machine.Fire(BugTrigger.Reopen);
    public void Reject() => _machine.Fire(BugTrigger.Reject);
    public void Defer() => _machine.Fire(BugTrigger.Defer);
    
    public void Reassign()
    {
        if (_currentState == BugState.Assigned)
        {
            _machine.Fire(BugTrigger.Reassign);
        }
        else
        {
            throw new InvalidOperationException("Cannot reassign bug that is not assigned");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var bug = new Bug("Application crashes on startup");
        
        Console.WriteLine($"Initial state: {bug.CurrentState}");
        Console.WriteLine($"Description: {bug.Description}");
        
        bug.Assign();
        Console.WriteLine($"After assign: {bug.CurrentState}, Assignee: {bug.Assignee}");
        
        bug.Reassign();
        Console.WriteLine($"After reassign: {bug.CurrentState}, Assignee: {bug.Assignee}");
        
        bug.StartProgress();
        Console.WriteLine($"After start progress: {bug.CurrentState}");
        
        bug.Fix();
        Console.WriteLine($"After fix: {bug.CurrentState}");
        
        bug.Verify();
        Console.WriteLine($"After verify: {bug.CurrentState}");
        
        bug.Close();
        Console.WriteLine($"After close: {bug.CurrentState}");
        
        Console.WriteLine("\nTesting reopen scenario:");
        bug.Reopen();
        Console.WriteLine($"After reopen: {bug.CurrentState}");
        
        Console.WriteLine("\nAll operations completed successfully!");
    }
}