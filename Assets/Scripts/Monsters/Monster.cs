using System;
using System.Collections;
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

    // TODO: 빙의 게이지 관련 처리
    //private float m_stamina = 0f;

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

    public override void Move(Vector3 direction)
    {
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

    public override void Dash()
    {
        //Movement.isDashing = !Movement.isDashing;
        if (IsPossessed)
        {
            Movement.TryDash(FirstPersonCameraPivot.transform.forward);
        }
    }
    
    public override void Block(bool value)
    {
        if (IsPossessed)
        {
            Status.IsBlocking = value;
            Animator.SetBool(s_blockAnimationKey, value);
        }
    }
    
    protected override void OnPossessed()
    {
        base.OnPossessed();

        // TODO: 빙의 게이지 관련 처리
        Status.Damage = Attack.Data.PossessedAttack.Damage;
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