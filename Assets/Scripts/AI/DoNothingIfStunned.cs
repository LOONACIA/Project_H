using BehaviorDesigner.Runtime.Tasks;

public class DoNothingIfStunned : Action
{
    private ActorStatus m_status;
    
    public override void OnAwake()
    {
        base.OnAwake();

        m_status = GetComponent<ActorStatus>();
    }

    public override TaskStatus OnUpdate()
    {
        return m_status.IsStunned ? TaskStatus.Running : TaskStatus.Failure;
    }
}
