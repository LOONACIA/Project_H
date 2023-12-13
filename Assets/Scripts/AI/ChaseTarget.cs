using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ChaseTarget : Action
{
    [SerializeField]
    private SharedTransform m_destination;

    private NavMeshAgent m_navMeshAgent;
    
    private NavMeshPath m_path;

    public override void OnAwake()
    {
        base.OnAwake();
        
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_path = new();
    }
    
    protected Vector3 GetDestination()
    {
        if (m_destination == null)
        {
            return default;
        }

        Vector3 position = m_destination.Value.position;
        if (!m_navMeshAgent.CalculatePath(position, m_path))
        {
            return default;
        }

        return m_path.status switch
        {
            NavMeshPathStatus.PathComplete => position,
            NavMeshPathStatus.PathPartial => m_path.corners[0],
            NavMeshPathStatus.PathInvalid => default,
            _ => throw new System.ArgumentOutOfRangeException()
        };
    }
    
    public override void OnReset()
    {
        m_destination = null;
    }
}
