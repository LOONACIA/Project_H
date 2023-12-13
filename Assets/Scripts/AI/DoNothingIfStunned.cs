using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class DoNothingIfStunned : Action
{
    private ActorStatus m_status;

    private NavMeshAgent m_navMeshAgent;
    
    private bool m_isAborted;
    
    public override void OnAwake()
    {
        base.OnAwake();

        m_status = GetComponent<ActorStatus>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        if (m_status.IsStunned)
        {
            m_navMeshAgent.isStopped = m_isAborted = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        return m_status.IsStunned ? TaskStatus.Running : TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        if (m_isAborted)
        {
            m_navMeshAgent.isStopped = false;
        }
    }
}
