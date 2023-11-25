using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 공격/스킬 구조 개편 후 제거
public class ShooterSkill : Weapon
{
    private ShooterWeapon m_weapon;
    
    private void Awake()
    {
        m_weapon = GetComponent<ShooterWeapon>();
    }

    protected override void Attack()
    {
        m_weapon.ChangeMode();
    }
}
