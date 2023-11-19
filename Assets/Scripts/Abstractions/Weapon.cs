using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격, 스킬의 부모 클래스
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    /// <summary>
    /// State는 Animation Event가 발생할 때 해당 Event의 정보로 변경됩니다.
    /// </summary>
    public enum AttackState
    {
        IDLE,
        LEAD_IN, //선딜
        HIT, //실제 공격
        FOLLOW_THROUGH, //후딜
    }

    public AttackState State { get; private set; }
    public Animator Animator { get; private set; }
    public Monster Owner { get; private set; }
    public AttackAnimationEventReceiver Receiver { get; private set; }
    public bool IsAttacking { get; private set; }

    [field: SerializeField]
    public WeaponData WeaponData { get; private set; }

    /// <summary>
    /// 실제 공격이 일어날 때 MonsterAttack의 attackEvent Handler에 의해 호출됩니다.
    /// </summary>
    public void StartAttack(Monster attacker)
    {
        if (attacker != Owner)
        {
            ChangeOwner(attacker);
        }
        
        //공격 중 = true;
        IsAttacking = true;
        Owner.Attack.IsAttacking = true;
        Attack();
    }

    protected abstract void Attack();

    public void ChangeOwner(Monster owner)
    {
        //기존 Receiver에 등록된 이벤트 삭제
        UnregisterAnimationEvents();

        //변수 초기화
        Owner = owner;
        Animator = owner.Animator;
        Receiver = Animator.GetComponent<AttackAnimationEventReceiver>();
        RegisterAnimationEvents();
    }

    public virtual void InvokeHitEvent(IEnumerable<AttackInfo> attackInfo) { Owner.Attack.OnHitEvent(attackInfo); }

    #region ProtectedAnimationEvents

    protected virtual void OnIdleMotion() { }

    protected virtual void OnLeadInMotion() { }

    protected virtual void OnHitMotion() { }

    protected virtual void OnFollowThroughMotion() { }

    #endregion

    #region AnimationInit

    private void InitIdleMotion(object sender, EventArgs e)
    {
        if (State == AttackState.IDLE) return;
        State = AttackState.IDLE;
        if (Owner)
        {
            Owner.Attack.IsAttacking = false;
        }
        OnIdleMotion();
    }

    private void InitLeadInMotion(object sender, EventArgs e)
    {
        //내가 공격중이 아니라면 return;
        if (!IsAttacking) return;
        
        if (State == AttackState.LEAD_IN) return;
        State = AttackState.LEAD_IN;
        OnLeadInMotion();
    }

    private void InitHitMotion(object sender, EventArgs e)
    {
        //내가 공격중이 아니라면 return;
        if (!IsAttacking) return;
        
        if (State == AttackState.HIT) return;
        State = AttackState.HIT;
        OnHitMotion();
    }

    private void InitFollowThroughMotion(object sender, EventArgs e)
    {
        //내가 공격중이 아니라면 return;
        if (!IsAttacking) return;
        
        //Follow Through: Attack이 끝나고 후딜 시작되는 상황
        //공격 종료 판정
        IsAttacking = false;
        Owner.Attack.IsAttacking = false;
        
        if (State == AttackState.FOLLOW_THROUGH) return;
        State = AttackState.FOLLOW_THROUGH;
        OnFollowThroughMotion();
    }

    #endregion

    #region UnityEventFunctions

    protected void OnDestroy()
    {
        UnregisterAnimationEvents();
    }

    #endregion

    #region EventRegisterFunctions

    protected virtual void RegisterAnimationEvents()
    {
        if (Receiver == null) return;
        Receiver.onIdle += InitIdleMotion;
        Receiver.onLeadIn += InitLeadInMotion;
        Receiver.onHit += InitHitMotion;
        Receiver.onFollowThrough += InitFollowThroughMotion;
    }

    protected virtual void UnregisterAnimationEvents()
    {
        if (Receiver == null) return;
        Receiver.onIdle -= InitIdleMotion;
        Receiver.onLeadIn -= InitLeadInMotion;
        Receiver.onHit -= InitHitMotion;
        Receiver.onFollowThrough -= InitFollowThroughMotion;
    }

    #endregion

}