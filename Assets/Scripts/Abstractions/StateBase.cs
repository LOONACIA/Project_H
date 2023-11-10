public abstract class StateBase<T> 
    where T : class
{
    protected readonly T m_owner;
    
    protected StateBase(T owner, State type)
    {
        m_owner = owner;
        Type = type;
    }
    
    public State Type { get; }

    public abstract void Enter();

    public abstract void Execute();

    public abstract void FixedExecute();

    public abstract void Exit();
}