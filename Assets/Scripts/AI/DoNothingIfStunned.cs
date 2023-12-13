using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class DoNothingIfStunned : Action
{
    private ActorStatus m_status;

    private NavMeshAgent m_navMeshAgent;
    
    public override void OnAwake()
    {
        base.OnAwake();

        m_status = GetComponent<ActorStatus>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        m_navMeshAgent.isStopped = m_status.IsStunned;
    }

    public override TaskStatus OnUpdate()
    {
        return m_status.IsStunned ? TaskStatus.Running : TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        m_navMeshAgent.isStopped = false;
    }
}
