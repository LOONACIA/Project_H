using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격에 따른 
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private AttackAnimationEventReceiver m_receiver;
    
    /// <summary>
    /// 실제 공격 히트 시 히트한 대상(들)을 반환하는 이벤트입니다.
    /// </summary>
    public event EventHandler<IEnumerable<IHealth>> AttackHit;

    #region HitEvent Functions
    
    protected virtual void OnHitEvent(IEnumerable<IHealth> hitObjects) { AttackHit?.Invoke(this, hitObjects); }

    #endregion

    #region AnimationEvent Functions

    protected virtual void OnAnimationStart(object sender, EventArgs e) { }

    protected virtual void OnAnimationEvent(object sender, EventArgs e) { }

    protected virtual void OnAnimationEnd(object sender, EventArgs e) { }

    #endregion
    
    #region UnityEventFunctions

    protected virtual void OnEnable()
    {
        RegisterAnimationEvents();
    }

    protected void OnDisable()
    {
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