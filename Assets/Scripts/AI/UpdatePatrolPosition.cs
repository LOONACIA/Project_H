using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

[System.Serializable]
public class UpdatePatrolPosition : NavMeshMovement
{
    [SerializeField]
    private SharedTransform m_destination;

    [SerializeField]
    private SharedTransformList m_patrolPoints;

    private int m_currentPatrolPointIndex;

    public override TaskStatus OnUpdate()
    {
        if (m_patrolPoints.Value.Count == 0)
        {
            return TaskStatus.Failure;
        }
        
        if (HasArrived())
        {
            m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % m_patrolPoints.Value.Count;
            m_destination.Value = m_patrolPoints.Value[m_currentPatrolPointIndex];
        }

        return TaskStatus.Failure;
    }
}