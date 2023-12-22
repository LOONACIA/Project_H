using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    /// <summary>
    /// IsAttacking을 내부에서 핸들링하기 위한 변수
    /// </summary>
    protected bool m_isAttacking;
    
    private WeaponState m_state;
    
    private IEventProxy m_eventProxy;
    
    public Monster Owner { get; set; }
    
    [field: SerializeField]
    public int Damage { get; set; }
    
    [field: SerializeField]
    public float KnockDownTime { get; set; }
    
    [field: SerializeField]
    public float KnockBackPower { get; set; }

    public virtual bool CanAttack { get; protected set; } = true;

    public bool IsEquipped { get; private set; }

    public virtual Vector3 Target { get; set; }

    public bool IsAttacking => State != WeaponState.Idle || m_isAttacking;

    public WeaponState State
    {
        get => m_state;
        private set
        {
            m_state = value;
            OnStateChanged(m_state);
        }
    }
    
    public event EventHandler AttackHit;
    
    public event EventHandler<WeaponState> StateChanged;

    protected virtual void Awake()
    {
        if (Owner == null)
        {
            Owner = GetComponentInParent<Monster>();
        }

        if (m_eventProxy == null)
        {
            m_eventProxy = GetComponent<IEventProxy>();
        }
    }

    public void Equip(Monster owner)
    {
        if (m_eventProxy == null)
        {
            m_eventProxy = GetComponent<IEventProxy>();
        }
        
        IsEquipped = true;
        Owner = owner;
        RegisterEvents(m_eventProxy);
        OnEquipped();
    }
    
    public void UnEquip()
    {
        IsEquipped = false;
        UnregisterEvents(m_eventProxy);
        OnUnEquipped();
    }
    
    protected virtual void OnEquipped()
    {
    }
    
    protected virtual void OnUnEquipped()
    {
    }

    protected void Hit(IEnumerable<AttackInfo> e)
    {
        bool isHit = false;
        foreach (var attackInfo in e.Where(attackInfo => attackInfo.Victim.gameObject != Owner.gameObject))
        {
            bool victimIsActor = attackInfo.Victim.gameObject.TryGetComponent<Actor>(out var victim);
            
            if (victimIsActor)
            {
                // 빙의되지 않은 몬스터가 타겟이 아닌 대상을 공격하는 경우
                if (!Owner.IsPossessed && !Owner.Targets.Contains(victim))
                {
                    continue;
                }
                
                //넉다운 적용
                if (victim.Status.CanKnockDown && KnockDownTime > 0f)
                {
                    victim.Status.SetKnockDown(KnockDownTime);
                }

                //넉백 적용
                if (victim.Status.CanKnockBack && KnockBackPower != 0f)
                {
                    MonsterMovement movement = attackInfo.Victim.gameObject.GetComponent<MonsterMovement>();
                    // TODO: 넉백 방향 수정
                    movement.TryKnockBack(attackInfo.AttackDirection, KnockBackPower);
                }
            }
            
            attackInfo.Victim.TakeDamage(attackInfo);
            isHit = true;
        }

        if (isHit)
        {
            OnAttackHit();
        }
    }

    protected virtual void OnAttackHit()
    {
        AttackHit?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnStateChanged(WeaponState state)
    {
        StateChanged?.Invoke(this, state);
    }
    
    protected virtual void RegisterEvents(IEventProxy eventProxy)
    {
        eventProxy.AddHandler($"On{nameof(WeaponState.Idle)}", OnIdle);
        eventProxy.AddHandler($"On{nameof(WeaponState.WaitAttack)}", OnWaitAttack);
        eventProxy.AddHandler($"On{nameof(WeaponState.Attack)}", OnAttack);
        eventProxy.AddHandler($"On{nameof(WeaponState.Recovery)}", OnRecovery);
    }
    
    protected virtual void UnregisterEvents(IEventProxy eventProxy)
    {
        eventProxy.RemoveHandler($"On{nameof(WeaponState.Idle)}", OnIdle);
        eventProxy.RemoveHandler($"On{nameof(WeaponState.WaitAttack)}", OnWaitAttack);
        eventProxy.RemoveHandler($"On{nameof(WeaponState.Attack)}", OnAttack);
        eventProxy.RemoveHandler($"On{nameof(WeaponState.Recovery)}", OnRecovery);
    }

    protected virtual void OnIdleState()
    {
    }

    protected virtual void OnWaitAttackState()
    {
    }

    protected virtual void OnAttackState()
    {
    }

    protected virtual void OnRecoveryState()
    {
    }

    private void OnIdle()
    {
        State = WeaponState.Idle;
        OnIdleState();
    }

    private void OnWaitAttack()
    {
        if (State == WeaponState.WaitAttack)
        {
            return;
        }
        
        State = WeaponState.WaitAttack;
        OnWaitAttackState();
    }
    
    private void OnAttack()
    {
        if (State == WeaponState.Attack)
        {
            return;
        }
        
        State = WeaponState.Attack;
        OnAttackState();
    }
    
    private void OnRecovery()
    {
        if (State == WeaponState.Recovery)
        {
            return;
        }
        
        State = WeaponState.Recovery;
        OnRecoveryState();
    }
    
    private void OnValidate()
    {
        if (m_eventProxy == null)
        {
            m_eventProxy = GetComponent<IEventProxy>();
        }

        if (m_eventProxy == null)
        {
            Debug.LogWarning("EventProxy is null", gameObject);
        }
    }
}