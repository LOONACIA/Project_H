using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackAnimationEventReceiver : MonoBehaviour
{
    private Weapon m_attackWeapon;
    private Weapon m_skillWeapon;
    private Weapon m_blockPushWeapon;
    
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

    private void Awake()
    {
        Weapon[] weapons = GetComponents<Weapon>();
        RegisterWeaponComponents(weapons, ref m_attackWeapon, ref m_skillWeapon, ref m_blockPushWeapon);
    }

    private void RegisterWeaponComponents(Weapon[] weapons, ref Weapon attackWeapon, ref Weapon skillWeapon, ref Weapon blockPushWeapon)
    {
        foreach (var weapon in weapons)
        {
            switch (weapon.Type)
            {
                case Weapon.WeaponType.ATTACK_WEAPON:
                    if (attackWeapon != null)
                    {
                        Debug.LogError("AttackWeapon 중복 등록됨");
                    }
                    attackWeapon = weapon;
                    break;
                case Weapon.WeaponType.SKILL_WEAPON:
                    if (skillWeapon != null)
                    {
                        Debug.LogError("SkillWeapon 중복 등록됨");
                    }
                    skillWeapon = weapon;
                    break;
                case Weapon.WeaponType.BLOCKPUSH_WEAPON:
                    if (blockPushWeapon != null)
                    {
                        Debug.LogError("BlockPushWeapon 중복 등록됨");
                    }
                    blockPushWeapon = weapon;
                    break;
                default:
                    Debug.LogError("초기화 오류: 등록되지 않은 Weapon 등록됨");
                    break;
            }
        }
    }
}
