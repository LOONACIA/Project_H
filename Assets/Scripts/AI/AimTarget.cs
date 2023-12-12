using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

// Shooter BT에서 타겟을 조준하는데 사용
[System.Serializable]
public class AimTarget : Action
{
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
    
    private Monster m_owner;
    
    private CapsuleCollider m_collider;
    
    private Vector3 m_previousTargetPosition;

    private float m_timer;
    
    private float m_aimDuration;

    private float m_waitDuration;
    private static readonly int s_isAiming = Animator.StringToHash("IsAiming");

    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
    }

    public override void OnStart()
    {
        if (m_target.Value == null)
        {
            return;
        }
        
        m_owner.Animator.SetBool(s_isAiming, true);

        m_collider = m_target.Value.GetComponent<CapsuleCollider>();
        if (m_previousTargetPosition == Vector3.zero)
        {
            m_previousTargetPosition = (m_target.Value.position + m_collider.center) + (Random.insideUnitSphere * m_randomRange.Value);
        }

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

        if (m_owner.Attack.IsAttacking)
        {
            return TaskStatus.Running;
        }
        
        if (Time.time - m_timer < m_aimDuration)
        {
            Vector3 targetPosition = Vector3.Lerp(m_previousTargetPosition, (m_target.Value.position + m_collider.center), (Time.time - m_timer) / m_aimSpeed.Value);

            Vector3 lookAt = new(targetPosition.x, m_owner.transform.position.y, targetPosition.z);
            m_owner.transform.LookAt(lookAt);
            
            m_previousTargetPosition = targetPosition;
            m_owner.Attack.Target = targetPosition;
            return TaskStatus.Running;
        }
        else if (Time.time - m_timer < m_waitDuration)
        {
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
        m_owner.Animator.SetBool(s_isAiming, false);
        m_previousTargetPosition = Vector3.zero;
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