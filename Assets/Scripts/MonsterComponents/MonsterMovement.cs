using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMovement : MonoBehaviour, INotifyPropertyChanged
{
    public bool isDashing;

    public bool isMoving;
    
    [SerializeField]
    private MonsterMovementData m_data;
    
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
    private bool m_isJumped = false;
    
    private bool m_isJumpApplied = false;
    
    // TODO: 공격 시 이동 속도를 느리게 만들 것인가?
    // 공격 시 이동 속도를 느리게 만듦
    private float m_speedMultiplier = 1f;

    private float m_dashMultiplier = 1f;

    // 현재 이동 애니메이션 보간 비율 (정지 0, 걷기 0.5, 달리기 1)
    private float m_currentMovementRatio;

    // 이동 애니메이션 보간 변화량
    private float m_moveAnimationChangeRatio = 0.01f;

    public MonsterMovementData Data => m_data;

    public bool IsOnGround
    {
        get => m_isOnGround;
        set => SetField(ref m_isOnGround, value);
    }
    
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
        CalculateCurrentNormal();
        CheckJumping();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        ApplyFriction();
        CheckGround();
    }

    public void Move(Vector3 directionInput)
    {
        // Awake에서 호출되는 경우
        if (m_rigidbody is null || m_data is null)
        {
            // 아무 것도 하지 않음
            return;
        }

        //플레이어가 움직이는지 체크
        isMoving = directionInput != Vector3.zero;

        // 대쉬 사용 시 속도 증가
        m_dashMultiplier = isDashing ? m_data.DashMultiplier : 1;
        float speedMultiplier = m_speedMultiplier * m_dashMultiplier;

        if (TranslateBySurfaceNormal(m_rigidbody.velocity, m_currentNormal).magnitude <
            m_data.ThirdMoveSpeedThreshold)
        {
            m_rigidbody.AddRelativeForce(
                directionInput * (m_data.FrictionAcceleration * Time.fixedDeltaTime * speedMultiplier),
                ForceMode.VelocityChange);
        }

        float strafeSpeed = Mathf.Abs(transform.InverseTransformDirection(m_rigidbody.velocity).x);
        if (directionInput.z < 0)
        {
            strafeSpeed += Mathf.Abs(transform.InverseTransformDirection(m_rigidbody.velocity).z);
        }

        Vector3 strafe = new(directionInput.x, directionInput.y, Mathf.Min(0, directionInput.z));
        Vector3 strafeForce = GetForce(strafe, strafeSpeed, m_data.FirstStrafeSpeedThreshold * speedMultiplier,
            m_data.SecondStrafeSpeedThreshold * speedMultiplier, m_data.ThirdStrafeSpeedThreshold * speedMultiplier);

        Vector3 forward = new(0, directionInput.y, Mathf.Max(directionInput.z, 0));
        Vector3 forwardForce = GetForce(forward, m_rigidbody.velocity.magnitude,
            m_data.FirstMoveSpeedThreshold * speedMultiplier, m_data.SecondMoveSpeedThreshold * speedMultiplier,
            m_data.ThirdMoveSpeedThreshold * speedMultiplier);

        m_rigidbody.AddRelativeForce((forwardForce + strafeForce) * speedMultiplier, ForceMode.VelocityChange);
    }

    public void MoveTo(Vector3 destination)
    {
        if (m_agent.enabled)
        {
            m_agent.SetDestination(destination);
            m_agent.speed = m_data.MoveSpeed;
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
        // 현재 움직이고 있는지 확인
        Vector3 navSpeed = new Vector3(m_agent.velocity.x, 0f, m_agent.velocity.z);
        bool moving = navSpeed.sqrMagnitude > 0.2f || isMoving;

        // move blend tree 값 설정 (정지 0, 걷기 0.5, 달리기 1)
        float targetMoveRatio = 0;
        if (isMoving)
            targetMoveRatio = (isDashing ? 1 : 0.5f);

        // 현재 이동 애니메이션 보간
        if (Mathf.Abs(m_currentMovementRatio - targetMoveRatio) > m_moveAnimationChangeRatio)
        {
            m_currentMovementRatio += (m_currentMovementRatio < targetMoveRatio ? m_moveAnimationChangeRatio : -m_moveAnimationChangeRatio);
            m_currentMovementRatio = Mathf.Clamp(m_currentMovementRatio, 0f, 1f);
        }

        // 애니메이터에 적용
        if (m_actor.IsPossessed)
        {
            m_actor.Animator.SetFloat("MovementRatio", m_currentMovementRatio);
            m_actor.Animator.SetBool("Jump", m_isJumpApplied);
        }
        else
        {
            m_actor.Animator.SetFloat("MovementRatio", m_currentMovementRatio);
            m_actor.Animator.SetBool("Jump", !IsOnGround);
        }
    }
}
