using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class WaitUntilNavMeshAgentEnabled : Conditional
{
    private NavMeshAgent m_navMeshAgent;

    public override void OnAwake()
    {
        base.OnAwake();

        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        return m_navMeshAgent.enabled ? TaskStatus.Success : TaskStatus.Running;
    }
}