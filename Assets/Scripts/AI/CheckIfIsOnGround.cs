using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class CheckIfIsOnGround : Conditional
{
    private Monster m_monster;

    private NavMeshAgent m_navMeshAgent;

    public override void OnAwake()
    {
        base.OnAwake();

        m_monster = GetComponent<Monster>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        return m_navMeshAgent.enabled || m_monster.Movement.IsOnGround ? TaskStatus.Success : TaskStatus.Failure;
    }
}
