using LOONACIA.Unity;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMovement : MonoBehaviour
{    
    private readonly Collider[] m_avoidanceCheckColliders = new Collider[3];

    [SerializeField]
    private MonsterMovementData m_data;

    private Actor m_actor;

    private Rigidbody m_rigidbody;

    private CapsuleCollider m_collider;

    private NavMeshAgent m_agent;

    // 이동 관련
    private float m_movementRatio;

    // MoveTo함수에서 계산되는 속도값.
    private float m_currentMoveToSpeed;

    // MoveTo함수에서 계산되는 가속도값
    private float m_currentMoveToAccel = 1f;

    private NavMeshPath m_lastPath;

    private Vector3 m_currentNormal;

    private float m_lastJumpedTime;

    [SerializeField]
    private bool m_isOnGround;

    private float m_jumpVelocity;

    // 현재 점프속도가 즉시 적용되지 않음, 애니메이션 적용 시 해당 값을 기다림
    private bool m_isJumped;

    private bool m_isJumpApplied;

    // 대쉬 방향
    private Vector3 m_dashDirection = Vector3.zero;

    // 대쉬 속도 (나눗셈 연산은 느리기 때문에 밖으로 분리함)
    private float m_dashSpeed;

    // 마지막으로 대쉬를 누른 시간
    private float m_lastDashTime;

    // 넉백의 최소 지속시간
    private float m_minKnockBackTime = 1f;

    // 마지막 넉백 시간
    private float m_lastKnockBackTime;

    // 걷는 사운드 출력 체크
    private bool m_isWalkSoundPlaying = false;

    private AudioSource m_audioSource;

    public MonsterMovementData Data => m_data;

    public float MovementRatio => m_movementRatio;

    public bool IsOnGround
    {
        get => m_isOnGround;
        private set => m_isOnGround = value;
    }

    public bool IsDashing { get; private set; }
    
    private void Start()
    {
        m_actor = GetComponent<Monster>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        m_agent = GetComponent<NavMeshAgent>();
        m_audioSource = GetComponent<AudioSource>();    

        m_actor.Status.CurrentDashCount = m_actor.Status.MaxDashCount = m_data.MaxDashCount;
        m_actor.Status.DashCoolTime = m_data.DashCoolTime;
        m_currentMoveToAccel = m_data.AccelerationTime > 0f ? 1f / m_data.AccelerationTime : float.PositiveInfinity;
        m_lastPath = new(); //네브매쉬패스는 Start, Awake에서 초기화되어야함.
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
        CheckCanUseNavMesh();

        //사운드 관련
        CheckWalk();
    }

    public void Move(Vector3 directionInput)
    {
        // Awake에서 호출되는 경우
        if (m_rigidbody is null || m_data is null || m_actor.Status.IsKnockedDown || IsDashing)
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
                directionInput * (m_data.FrictionAcceleration * Time.fixedDeltaTime),
                ForceMode.VelocityChange);
        }

        float strafeSpeed = Mathf.Abs(transform.InverseTransformDirection(m_rigidbody.velocity).x);
        if (directionInput.z < 0)
        {
            strafeSpeed += Mathf.Abs(transform.InverseTransformDirection(m_rigidbody.velocity).z);
        }

        Vector3 strafe = new(directionInput.x, directionInput.y, Mathf.Min(0, directionInput.z));
        Vector3 strafeForce = GetForce(strafe, strafeSpeed, m_data.FirstStrafeSpeedThreshold,
            m_data.SecondStrafeSpeedThreshold, m_data.ThirdStrafeSpeedThreshold);

        Vector3 forward = new(0, directionInput.y, Mathf.Max(directionInput.z, 0));
        Vector3 forwardForce = GetForce(forward, m_rigidbody.velocity.magnitude,
            m_data.FirstMoveSpeedThreshold, m_data.SecondMoveSpeedThreshold,
            m_data.ThirdMoveSpeedThreshold);

        m_rigidbody.AddRelativeForce((forwardForce + strafeForce), ForceMode.VelocityChange);

        // 사운드
        if(directionInput != default)
            PlayWalkSound();
    }

    public void MoveTo(Vector3 destination)
    {
        //3인칭인 경우 Avoidance값을 높임
        m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        //MoveToWithNavMove(destination);
        MoveToWithNavSetDest(destination, true);
        //MoveToWithNavSetPath(destination, true);

        //사운드
        PlayWalkSound();
    }

    #region LegacyMoveToFunctions
    /// <summary>
    /// NavMesh의 Move를 통한 이동을 구현
    /// 현재 사용하지 않으나, 후에 다른 로직 구현에 사용될 수 있는 코드가 있어 남겨둠
    /// </summary>
    /// <param name="destination"></param>
    private void MoveToWithNavMove(Vector3 destination)
    {
        if (m_agent.enabled)
        {
            //1. Path상 다음 목적지를 찾는다.
            if (!NavMesh.CalculatePath(transform.position, destination, m_agent.areaMask, m_lastPath))
            {
                //만약 Invalid한 위치(공중 or Navmesh가 없는 곳)에 있다면, 반경 10f 안에 가장 가까운 NavMesh위치로 이동합니다.
                if (!(NavMesh.SamplePosition(destination, out var hit, float.PositiveInfinity, m_agent.areaMask)
                    && NavMesh.CalculatePath(transform.position, hit.position, m_agent.areaMask, m_lastPath)))
                {
                    //반경 10f 안에 가장 가까운 NavMesh가 없다면, return합니다.
                    Debug.LogWarning($"{gameObject.name}: destination에 인접한 NavMesh가 없어 길찾기 실패.");
                    return;
                }
            }

            Vector3 navDest = m_lastPath.corners.Length > 1 ? m_lastPath.corners[1] : m_lastPath.corners[0];

            //2. 목표 지점을 향한 방향 탐색
            Vector3 dir = (navDest - transform.position).normalized;

            //2.1. 주변 적들과의 거리를 계산하여, 가중치를 부여함 (boids 알고리즘의 separation)
            // - 굳이 없어도 자연스러운 것 같아 구현하지 않음.
            // - ObstacleAvoidance가 None이 아니면, Move하되 충돌검사는 해주는 것 같음

            //3.1. Rotate
            //각속도 적용
            float angle = Vector3.SignedAngle(transform.forward.GetFlatVector(), dir.GetFlatVector(), Vector3.up);
            if (Mathf.Abs(angle) > Mathf.Abs(Data.AngularSpeed * Time.deltaTime))
            {
                if (angle >= 0)
                    angle = Data.AngularSpeed * Time.deltaTime;
                else
                    angle = -Data.AngularSpeed * Time.deltaTime;
            }
            Vector3 nextDir = Quaternion.Euler(0f, angle, 0f) * transform.forward.GetFlatVector();

            //Vector3 nextDir = Vector3.Lerp(transform.forward.GetFlatVector(), dir.GetFlatVector(), Time.deltaTime * rotSpeed);
            transform.rotation = Quaternion.LookRotation(nextDir);

            //3.2. 이동
            m_currentMoveToSpeed += m_currentMoveToAccel * Data.MoveSpeed * Time.deltaTime;
            if (m_currentMoveToSpeed > Data.MoveSpeed)
                m_currentMoveToSpeed = Data.MoveSpeed;
            m_agent.Move(nextDir * Time.deltaTime * m_currentMoveToSpeed);

            //4. 애니메이션 적용
            m_movementRatio = 1f;

            if (!IsOnGround)
            {
                // 몬스터가 떨어지는 경우 중력 영향을 받음
                m_agent.speed += Physics.gravity.y * Time.deltaTime * -1;
                m_agent.speed = Mathf.Clamp(m_agent.speed, 0, 30);
            }
            else
            {
                m_agent.speed = m_data.MoveSpeed;
                m_agent.angularSpeed = m_data.AngularSpeed;
            }

            m_agent.autoTraverseOffMeshLink = false;
            if (m_agent.isOnOffMeshLink)
            {
                m_agent.CompleteOffMeshLink();
                Debug.Log($"{gameObject.name}: OffMeshLink에잇슴");
            }
        }
    }

    /// <summary>
    /// SetPath를 통한 이동을 구현
    /// 현재 사용하지 않으나, 후에 다른 로직 구현에 사용될 수 있는 코드가 있어 남겨둠
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="useAdaptiveAvoidanceTest"></param>
    private void MoveToWithNavSetPath(Vector3 destination, bool useAdaptiveAvoidanceTest = true)
    {
        if (m_agent.enabled)
        {
            m_agent.isStopped = false;

            //1. Path상 다음 목적지를 찾는다.
            if (!NavMesh.CalculatePath(transform.position, destination, m_agent.areaMask, m_lastPath))
            {
                //만약 Invalid한 위치(공중 or Navmesh가 없는 곳)에 있다면, 가장 가까운 NavMesh위치로 이동합니다.
                if (!(NavMesh.SamplePosition(destination, out var hit, float.PositiveInfinity, m_agent.areaMask)
                    && NavMesh.CalculatePath(transform.position, hit.position, m_agent.areaMask, m_lastPath)))
                {
                    //가장 가까운 NavMesh가 없다면, return합니다.
                    Debug.LogWarning($"{gameObject.name}: destination에 인접한 NavMesh가 없어 길찾기 실패.");
                    return;
                }
            }

            //주변에 다른 대상이 있는지 찾는다.
            if (useAdaptiveAvoidanceTest)
            {
                int amount = Physics.OverlapSphereNonAlloc(transform.position + m_collider.center, m_agent.radius + m_agent.velocity.magnitude * Time.deltaTime * 2f, m_avoidanceCheckColliders, LayerMask.GetMask("Monster"));
                //자기 자신이 포함되었는지 확인
                int max = amount < m_avoidanceCheckColliders.Length ? amount : m_avoidanceCheckColliders.Length;
                for (int i = 0; i < max; i++)
                {
                    if (m_avoidanceCheckColliders[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
                    {
                        //자기 자신일 경우 패스
                        max -= 1;
                        break;
                    }
                }
                if (max > 0)
                {
                    //자기 자신을 제외하고 누군가 범위 내에 있다면, Quality를 높인다.
                    m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                }
                else
                {
                    //아무도 없다면 None으로 진행(다른 Agent, Corner 무시)
                    m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                }
            }

            m_agent.SetPath(m_lastPath);

            if (!IsOnGround)
            {
                // 몬스터가 떨어지는 경우 중력 영향을 받음
                float expectedSpeed = m_agent.speed + Physics.gravity.y * Time.deltaTime * -1;
                m_agent.speed = Mathf.Clamp(expectedSpeed, 0, 30);
            }
            else
            {
                m_agent.speed = m_data.MoveSpeed;
            }
        }
    }
    #endregion

    private void MoveToWithNavSetDest(Vector3 destination, bool useAdaptiveAvoidanceTest = true)
    {
        if (m_agent.enabled)
        {
            m_agent.isStopped = false;

            if (useAdaptiveAvoidanceTest)
            {
                int amount = Physics.OverlapSphereNonAlloc(transform.position + m_collider.center, m_agent.radius + m_agent.velocity.magnitude * Time.deltaTime * 2f, m_avoidanceCheckColliders, LayerMask.GetMask("Monster"));
                //자기 자신이 포함되었는지 확인
                int max = amount < m_avoidanceCheckColliders.Length ? amount : m_avoidanceCheckColliders.Length;
                for (int i = 0; i < max; i++)
                {
                    if (m_avoidanceCheckColliders[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
                    {
                        //자기 자신일 경우 패스
                        max -= 1;
                        break;
                    }
                }
                if (max > 0)
                {
                    //자기 자신을 제외하고 누군가 범위 내에 있다면, Quality를 높인다.
                    m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                }
                else
                {
                    //아무도 없다면 None으로 진행(다른 Agent, Corner 무시)
                    m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                }
            }

            m_agent.SetDestination(destination);

            if (!IsOnGround)
            {
                // 몬스터가 떨어지는 경우 중력 영향을 받음
                float expectedSpeed = m_agent.speed + Physics.gravity.y * Time.deltaTime * -1;
                m_agent.speed = Mathf.Clamp(expectedSpeed, 0, 30);
            }
            else
            {
                m_agent.speed = m_data.MoveSpeed;
            }
        }
    }

    /// <summary>
    /// NavMeshAgent의 움직임을 멈춥니다.
    /// </summary>
    public void StopAgentMove()
    {
        if (m_agent.enabled)
        {
            m_agent.isStopped = true;
            m_agent.ResetPath();
            m_agent.velocity = Vector3.zero;

            //애니메이션 값 0
            m_movementRatio = 0f;

            //m_agent.updatePosition = false;
            //m_agent.updateRotation = false;
        }
    }

    public void TryJump()
    {
        // 지상에 있지 않은 경우 또는 CanJump가 false라면
        if (!IsOnGround || !m_data.CanJump)
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

        //점프 사운드 출력
        gameObject.FindChild<MonsterSFXPlayer>().OnPlayJump();
    }

    public void TryDash(Vector3 direction)
    {
        //대쉬 중인 경우 다른 물리 이동(move, jump, gravity)등을 무시하고 해당 위치까지 이동합니다.
        //TryDash에서는 대쉬 시작 명령을 내리고 방향을 정하며, 실제 대쉬 연산은 매 FixedUpdate의 ApplyDash에서 일어납니다.

        //이미 대쉬 중이거나, 대쉬 딜레이 중이거나, 남은 대쉬가 없다면 대쉬 불가능
        if (IsDashing || m_actor.Status.CurrentDashCount <= 0 || m_actor.Status.IsKnockedDown || direction == Vector3.zero)
        {
            GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.DashUnable);
            return;
        }

        m_actor.Status.CurrentDashCount -= 1;

        //대쉬를 시도함.
        //1. 속도값을 지정 (나눗셈 연산을 줄이기 위해 한번만 수행)
        if (m_data.DashDuration == 0)
        {
            Debug.LogWarning("대쉬한 오브젝트의 대쉬 지속시간이 0입니다.");
            m_dashSpeed = 0f;
        }
        else
        {
            m_dashSpeed = m_data.DashAmount / m_data.DashDuration;
        }

        //2. 대쉬의 초기 방향을 지정
        if (direction == Vector3.zero)
        {
            m_dashDirection = transform.forward;
        }
        else
        {
            m_dashDirection = transform.TransformDirection(direction.normalized);
        }

        m_lastDashTime = Time.time;
        IsDashing = true;

        GameManager.Effect.ShowDashEffect();
        gameObject.FindChild<MonsterSFXPlayer>().OnPlayDash();
    }

    public void TryKnockBack(Vector3 direction, float power, bool overwrite = true)
    {
        //넉백값 변경
        m_actor.Status.IsKnockBack = true;
        m_lastKnockBackTime = Time.time;
        m_actor.Animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_KNOCKBACK, true);

        m_agent.enabled = false;
        m_rigidbody.isKinematic = false;

        if (overwrite)
        {
            m_rigidbody.velocity = new Vector3();
        }

        m_rigidbody.AddForce(direction * power, ForceMode.Impulse);
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
            m_rigidbody.AddForce(Vector3.up * (m_data.Gravity * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
        else
        {
            // 점프 중력 적용
            float jumpGravity = -2f * m_data.JumpHeight / (m_data.FallTime * m_data.FallTime);
            m_rigidbody.AddForce(Vector3.up * (jumpGravity * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
    }

    private void CheckGround()
    {
        float radius = m_collider.radius;
        IsOnGround = Physics.SphereCast(transform.position + m_collider.center, radius, Vector3.down, out var ground,
            radius + 0.2f, m_data.WhatIsGround);
        if (IsOnGround)
        {
            // 착지시 IsFlying 상태이상 초기화
            m_actor.Status.IsFlying = false;
        }
        else
        {
            if (m_rigidbody.velocity.y < 0)
                m_isJumped = false;

            if (m_isJumped)
                m_isJumpApplied = true;
        }
    }

    private void ApplyDash()
    {
        if (m_lastDashTime + m_data.DashDuration <= Time.time
            || m_actor.Status.IsKnockedDown)
        {
            //대쉬 시간이 지났다면 대쉬 종료
            IsDashing = false;
            m_dashDirection = Vector3.zero;
            m_rigidbody.velocity = Vector3.zero;
            return;
        }
        float nextSpeed = m_dashSpeed;

        //대쉬시 모든 물리 영향을 초기화함
        m_rigidbody.velocity = Vector3.zero;

        //1. 대쉬 방향을 지정. 땅위라면 땅과 평행, 땅 위가 아니라면 xz 평면과 평행
        Vector3 nextDirection = TranslateBySurfaceNormal(m_dashDirection, m_currentNormal).normalized;

        //2. 캡슐캐스트 진행, 몬스터와 만나는지 체크합니다.
        Vector3 p1 = transform.TransformPoint(m_collider.center + Vector3.up * (0.5f * m_collider.height - m_collider.radius));
        Vector3 p2 = transform.TransformPoint(m_collider.center + Vector3.down * (0.5f * m_collider.height - m_collider.radius));

        if (Physics.CapsuleCast(p1, p2, m_collider.radius - 0.01f, m_dashDirection, out var hit,
                m_dashSpeed * Time.fixedDeltaTime, LayerMask.GetMask(ConstVariables.MOVEMENT_COLLISION_LAYERS)))
        {
            //부딪힐 대상이 있다면, 해당 위치까지만 이동.
            m_rigidbody.MovePosition(transform.position + m_dashDirection * hit.distance);

            if (hit.transform.gameObject.layer != LayerMask.NameToLayer(ConstVariables.LAYER_MONSTER))
            {
                //몬스터가 아닌 대상과 충돌 시 이동 방향을 바꿔서 한번 더 테스트
                Vector3 nextDir = Vector3.ProjectOnPlane(m_dashDirection, hit.normal);
                Vector3 nextPos = transform.position + m_dashDirection * hit.distance;
                float nextDist = m_dashSpeed * Time.fixedDeltaTime - hit.distance;
                Vector3 np1 = nextPos + (m_collider.center + Vector3.up * (0.5f * m_collider.height - m_collider.radius));
                Vector3 np2 = nextPos + (m_collider.center + Vector3.down * (0.5f * m_collider.height - m_collider.radius));

                if (Physics.CapsuleCast(np1, np2, m_collider.radius - 0.01f, nextDir, out var nhit,
                nextDist, LayerMask.GetMask(ConstVariables.MOVEMENT_COLLISION_LAYERS)))
                {
                    //그래도 충돌할 경우 거기까지만 이동.
                    m_rigidbody.MovePosition(nextPos + nextDir * nhit.distance);
                }
                else
                {
                    //충돌 대상이 없다면 최대 사거리까지 이동
                    m_rigidbody.MovePosition(nextPos + nextDir * nextDist);
                }
            }
            return;
        }


        //강제로 속도를 대쉬값으로 만듭니다.
        m_rigidbody.AddForce(nextDirection * nextSpeed, ForceMode.VelocityChange);
    }

    private void CheckKnockBackEnd()
    {
        //넉백이 아니라면 처리하지 않음
        if (!m_actor.Status.IsKnockBack) return;

        //넉백 종료조건: 정해진 최소 시간 경과&IsOnGround
        if (m_lastKnockBackTime + m_minKnockBackTime > Time.time) return;
        if (!IsOnGround) return;

        //23.12.18 NavMeshAgent는 CheckCanUseNavMesh에서 처리
        m_actor.Status.IsKnockBack = false;
        m_actor.Animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_KNOCKBACK, false);
    }

    private void CheckCanUseNavMesh()
    {
        //해킹된 상태가 아니면서, 땅위에 있으면서, 아무 상태이상도 아닌 경우에만 true
        if (!m_actor.IsPossessed && !m_agent.enabled && IsOnGround)
        {
            //NavMesh가 켜지면 안되는 상태이상에 대한 처리
            //TODO: 넉백을 시간 대신, 물리 값으로 처리할지에 대한 여부
            if (m_actor.Status.IsKnockBack) return;

            m_agent.enabled = true;
            m_rigidbody.isKinematic = true;
        }
    }

    private void ApplyFriction()
    {
        if (m_actor.Status.IsFlying) return;

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
        //if (m_isJumped && m_rigidbody.velocity.y > 0)
        //{
        //    m_isJumpApplied = true;
        //}

        //실제 점프 중이면서, 속도는 0 이하이고, 땅에 닿았다면 점프 끝
        if (m_isJumpApplied && /*m_rigidbody.velocity.y <= 0 &&*/ IsOnGround)
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
            Debug.LogWarning($"{name}: {nameof(m_data)} is null", gameObject);
        }
    }

    private void UpdateAnimator()
    {
        //bool jump = m_actor.IsPossessed ? m_isJumpApplied : !m_isOnGround;
        bool jump = m_actor.IsPossessed ? !m_isOnGround : !m_isOnGround;
        m_actor.Animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_JUMP, jump);
    }


    #region 사운드 관련
    private void PlayWalkSound()
    {
        //땅 위가 아니면 return
        if (!IsOnGround)
        {
            m_isWalkSoundPlaying = false;
            m_audioSource.Stop();
            return;
        }
            

        //걷는 사운드가 이미 출력되고 있으면 return
        if (m_isWalkSoundPlaying)
            return;

        m_audioSource.Play();
        m_isWalkSoundPlaying = true;
    }

    private void CheckWalk()
    {
        if (m_actor.Animator.GetFloat(ConstVariables.ANIMATOR_PARAMETER_MOVEMENT_RATIO) >= 0.1f)
            return;

        m_isWalkSoundPlaying = false;
        m_audioSource.Stop();
    }
    #endregion
}