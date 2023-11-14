using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격, 스킬의 부모 클래스
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public Monster Owner { get; private set; }
    public AttackAnimationEventReceiver Receiver { get; private set; }
    public bool IsAttacking { get; set; }

    /// <summary>
    /// 실제 공격이 일어날 때 MonsterAttack의 attackEvent Handler에 의해 호출됩니다.
    /// </summary>
    public void StartAttack(Monster attacker)
    {
        if (attacker != Owner)
        {
            ChangeOwner(attacker);
        }
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
    
    protected virtual void InvokeHitEvent(IEnumerable<IHealth> hitObjects) { Owner.Attack.OnHitEvent(this,hitObjects); }

    #region AnimationEvents

    protected virtual void OnAnimationStart(object sender, EventArgs e) { }

    protected virtual void OnAnimationEvent(object sender, EventArgs e) { }

    protected virtual void OnAnimationEnd(object sender, EventArgs e) { }

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
        Receiver.AttackStart += OnAnimationStart;
        Receiver.AttackHit += OnAnimationEvent;
        Receiver.AttackFinish += OnAnimationEnd;
    }

    protected virtual void UnregisterAnimationEvents()
    {
        if (Receiver == null) return;
        Receiver.AttackStart -= OnAnimationStart;
        Receiver.AttackHit -= OnAnimationEvent;
        Receiver.AttackFinish -= OnAnimationEnd;
    }
    #endregion
}