using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMovement : MonoBehaviour, INotifyPropertyChanged
{
    private static readonly int s_animatorKnockBack = Animator.StringToHash("KnockBack");

    [SerializeField]
    private MonsterMovementData m_data;

    //대쉬 시작 시간을 저장하기 위한 값
    private float m_lastDashTime;

    private Actor m_actor;

    private Rigidbody m_rigidbody;

    private CapsuleCollider m_collider;

    private NavMeshAgent m_agent;

    private Vector3 m_currentNormal;

    private float m_lastJumpedTime;

    [SerializeField]
    private bool m_isOnGround;

    private float m_jumpVelocity;

    //현재 점프속도가 즉시 적용되지 않음, 애니메이션 적용 시 해당 값을 기다림
    private bool m_isJumped;

    private bool m_isJumpApplied;

    // TODO: 공격 시 이동 속도를 느리게 만들 것인가?
    // 공격 시 이동 속도를 느리게 만듦
    private float m_speedMultiplier = 1f;

    //private float m_dashMultiplier = 1f; //대쉬가 돌진으로 바뀌면서 미사용

    //대쉬 방향
    private Vector3 m_dashDirection = Vector3.zero;

    // 넉백의 최소 지속시간
    private float m_minKnockBackTime = 1f;

    // 마지막 넉백 시간
    private float m_lastKnockBackTime = 0f;

    // 리지드바디 속도가 이것보다 낮아지면 넉백 종료
    private float m_knockBackEndSpeedThreshold = 0.1f;

    public MonsterMovementData Data => m_data;

    public bool IsOnGround
    {
        get => m_isOnGround;
        set => SetField(ref m_isOnGround, value);
    }

    public bool IsDashing { get; private set; } = false;

    public event PropertyChangedEventHandler PropertyChanged;

    private void Start()
    {
        m_actor = GetComponent<Monster>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckJumping();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (m_actor.IsPossessed)
        {
            CalculateCurrentNormal();
        }

        if (!IsDashing)
        {
            ApplyGravity();
            ApplyFriction();
        }
        else
        {
            ApplyDash();
        }

        CheckGround();
        CheckKnockBackEnd();
    }

    public void Move(Vector3 directionInput)
    {
        // Awake에서 호출되는 경우
        if (m_rigidbody is null || m_data is null || m_actor.Status.IsKnockedDown)
        {
            // 아무 것도 하지 않음
            return;
        }

        // 대쉬 사용 시 속도 증가
        //m_dashMultiplier = IsDashing ? m_data.DashMultiplier : 1;
        //float speedMultiplier = m_speedMultiplier * m_dashMultiplier;

        if (TranslateBySurfaceNormal(m_rigidbody.velocity, m_currentNormal).magnitude <
            m_data.ThirdMoveSpeedThreshold)
        {
            m_rigidbody.AddRelativeForce(
                directionInput * (m_data.FrictionAcceleration * Time.fixedDeltaTime * m_speedMultiplier),
                ForceMode.VelocityChange);
        }

        float strafeSpeed = Mathf.Abs(transform.InverseTransformDirection(m_rigidbody.velocity).x);
        if (directionInput.z < 0)
        {
            strafeSpeed += Mathf.Abs(transform.InverseTransformDirection(m_rigidbody.velocity).z);
        }

        Vector3 strafe = new(directionInput.x, directionInput.y, Mathf.Min(0, directionInput.z));
        Vector3 strafeForce = GetForce(strafe, strafeSpeed, m_data.FirstStrafeSpeedThreshold * m_speedMultiplier,
            m_data.SecondStrafeSpeedThreshold * m_speedMultiplier, m_data.ThirdStrafeSpeedThreshold * m_speedMultiplier);

        Vector3 forward = new(0, directionInput.y, Mathf.Max(directionInput.z, 0));
        Vector3 forwardForce = GetForce(forward, m_rigidbody.velocity.magnitude,
            m_data.FirstMoveSpeedThreshold * m_speedMultiplier, m_data.SecondMoveSpeedThreshold * m_speedMultiplier,
            m_data.ThirdMoveSpeedThreshold * m_speedMultiplier);

        m_rigidbody.AddRelativeForce((forwardForce + strafeForce) * m_speedMultiplier, ForceMode.VelocityChange);
    }

    public void MoveTo(Vector3 destination)
    {
        if (m_agent.enabled)
        {
            m_agent.isStopped = false;
            m_agent.SetDestination(destination);

            if (!IsOnGround)
            {
                // 몬스터가 떨어지는 경우 중력 영향을 받음
                m_agent.speed += Physics.gravity.y * Time.deltaTime * -1;
                m_agent.speed = Mathf.Clamp(m_agent.speed, 0, 30);
            }
            else
            {
                m_agent.speed = m_data.MoveSpeed;
            }
        }
    }

    public void TryJump()
    {
        // 지상에 있지 않은 경우
        if (!IsOnGround)
        {
            // 아무 것도 하지 않음
            return;
        }

        m_jumpVelocity = m_data.JumpHeight / m_data.ZeroGravityDuration;
        m_lastJumpedTime = Time.time;

        // velocity가 원래 몇이든 m_jumpVelocity로 만드는 효과
        m_rigidbody.AddForce(Vector3.up * (m_jumpVelocity - m_rigidbody.velocity.y), ForceMode.VelocityChange);

        //점프 애니메이션 체크용 변수 true
        m_isJumped = true;
    }

    public void TryDash(Vector3 direction)
    {
        //대쉬 중인 경우 다른 물리 이동(move, jump, gravity)등을 무시하고 해당 위치까지 이동합니다.
        //TryDash에서는 대쉬 시작 명령을 내리고 방향을 정하며, 실제 대쉬 연산은 매 FixedUpdate의 ApplyDash에서 일어납니다.

        //쿨타임이 끝났는지 체크한다.
        if (m_lastDashTime + m_data.DashCoolTime > Time.time)
        {
            return;
        }

        //대쉬를 시도함.
        if(direction == Vector3.zero)
        {
            m_dashDirection = transform.forward;
        }
        else
        {
            m_dashDirection = transform.TransformDirection(direction.normalized);
        }
        m_lastDashTime = Time.time;
        IsDashing = true;
        gameObject.layer = m_data.DashLayer;
    }

    public void TryKnockBack(Vector3 direction, float power, bool overwrite = true)
    {
        //넉백값 변경
        m_actor.Status.IsKnockBack = true;
        m_lastKnockBackTime = Time.time;
        m_actor.Animator.SetBool(s_animatorKnockBack, true);

        m_agent.enabled = false;
        m_rigidbody.isKinematic = false;

        if (overwrite)
        {
            m_rigidbody.velocity = new Vector3();
        }

        m_rigidbody.AddForce(direction * power, ForceMode.Impulse);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    private static Vector3 TranslateBySurfaceNormal(Vector3 originalVector, Vector3 surfaceNormal)
    {
        return Vector3.ProjectOnPlane(originalVector, surfaceNormal);
    }

    private void ApplyGravity()
    {
        if (Time.time - m_lastJumpedTime < m_data.ZeroGravityDuration && m_rigidbody.velocity.y > 0f)
        {
            // 점프 직후에는 중력을 적용하지 않음
        }
        else if (IsOnGround)
        {
            m_rigidbody.AddForce(Vector3.up * (m_data.Gravity * Time.fixedDeltaTime),
                ForceMode.VelocityChange);
        }
        else
        {
            // 점프 중력 적용
            float jumpGravity = -2f * m_data.JumpHeight /
                                (m_data.FallTime * m_data.FallTime);
            m_rigidbody.AddForce(Vector3.up * (jumpGravity * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
    }

    private void CheckGround()
    {
        float radius = m_collider.radius;
        IsOnGround = Physics.SphereCast(transform.position + m_collider.center, radius, Vector3.down, out _,
            radius + 0.2f, m_data.WhatIsGround);
    }

    private void ApplyDash()
    {
        if (m_lastDashTime + m_data.DashDuration <= Time.time)
        {
            //대쉬 시간이 지났다면 대쉬 종료
            IsDashing = false;
            m_dashDirection = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("Monster");
            return;
        }

        //1. 대쉬 방향을 지정. 땅위라면 땅과 평행, 땅 위가 아니라면 xz 평면과 평행
        m_dashDirection = TranslateBySurfaceNormal(m_dashDirection, m_currentNormal).normalized;

        //2. 캡슐캐스트 진행, 벽이나 땅을 만나는지 체크합니다. 이미 충돌중인 상태인 벽이나 땅은 체크하지 않기 때문에, 실제 콜라이더보다 약간 작은 radius로 검출합니다.
        Vector3 p1 = m_collider.center + Vector3.up * (0.5f * m_collider.height - m_collider.radius);
        Vector3 p2 = m_collider.center + Vector3.down * (0.5f * m_collider.height - m_collider.radius);
        float speed = m_data.DashAmount / m_data.DashDuration;
        int l = LayerMask.GetMask("Ground", "Wall");
        if (Physics.CapsuleCast(transform.TransformPoint(p1), transform.TransformPoint(p2), m_collider.radius-0.01f, m_dashDirection, out var hit, speed*Time.fixedDeltaTime, l))
        {
            //벽이나 땅을 만난다면, 해당 위치까지만 이동합니다.
            m_rigidbody.MovePosition(transform.position + m_dashDirection * hit.distance);
        }
        else
        {
            //아무것도 만나지 않는다면, 지정된 위치까지 이동합니다.
            m_rigidbody.MovePosition(transform.position + m_dashDirection * speed * Time.fixedDeltaTime);
        }
    }

    private void CheckKnockBackEnd()
    {
        if (!m_actor.Status.IsKnockBack) return;

        //최소 넉백시간이 지나지 않았다면 return;
        if (m_lastKnockBackTime + m_minKnockBackTime > Time.time) return;

        float sqrThreshold = m_knockBackEndSpeedThreshold * m_knockBackEndSpeedThreshold;
        if (m_rigidbody.velocity.sqrMagnitude <= sqrThreshold)
        {
            //속도가 최소보다 작아졌으므로 KnockBack 종료, 관련 변수 초기화
            //AI라면 Agent를 켜줍니다.
            //TODO: AI 점프 적용 시 점프까지 같이 체크해주어야함.
            if (!m_actor.IsPossessed)
            {
                m_agent.enabled = !m_actor.IsPossessed;
                m_rigidbody.isKinematic = true;
            }

            m_actor.Status.IsKnockBack = false;
            m_actor.Animator.SetBool(s_animatorKnockBack, false);
        }
    }

    private void ApplyFriction()
    {
        Vector3 frictionDirection = -m_rigidbody.velocity.GetFlatVector().normalized *
                                    (Time.fixedDeltaTime * m_data.FrictionAcceleration);
        if (Mathf.Abs(m_rigidbody.velocity.x) - Mathf.Abs(frictionDirection.x) < 0)
        {
            frictionDirection.x = -m_rigidbody.velocity.x;
        }

        if (Mathf.Abs(m_rigidbody.velocity.z) - Mathf.Abs(frictionDirection.z) < 0)
        {
            frictionDirection.z = -m_rigidbody.velocity.z;
        }

        if (m_rigidbody.velocity.y > 0)
        {
            frictionDirection = TranslateBySurfaceNormal(frictionDirection, m_currentNormal);
        }

        m_rigidbody.AddForce(frictionDirection, ForceMode.VelocityChange);
    }

    private void CalculateCurrentNormal()
    {
        float radius = m_collider.radius;
        m_currentNormal = Physics.SphereCast(transform.position + m_collider.center, radius, Vector3.down, out var hit,
            radius + 0.05f, m_data.WhatIsGround)
            ? hit.normal
            : Vector3.up;
    }

    // TODO: ApplyGravity로 마이그레이션
    /// <summary>
    /// 실제 점프가 이루어지고 있는지 확인. 애니메이션 적용
    /// </summary>
    private void CheckJumping()
    {
        //점프키가 눌렸고 속도가 0보다 크다면 진짜 점프중
        if (m_isJumped && m_rigidbody.velocity.y > 0)
        {
            m_isJumpApplied = true;
        }

        //실제 점프 중이면서, 속도는 0 이하이고, 땅에 닿았다면 점프 끝
        if (m_isJumpApplied && m_rigidbody.velocity.y <= 0 && IsOnGround)
        {
            m_isJumped = false;
            m_isJumpApplied = false;
        }
    }

    private Vector3 GetForce(Vector3 directionInput, float currentSpeed, float firstThreshold, float secondThreshold,
        float thirdThreshold)
    {
        float lowerSpeedThreshold = 0;
        float higherSpeedThreshold = 0;
        float accelerationTime = Mathf.Infinity;

        if (currentSpeed < firstThreshold)
        {
            lowerSpeedThreshold = 0f;
            higherSpeedThreshold = firstThreshold;
            accelerationTime = m_data.FirstAccelerationTime;
        }
        else if (currentSpeed < secondThreshold)
        {
            lowerSpeedThreshold = firstThreshold;
            higherSpeedThreshold = secondThreshold;
            accelerationTime = m_data.SecondAccelerationTime;
        }
        else if (currentSpeed < thirdThreshold)
        {
            lowerSpeedThreshold = secondThreshold;
            higherSpeedThreshold = thirdThreshold;
            accelerationTime = m_data.ThirdAccelerationTime;
        }

        float deltaAcceleration = (higherSpeedThreshold - lowerSpeedThreshold) / accelerationTime * Time.fixedDeltaTime;
        Vector3 translatedDirectionBySlope =
            TranslateBySurfaceNormal(transform.TransformDirection(directionInput), m_currentNormal);
        translatedDirectionBySlope = transform.InverseTransformDirection(translatedDirectionBySlope);

        return translatedDirectionBySlope * deltaAcceleration;
    }

    private void OnValidate()
    {
        if (m_data == null)
        {
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
        }
    }

    private void UpdateAnimator()
    {
        bool jump = m_actor.IsPossessed ? m_isJumpApplied : !m_isOnGround;
        m_actor.Animator.SetBool("Jump", jump);
    }
}