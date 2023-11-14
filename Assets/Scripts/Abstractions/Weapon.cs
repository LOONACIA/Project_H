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
    /// 실제 공격 히트 시 히트한 대상(들)을 반환하는 이벤트입니다.
    /// </summary>
    public event EventHandler<IEnumerable<IHealth>> HitEventHandler;

    [SerializeField] protected MonsterAttack m_monsterAttack;
    protected Monster m_attacker;
    [SerializeField] protected Animator m_animator;
    protected AttackAnimationEventReceiver m_receiver;


    /// <summary>
    /// 실제 공격이 일어날 때 MonsterAttack의 attackEvent Handler에 의해 호출됩니다.
    /// </summary>
    public abstract void StartAttack(object o, Monster attacker);
    
    protected virtual void InvokeHitEvent(IEnumerable<IHealth> hitObjects) { HitEventHandler?.Invoke(this, hitObjects); }

    #region AnimationEvents

    protected virtual void OnAnimationStart(object sender, EventArgs e) { }

    protected virtual void OnAnimationEvent(object sender, EventArgs e) { }

    protected virtual void OnAnimationEnd(object sender, EventArgs e) { }

    #endregion
    
    #region UnityEventFunctions
    
    protected virtual void Awake()
    {
        m_attacker = m_monsterAttack.GetComponent<Monster>();
        m_receiver = m_animator.GetComponent<AttackAnimationEventReceiver>();
    }

    protected virtual void OnEnable()
    {
        RegisterAnimationEvents();
        
        m_monsterAttack.attackEventHandler += StartAttack;
    }

    protected void OnDisable()
    {
        m_monsterAttack.attackEventHandler -= StartAttack;
        
        UnregisterAnimationEvents();
    }

    #endregion

    #region EventRegisterFunctions
    protected virtual void RegisterAnimationEvents()
    {
        if (m_receiver == null) return;
        m_receiver.AttackStart += OnAnimationStart;
        m_receiver.AttackHit += OnAnimationEvent;
        m_receiver.AttackFinish += OnAnimationEnd;
    }

    protected virtual void UnregisterAnimationEvents()
    {
        if (m_receiver == null) return;
        m_receiver.AttackStart -= OnAnimationStart;
        m_receiver.AttackHit -= OnAnimationEvent;
        m_receiver.AttackFinish -= OnAnimationEnd;
    }
    #endregion
}