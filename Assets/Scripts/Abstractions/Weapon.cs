using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격, 스킬의 부모 클래스
/// </summary>
[RequireComponent(typeof(AttackAnimationEventReceiver))]
public abstract class Weapon : MonoBehaviour
{
    #region

    private bool m_isAttackTriggered = false;
    
    #endregion
    
    #region Properties

    public bool IsAttacking => State != AttackState.Idle || m_isAttackTriggered;
    
    public virtual Vector3 Target { get; set; }

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
        m_isAttackTriggered = true;
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
        //Idle 상태로 들어가면 모든 공격 트리거가 초기화됨. (Animator 참조)
        
        //Weapon이 Idle이더라도, Attack 애니메이션으로 들어가기 전에 Idle 이벤트가 실행되었다면 Animator의 Attack Trigger가 초기화 되므로 공격 트리거 정보역시 소멸해야함
        m_isAttackTriggered = false;
        
        if (State == AttackState.Idle) return;
        State = AttackState.Idle;
        OnIdleMotion();
    }

    public void EnterLeadInState(object sender, EventArgs e)
    {
        if (State == AttackState.LeadIn) return;
        State = AttackState.LeadIn;
        OnLeadInMotion();
    }

    public void EnterHitState(object sender, EventArgs e)
    {
        if (State == AttackState.Hit) return;
        State = AttackState.Hit;
        OnHitMotion();
    }

    public void EnterFollowThroughState(object sender, EventArgs e)
    {
        //Follow Through: Attack이 끝나고 후딜 시작되는 상황
        //공격 종료 판정

        if (State == AttackState.FollowThrough) return;
        State = AttackState.FollowThrough;
        OnFollowThroughMotion();
    }

    #endregion

    public enum WeaponType
    {
        AttackWeapon,
        SkillWeapon,
        BlockPushWeapon,
    }


    /// <summary>
    /// State는 Animation Event가 발생할 때 해당 Event의 정보로 변경됩니다.
    /// </summary>
    public enum AttackState
    {
        Idle,
        LeadIn, //선딜
        Hit, //실제 공격
        FollowThrough, //후딜
    }
}