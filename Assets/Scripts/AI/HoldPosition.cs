using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

[System.Serializable]
public class HoldPosition : NavMeshMovement
{
    [SerializeField]
	private SharedTransform m_destination;

    public override TaskStatus OnUpdate()
    {
        if (m_destination.Value == null)
        {
            return TaskStatus.Failure;
        }

        if (HasArrived())
        {
            navMeshAgent.SetDestination(m_destination.Value.position);
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
