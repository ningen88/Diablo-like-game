using System;


public class Transition
{
    public Func<bool> _condition { get; }

    public IState _to { get; }

    public Transition(Func<bool> condition, IState to)
    {
        _condition = condition;
        _to = to;
    }
}
