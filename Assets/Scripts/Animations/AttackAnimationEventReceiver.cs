using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Weapon m_attackWeapon;
    [SerializeField] private Weapon m_skillWeapon;
    
    #region Attack
    
    public virtual void OnAttackIdle()
    {
        m_attackWeapon.EnterIdleState(this, null);
    }

    protected virtual void OnAttackLeadIn()
    {
        m_attackWeapon.EnterLeadInState(this, null);
    }
	
    protected virtual void OnAttackHit()
    {
        m_attackWeapon.EnterHitState(this,null);
    }
    
    
    protected virtual void OnAttackFollowThrough()
    {
        m_attackWeapon.EnterFollowThroughState(this,null);
    }
    
    #endregion
    
    #region Skill
    
    public virtual void OnSkillIdle()
    {
        m_skillWeapon.EnterIdleState(this, null);
    }

    protected virtual void OnSkillLeadIn()
    {
        m_skillWeapon.EnterLeadInState(this, null);
    }
	
    protected virtual void OnSkillHit()
    {
        m_skillWeapon.EnterHitState(this,null);
    }
    
    protected virtual void OnSkillFollowThrough()
    {
        m_skillWeapon.EnterFollowThroughState(this,null);
    }
    
    #endregion
	
}
