using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Weapon m_attackWeapon;
    [SerializeField] private Weapon m_skillWeapon;
    [SerializeField] private Weapon m_blockPushWeapon;
    
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

    #region BlockPush
    public virtual void OnBlockPushIdle()
    {
        m_blockPushWeapon.EnterIdleState(this, null);
    }

    protected virtual void OnBlockPushLeadIn()
    {
        m_blockPushWeapon.EnterLeadInState(this, null);
    }

    protected virtual void OnBlockPushHit()
    {
        m_blockPushWeapon.EnterHitState(this, null);
    }

    protected virtual void OnBlockPushFollowThrough()
    {
        m_blockPushWeapon.EnterFollowThroughState(this, null);
    }


    #endregion
}
