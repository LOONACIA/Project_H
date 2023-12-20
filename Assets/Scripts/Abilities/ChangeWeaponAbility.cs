using System;
using UnityEngine;

public class ChangeWeaponAbility : Ability
{
	[SerializeField]
    private Weapon m_primaryWeapon;
    
    [SerializeField]
    private Weapon m_secondaryWeapon;

    private void Start()
    {
        Owner.Attack.ChangeWeapon(m_primaryWeapon);
    }

    protected override void OnActivateState()
    {
        Weapon newWeapon = m_primaryWeapon.IsEquipped ? m_secondaryWeapon : m_primaryWeapon;
        Owner.Attack.ChangeWeapon(newWeapon);
    }
}