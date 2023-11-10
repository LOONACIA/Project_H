using System;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterAttack))]
[RequireComponent(typeof(MonsterHealth))]
[RequireComponent(typeof(MonsterStatus))]
[RequireComponent(typeof(MonsterMovement))]
public class Monster : Actor
{
    // TODO: 빙의 게이지 관련 처리
    private float m_stamina = 0f;
    
    public MonsterAttack Attack { get; private set; }
    
    public MonsterHealth Health { get; private set; }
    
    public MonsterMovement Movement { get; private set; }

    public MonsterStatus Status { get; private set; }

    public ObservableCollection<Actor> Targets { get; } = new();
    
    protected override void Awake()
    {
        base.Awake();

        Attack = GetComponent<MonsterAttack>();
        Health = GetComponent<MonsterHealth>();
        Status = GetComponent<MonsterStatus>();
        Movement = GetComponent<MonsterMovement>();
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
        Movement.isDashing = !Movement.isDashing;
    }

    public override void Possessed()
    {
        base.Possessed();

        // TODO: 빙의 게이지 관련 처리
        Status.Damage = Attack.Data.PossessedDamage;
    }

    public override void Unpossessed()
    {
        base.Unpossessed();

        Status.Damage = Attack.Data.Damage;
    }
}