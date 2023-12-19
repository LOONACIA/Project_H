using UnityEngine;

public class ChangeWeaponAbility : Ability
{
	[SerializeField]
    private Weapon m_primaryWeapon;
    
    [SerializeField]
    private Weapon m_secondaryWeapon;

    protected override void OnActivateState()
    {
        base.OnActivateState();
        
        Weapon newWeapon = m_primaryWeapon.IsEquipped ? m_secondaryWeapon : m_primaryWeapon;
        Debug.Log(newWeapon.name);
        Owner.Attack.ChangeWeapon(newWeapon);
    }
}