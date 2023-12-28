using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

// Shooter BT에서 타겟을 조준하는데 사용
[System.Serializable]
public class AimTarget : Action
{
    private static readonly int s_isAiming = Animator.StringToHash("IsAiming");
    
    [SerializeField]
    [Tooltip("조준할 타겟")]
    private SharedTransform m_target;
    
    [SerializeField]
    [Tooltip("조준하는데 걸리는 시간")]
	private SharedFloat m_aimSeconds;
    
    [SerializeField]
    [Tooltip("조준 후 홀드하는 시간")]
    private SharedFloat m_holdSeconds;

    [SerializeField]
    [Tooltip("조준 속도")]
    private SharedFloat m_aimSpeed;
    
    [SerializeField]
    [Tooltip("조준을 시작할 때 타겟의 위치를 랜덤으로 변경할 범위")]
    private SharedFloat m_randomRange;
    
    [SerializeField]
    private SharedBool m_isAiming;
    
    private Monster m_owner;
    
    private CapsuleCollider m_collider;

    private NavMeshAgent m_agent;
    
    private Vector3 m_previousTargetPosition;

    private float m_timer;
    
    private float m_aimDuration;

    private float m_waitDuration;

    private bool m_targetIsActor;

    private Transform m_targetActor;

    //NavMeshAgent의 공격 전 Priority, ObstacleAvoidance
    private int m_orgPriority;
    private ObstacleAvoidanceType m_orgAvoidance;

    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        if (m_target.Value == null)
        {
            return;
        }

        m_targetIsActor = (m_targetActor = m_target.Value.GetComponentInChildren<CinemachineVirtualCamera>().transform) != null;
        
        m_owner.Animator.SetBool(s_isAiming, true);

        m_collider = m_target.Value.GetComponent<CapsuleCollider>();
        if (m_previousTargetPosition == Vector3.zero)
        {
            Vector3 offset = m_collider.center + m_collider.height / 4f * Vector3.up;
            Vector3 target = m_targetIsActor ? m_targetActor.position : m_target.Value.position + offset;
            m_previousTargetPosition = target + (Random.insideUnitSphere * m_randomRange.Value);
        }

        //조준 중일 때 다른 적의 이동이 막히지 않게 하기 위함
        m_orgAvoidance = m_agent.obstacleAvoidanceType;
        m_orgPriority = m_agent.avoidancePriority;
        m_agent.avoidancePriority = 99;
        m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        m_aimDuration = m_aimSeconds.Value;
        m_waitDuration = m_aimDuration + m_holdSeconds.Value;
        m_timer = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_target.Value == null)
        {
            return TaskStatus.Failure;
        }
        
        m_isAiming.Value = true;

        if (m_owner.Attack.IsAttacking)
        {
            return TaskStatus.Running;
        }
        
        if (Time.time - m_timer < m_aimDuration)
        {
            //타겟이 죽었을 경우 조준을 유지하고 Return;
            if (!m_target.Value)
            {
                return TaskStatus.Running;
            }

            Vector3 offset = m_collider.center + m_collider.height / 4f * Vector3.up;
            Vector3 target = m_targetIsActor ? m_targetActor.position - Vector3.up * 0.05f : m_target.Value.position + offset;
            Vector3 targetPosition = Vector3.Lerp(m_previousTargetPosition, target, (Time.time - m_timer) / m_aimSpeed.Value);

            Vector3 lookAt = new(targetPosition.x, m_owner.transform.position.y, targetPosition.z);
            m_owner.transform.LookAt(lookAt);
            
            m_previousTargetPosition = targetPosition;
            m_owner.Attack.Target = targetPosition;
            
            return TaskStatus.Running;
        }
        else if (Time.time - m_timer < m_waitDuration)
        {
            m_owner.Attack.Target = m_previousTargetPosition;
            // Do nothing
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }

    public override void OnEnd()
    {
        m_isAiming.Value = false;
        m_owner.Animator.SetBool(s_isAiming, false);
        m_previousTargetPosition = Vector3.zero;

        //조준 종료, 기존 Agent 값으로 돌아감
        m_agent.obstacleAvoidanceType = m_orgAvoidance;
        m_agent.avoidancePriority = m_orgPriority;
    }

    public override void OnReset()
    {
        m_target = null;
        m_aimSeconds = 4f;
        m_holdSeconds = 2f;
        m_aimSpeed = 2f;
        m_randomRange = 1f;
    }
}