using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterAttack))]
[RequireComponent(typeof(MonsterMovement))]
public class Monster : Actor
{
    private WaitUntil m_waitUntilNavMeshAgentEnableCache;
    
    private NavMeshAgent m_navMeshAgent;

    // Movement animation ratio (for lerp)
    private float m_movementAnimationRatio;

    private Vector3 m_directionInput;
    
    private CoroutineEx m_waitUntilNavMeshAgentEnableCoroutine;

    public MonsterAttack Attack { get; private set; }

    public MonsterMovement Movement { get; private set; }

    public ObservableCollection<Actor> Targets { get; } = new();

    protected override void Awake()
    {
        base.Awake();

        m_navMeshAgent = GetComponent<NavMeshAgent>();
        Attack = GetComponent<MonsterAttack>();
        Movement = GetComponent<MonsterMovement>();

        //네브매쉬의 Agent와 Rigidbody의 Kinematic은 세트로 움직여야 함
        //MonsterMovement의 FixedUpdate 또는 Monster의 OnCollisionEnter에서 판정하여 On/Off됨
        m_navMeshAgent.enabled = false;
        m_rigidbody.isKinematic = false;
        m_waitUntilNavMeshAgentEnableCache = new(() => m_navMeshAgent!.enabled);
    }

    protected override void Start()
    {
        base.Start();

        if (Attack.CurrentWeapon == null)
        {
            var weapons = Animator.GetComponents<Weapon>();
            Attack.ChangeWeapon(weapons.FirstOrDefault(weapon => weapon.enabled));
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Targets.CollectionChanged -= OnTargetCollectionChanged;
        Targets.CollectionChanged += OnTargetCollectionChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Targets.CollectionChanged -= OnTargetCollectionChanged;
    }

    protected void Update()
    {
        UpdateAnimator();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsPossessed)
        {
            return;
        }

        if (((1 << other.gameObject.layer) & Movement.Data.WhatIsGround) != 0 && Movement.IsOnGround &&
            !Status.IsKnockBack)
        {
            m_rigidbody.isKinematic = true;
            m_navMeshAgent.enabled = true;
        }
    }

    public override void Move(Vector3 direction)
    {
        m_directionInput = direction;
        Movement.Move(direction);
    }

    public override void TryJump()
    {
        Movement.TryJump();
    }

    public override void TryAttack()
    {
        Attack.Attack();
    }

    public override void Ability(bool isToggled)
    {
        Attack.Ability(isToggled);
    }

    public override void Dash(Vector3 direction)
    {
        //Movement.isDashing = !Movement.isDashing;
        if (IsPossessed)
        {
            //Movement.TryDash(FirstPersonCameraPivot.transform.forward);
            Movement.TryDash(direction);
        }
    }

    protected override void EnableAIComponents()
    {
        // NavMeshAgent가 있는 경우 BehaviorTree를 활성화하기 전에 NavMeshAgent를 활성화해야 함
        // 따라서 base.EnableAIComponents()를 호출하지 않고, 별도로 구현
        if (m_rigidbody != null)
        {
            m_rigidbody.isKinematic = true;
        }

        if (m_behaviorTree != null)
        {
            m_behaviorTree.enabled = false;
            m_waitUntilNavMeshAgentEnableCoroutine = CoroutineEx.Create(this, WaitUntilNavMeshAgentEnable());
        }

        if (Movement != null && Movement.IsOnGround)
        {
            m_navMeshAgent.enabled = true;
            m_rigidbody.isKinematic = true;
        }
        else
        {
            m_navMeshAgent.enabled = false;
            m_rigidbody.isKinematic = false;
        }
        return;
        
        IEnumerator WaitUntilNavMeshAgentEnable()
        {
            if (m_navMeshAgent != null)
            {
                yield return m_waitUntilNavMeshAgentEnableCache;
            }

            m_behaviorTree.enabled = true;
        }
    }

    protected override void DisableAIComponents()
    {
        base.DisableAIComponents();

        m_waitUntilNavMeshAgentEnableCoroutine?.Abort();
        m_navMeshAgent.enabled = false;
    }

    protected virtual void UpdateAnimator()
    {
        var velocity = m_rigidbody.velocity.GetFlatVector();
        if (IsPossessed && m_directionInput.magnitude <= 0f)
        {
            velocity = Vector3.zero;
        }

        // move blend tree 값 설정 (정지 0, 걷기 0.5, 달리기 1)
        float movementRatio = 0f;

        if (velocity.magnitude > 0f && IsPossessed)
        {
            movementRatio = Movement.IsDashing ? 1 : 0.5f;
        }

        if (!IsPossessed)
        {
            float movementValue = Movement.MovementRatio;
            float agentValue = m_navMeshAgent.velocity.GetFlatMagnitude();
            movementRatio = movementValue > agentValue ? movementValue : agentValue;
        }

        m_movementAnimationRatio = Mathf.Lerp(m_movementAnimationRatio, movementRatio, Time.deltaTime * 5f);

        // 애니메이터에 적용
        Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_MOVEMENT_RATIO, m_movementAnimationRatio);

        // 몬스터 상태이상 적용
        if (!IsPossessed)
            Animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_IS_STUNED, Status.IsStunned);
    }

    protected override void OnPossessed()
    {
        base.OnPossessed();

        if (Attack.CurrentWeapon == null || !Attack.CurrentWeapon.isActiveAndEnabled)
        {
            Attack.ChangeWeapon(Animator.GetComponent<Weapon>());
        }

        Attack.Target = Vector3.zero;
        m_directionInput = Vector3.zero;
    }

    protected override void OnUnPossessed()
    {
        base.OnUnPossessed();

        if (Attack.CurrentWeapon == null || !Attack.CurrentWeapon.isActiveAndEnabled)
        {
            Attack.ChangeWeapon(Animator.GetComponent<Weapon>());
        }
        
        Status.IsBlocking = false;
    }

    private void OnTargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (Actor target in e.OldItems)
            {
                target.Dying -= OnTargetDying;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (Actor target in e.NewItems)
            {
                if (target == this)
                {
                    Targets.Remove(target);
                }

                target.Dying += OnTargetDying;
            }
        }
    }

    private void OnTargetDying(object sender, in AttackInfo info)
    {
        if (sender is not Actor actor)
        {
            return;
        }

        int targetIndex = Targets.IndexOf(actor);
        if (targetIndex != -1)
        {
            Targets.RemoveAt(targetIndex);
        }
    }
}