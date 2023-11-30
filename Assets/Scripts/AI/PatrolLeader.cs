using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

[TaskDescription("Patrol around the specified waypoints using the Unity NavMesh.")]
[TaskCategory("Movement")]
[UnityEngine.HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
public class PatrolLeader : NavMeshMovement
{
    [UnityEngine.Tooltip("Should the agent patrol the waypoints randomly?")]
    public SharedBool randomPatrol = false;

    [UnityEngine.Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
    public SharedFloat waypointPauseDuration = 0;
    
    public SharedFloat waitAngle;

    [UnityEngine.Tooltip("The waypoints to move to")]
    public SharedGameObjectList waypoints;

    // The current index that we are heading towards within the waypoints array
    private int m_waypointIndex;

    private float m_waypointReachedTime;

    private Vector3 m_targetPosition;
    
    private bool m_isTargetSet;

    public override void OnStart()
    {
        base.OnStart();

        // initially move towards the closest waypoint
        float distance = Mathf.Infinity;
        float localDistance;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) <
                distance)
            {
                distance = localDistance;
                m_waypointIndex = i;
            }
        }

        m_waypointReachedTime = -1;
        m_targetPosition = Target();
        SetDestination(m_targetPosition);
    }

    // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
    public override TaskStatus OnUpdate()
    {
        if (waypoints.Value.Count == 0)
        {
            return TaskStatus.Failure;
        }

        if (m_isTargetSet)
        {
            Vector3 dir = (m_targetPosition.GetFlatVector() - transform.position.GetFlatVector()).normalized;
            var rotation = Quaternion.LookRotation(dir, transform.up);
            navMeshAgent.isStopped = Quaternion.Angle(transform.rotation, rotation) > waitAngle.Value;

            if (navMeshAgent.isStopped)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 3f * Time.deltaTime);
            }
            else
            {
                m_isTargetSet = false;
            }
        }

        if (HasArrived())
        {
            if (m_waypointReachedTime < 0)
            {
                m_waypointReachedTime = Time.time;
                SetNextTarget();
                m_targetPosition = Target();
            }

            // wait the required duration before switching waypoints.
            if (m_waypointReachedTime + waypointPauseDuration.Value <= Time.time)
            {
                SetDestination(m_targetPosition);
                m_waypointReachedTime = -1;
                m_isTargetSet = true;
            }
        }

        return TaskStatus.Running;
    }

    private void SetNextTarget()
    {
        if (randomPatrol.Value)
        {
            if (waypoints.Value.Count == 1)
            {
                m_waypointIndex = 0;
            }
            else
            {
                // prevent the same waypoint from being selected
                var newWaypointIndex = m_waypointIndex;
                while (newWaypointIndex == m_waypointIndex)
                {
                    newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                }

                m_waypointIndex = newWaypointIndex;
            }
        }
        else
        {
            m_waypointIndex = (m_waypointIndex + 1) % waypoints.Value.Count;
        }
    }

    // Return the current waypoint index position
    private Vector3 Target()
    {
        if (m_waypointIndex >= waypoints.Value.Count)
        {
            return transform.position;
        }

        return waypoints.Value[m_waypointIndex].transform.position;
    }

    // Reset the public variables
    public override void OnReset()
    {
        base.OnReset();

        randomPatrol = false;
        waitAngle = 10;
        waypointPauseDuration = 0;
        waypoints = null;
    }

    // Draw a gizmo indicating a patrol 
    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (waypoints == null || waypoints.Value == null)
        {
            return;
        }

        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if (waypoints.Value[i] != null)
            {
                UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position,
                    waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
            }
        }

        UnityEditor.Handles.color = oldColor;
#endif
    }
}