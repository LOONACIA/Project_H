using System.Buffers;
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
    private static readonly int s_animatorKnockBack = Animator.StringToHash("KnockBack");

    [SerializeField]
    private MonsterMovementData m_data;

    private Actor m_actor;

    private Rigidbody m_rigidbody;

    private CapsuleCollider m_collider;

    private NavMeshAgent m_agent;

    //이동 관련
    private float m_movementRatio;

    //MoveTo함수에서 계산되는 속도값.
    private float m_currentMoveToSpeed = 0f;

    //MoveTo함수에서 계산되는 가속도값
    private float m_currentMoveToAccel = 1f;

    private NavMeshPath m_lastPath;

    private Vector3 m_currentNormal;

    private float m_lastJumpedTime;

    [SerializeField]
    private bool m_isOnGround;

    private GameObject m_standingGround;

    private float m_jumpVelocity;

    //현재 점프속도가 즉시 적용되지 않음, 애니메이션 적용 시 해당 값을 기다림
    private bool m_isJumped;

    private bool m_isJumpApplied;

    //private float m_dashMultiplier = 1f; //대쉬가 돌진으로 바뀌면서 미사용

    //대쉬 방향
    private Vector3 m_dashDirection = Vector3.zero;

    private Vector3 m_lastDashVelocity = Vector3.zero;

    //대쉬 속도 (나눗셈 연산은 느리기 때문에 밖으로 분리함)
    private float m_dashSpeed = 0f;

    //마지막으로 대쉬를 누른 시간
    private float m_lastDashTime;

    //대쉬 쿨타임이 체크되기 시작된 시간
    private float m_dashCooldownStartTime;

    //남은 대쉬 수
    private int m_dashCount;

    // 넉백의 최소 지속시간
    private float m_minKnockBackTime = 1f;

    // 마지막 넉백 시간
    private float m_lastKnockBackTime;

    public MonsterMovementData Data => m_data;

    public float MovementRatio => m_movementRatio;

    public bool IsOnGround
    {
        get => m_isOnGround;
        set => SetField(ref m_isOnGround, value);
    }

    public bool IsDashing { get; private set; }

    public float CurrentDashCoolDown
    {
        get
        {
            if (m_dashCount == m_data.MaxDashCount)
            {
                return 1;
            }

            if (m_data.DashCoolTime <= 0)
            {
                return 1;
            }
            
            return Mathf.Clamp((Time.time - m_dashCooldownStartTime) / m_data.DashCoolTime, 0f, 1f);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Start()
    {
        m_actor = GetComponent<Monster>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        m_agent = GetComponent<NavMeshAgent>();

        m_dashCount = m_data.MaxDashCount;
        m_currentMoveToAccel = (m_data.AccelerationTime > 0f ? (1f / m_data.AccelerationTime) : (float.PositiveInfinity));
        m_lastPath = new NavMeshPath(); //네브매쉬패스는 Start, Awake에서 초기화되어야함.
    }

    private void Update()
    {
        CheckJumping();

        UpdateAnimator();
        UpdateDashCoolDown();
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
    }

    public void MoveTo(Vector3 destination)
    {
        //3인칭인 경우 Avoidance값을 높임
        m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        //MoveToWithNavMove(destination);
        MoveToWithNavSetDest(destination,true);
    }

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
            Debug.Log($"속도: {m_currentMoveToSpeed}, 가속도{m_currentMoveToAccel}, 걸리는 시간{m_data.AccelerationTime}");
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

    Collider[] m_avoidanceTest = new Collider[3];

    private void MoveToWithNavSetDest(Vector3 destination, bool useAdaptiveAvoidanceTest = true)
    {
        if (m_agent.enabled)
        {
            m_agent.isStopped = false;

            if (useAdaptiveAvoidanceTest)
            {
                Physics.OverlapSphere(transform.position + m_collider.center, m_agent.radius + m_agent.velocity.magnitude * Time.deltaTime * 2f);
                int amount = Physics.OverlapSphereNonAlloc(transform.position + m_collider.center, m_agent.radius + m_agent.velocity.magnitude * Time.deltaTime * 2f,m_avoidanceTest,LayerMask.GetMask("Monster"));
                //자기 자신이 포함되었는지 확인
                int max = amount<m_avoidanceTest.Length?amount:m_avoidanceTest.Length;
                for(int i = 0; i < max; i++)
                {
                    if (m_avoidanceTest[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
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
                m_agent.speed += Physics.gravity.y * Time.deltaTime * -1;
                m_agent.speed = Mathf.Clamp(m_agent.speed, 0, 30);
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

        //이미 대쉬 중이거나, 대쉬 딜레이 중이거나, 남은 대쉬가 없다면 대쉬 불가능
        if (IsDashing || m_lastDashTime + m_data.DashDelay > Time.time || m_dashCount <= 0 || m_actor.Status.IsKnockedDown)
        {
            if (m_dashCount <= 0)
                Debug.Log("남은 대쉬 0, 차지까지 남은 시간: " + (m_dashCooldownStartTime + m_data.DashCoolTime - Time.time));
            if (IsDashing) Debug.Log("이미 대쉬 중");
            if (m_lastDashTime + m_data.DashDelay > Time.time) Debug.Log("대쉬 딜레이중");
            return;
        }

        //만약 대쉬가 꽉 차있었는데 대쉬를 시도했다면, 쿨타임을 세기 시작함.
        if (m_dashCount == m_data.MaxDashCount)
        {
            m_dashCooldownStartTime = Time.time;
        }

        m_dashCount -= 1;

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
        //gameObject.layer = m_data.DashLayer;

        GameManager.Effect.ShowDashEffect();
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
            m_standingGround = ground.collider.gameObject;

            // 착지시 IsFlying 상태이상 초기화
            m_actor.Status.IsFlying = false;
        }
        else
        {
            m_standingGround = null;

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
            m_lastDashVelocity = Vector3.zero;
            m_rigidbody.velocity = Vector3.zero;
            //gameObject.layer = LayerMask.NameToLayer("Monster");
            return;
        }

        //대쉬시 모든 물리 영향을 초기화함
        m_rigidbody.velocity = Vector3.zero;

        //1. 대쉬 방향을 지정. 땅위라면 땅과 평행, 땅 위가 아니라면 xz 평면과 평행
        m_dashDirection = TranslateBySurfaceNormal(m_dashDirection, m_currentNormal).normalized;

        //2. 캡슐캐스트 진행, 벽이나 땅을 만나는지 체크합니다. 이미 충돌중인 상태인 벽이나 땅은 체크하지 않기 때문에, 실제 콜라이더보다 약간 작은 radius로 검출합니다.
        Vector3 p1 =
            transform.TransformPoint(m_collider.center + Vector3.up * (0.5f * m_collider.height - m_collider.radius));
        Vector3 p2 =
            transform.TransformPoint(m_collider.center + Vector3.down * (0.5f * m_collider.height - m_collider.radius));
        int mask = LayerMask.GetMask(ConstVariables.MOVEMENT_COLLISION_LAYERS);

        //2.1. 캡슐캐스트는 시작 위치는 체크하지 않기 때문에, 시작위치를 체크하기 위해 OverlapCapsule도 체크합니다.
        //서있는 땅을 제외하고 다른 오브젝트와 충돌 시 이동하지 않습니다.
        Collider[] cols = ArrayPool<Collider>.Shared.Rent(100);
        int count = Physics.OverlapCapsuleNonAlloc(p1, p2, m_collider.radius - 0.01f, cols, mask);
        for (int i = 0; i < count; i++)
        {
            //나 자신을 검사했거나, 밟고있는 땅을 검사할 경우 continue
            if (cols[i].gameObject.GetInstanceID() != m_collider.gameObject.GetInstanceID()
                && (m_standingGround == null ||
                    cols[i].gameObject.GetInstanceID() != m_standingGround.gameObject.GetInstanceID()))
            {
                //내가 서있는 땅과 다른 곳과 충돌했다면 이동하지 않음
                //Debug.Log("Dash: 이상한 놈과 충돌 중...");
                ArrayPool<Collider>.Shared.Return(cols);
                return;
            }
        }

        ArrayPool<Collider>.Shared.Return(cols);

        //2.2. 오버랩캡슐에서 검출되지 않았다면, 캡슐캐스트로 체크합니다.
        //만약 검출되었다면, 검출된 위치까지만 이동합니다.
        if (Physics.CapsuleCast(p1, p2, m_collider.radius - 0.01f, m_dashDirection, out var hit,
                m_dashSpeed * Time.fixedDeltaTime, mask))
        {
            //벽이나 땅을 만난다면, 해당 위치까지만 이동합니다.
            //아래 주석은, MovePosition으로 로직을 변경할 가능성이 있어 남겨두었습니다.
            //m_rigidbody.MovePosition(transform.position + m_dashDirection * hit.distance);
            //Debug.DrawLine(transform.position + m_collider.center, hit.point);

            //TODO: 충돌 예정 시 충돌 직전거리까지만 이동하게 로직 변경
            return;
        }
        else
        {
            //아무것도 만나지 않는다면, 지정된 위치까지 이동합니다.
            //Debug.Log("미충돌");
            //m_rigidbody.MovePosition(transform.position + m_dashDirection * m_dashSpeed * Time.fixedDeltaTime);
        }

        //m_rigidbody.velocity = Vector3.zero;

        //강제로 속도를 대쉬값으로 만듭니다.
        m_rigidbody.velocity = m_dashDirection * m_dashSpeed;
    }

    private void UpdateDashCoolDown()
    {
        if (m_data.MaxDashCount > m_dashCount
            && m_dashCooldownStartTime + m_data.DashCoolTime <= Time.time)
        {
            //최대 대쉬 카운트가 아니면서
            //마지막 대쉬 시간부터 쿨타임이 지났다면 대쉬 카운트 +1
            m_dashCount += 1;
            Debug.Log($"대쉬 충전됨, 남은 대쉬 수: {m_dashCount}");

            //만약 더해줬어도 최대 대쉬 수가 아니라면, 지금 시간부터 다시 대쉬 쿨타임 카운트 시작
            if (m_dashCount != m_data.MaxDashCount)
            {
                m_dashCooldownStartTime = Time.time;
            }
        }
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
        m_actor.Animator.SetBool(s_animatorKnockBack, false);
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
}