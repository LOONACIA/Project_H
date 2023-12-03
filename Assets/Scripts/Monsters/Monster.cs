using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterAttack))]
[RequireComponent(typeof(MonsterMovement))]
public class Monster : Actor
{
    private static readonly int s_blockAnimationKey = Animator.StringToHash("Block");

    // Movement animation ratio (for lerp)
    private float m_movementAnimationRatio;

    private Vector3 m_directionInput;

    public MonsterAttack Attack { get; private set; }
    
    public MonsterMovement Movement { get; private set; }

    public ObservableCollection<Actor> Targets { get; } = new();
    
    protected override void Awake()
    {
        base.Awake();

        Attack = GetComponent<MonsterAttack>();
        Movement = GetComponent<MonsterMovement>();
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

    public override void Skill()
    {
        Attack.Skill();
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
    
    public override void Block(bool value)
    {
        if (IsPossessed)
        {
            //Status.IsBlocking = value;
            Animator.SetBool(s_blockAnimationKey, value);
        }
    }
    
    protected virtual void UpdateAnimator()
    {
        var velocity = IsPossessed ? m_rigidbody.velocity.GetFlatVector() : m_navMeshAgent.velocity.GetFlatVector();
        if (IsPossessed && m_directionInput.magnitude <= 0f)
        {
            velocity = Vector3.zero;
        }
        
        // move blend tree 값 설정 (정지 0, 걷기 0.5, 달리기 1)
        float movementRatio = 0f;
        if (velocity.magnitude > 0f)
        {
            movementRatio = Movement.isDashing ? 1 : 0.5f;
        }
        m_movementAnimationRatio = Mathf.Lerp(m_movementAnimationRatio, movementRatio, Time.deltaTime * 5f);

        // 애니메이터에 적용
        Animator.SetFloat("MovementRatio", m_movementAnimationRatio);
    }
    
    protected override void OnPossessed()
    {
        base.OnPossessed();
        
        Status.Damage = Attack.Data.PossessedAttack.Damage;
        m_directionInput = Vector3.zero;
    }

    protected override void OnUnPossessed()
    {
        base.OnUnPossessed();

        Status.Damage = Attack.Data.Attack.Damage;
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
                target.Dying += OnTargetDying;
            }
        }
    }

    private void OnTargetDying(object sender, EventArgs e)
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