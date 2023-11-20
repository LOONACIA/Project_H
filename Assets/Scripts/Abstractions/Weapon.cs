using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격, 스킬의 부모 클래스
/// </summary>
[RequireComponent(typeof(AttackAnimationEventReceiver))]
public abstract class Weapon : MonoBehaviour
{
    #region Properties

    [field: SerializeField]
    public WeaponType Type { get; private set; }
    
    public AttackState State { get; private set; }

    public event EventHandler<IEnumerable<WeaponAttackInfo>> OnHitEvent;

    #endregion
    
    /// <summary>
    /// 실제 공격이 일어날 때 MonsterAtaack의 attackEvent Handler에 의해 호출됩니다.
    /// </summary>
    public void StartAttack()
    {
        Attack();
    }

    protected abstract void Attack();

    public virtual void InvokeHitEvent(IEnumerable<WeaponAttackInfo> attackInfo)
    {
        OnHitEvent?.Invoke(this, attackInfo);
    }

    #region ProtectedAnimationEvents

    protected virtual void OnIdleMotion() { }

    protected virtual void OnLeadInMotion() { }

    protected virtual void OnHitMotion() { }

    protected virtual void OnFollowThroughMotion() { }

    #endregion

    #region Change State

    public void EnterIdleState(object sender, EventArgs e)
    {
        if (State == AttackState.IDLE) return;
        State = AttackState.IDLE;
        OnIdleMotion();
    }

    public void EnterLeadInState(object sender, EventArgs e)
    {
        if (State == AttackState.LEAD_IN) return;
        State = AttackState.LEAD_IN;
        OnLeadInMotion();
    }

    public void EnterHitState(object sender, EventArgs e)
    {
        if (State == AttackState.HIT) return;
        State = AttackState.HIT;
        OnHitMotion();
    }

    public void EnterFollowThroughState(object sender, EventArgs e)
    {
        //Follow Through: Attack이 끝나고 후딜 시작되는 상황
        //공격 종료 판정

        if (State == AttackState.FOLLOW_THROUGH) return;
        State = AttackState.FOLLOW_THROUGH;
        OnFollowThroughMotion();
    }

    #endregion

    public enum WeaponType
    {
        ATTACK_WEAPON,
        SKILL_WEAPON,
        BLOCKPUSH_WEAPON,
    }


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
}