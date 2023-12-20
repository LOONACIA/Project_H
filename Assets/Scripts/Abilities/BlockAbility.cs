using UnityEngine;

public class BlockAbility : Ability
{
    [SerializeField]
    private Weapon m_defaultWeapon;
    
    [SerializeField]
    private Weapon m_blockWeapon;
    
    protected override void OnActivateState()
    {
        Owner.Attack.ChangeWeapon(m_blockWeapon);
    }

    protected override void OnIdleState()
    {
        if (!m_defaultWeapon.IsEquipped)
        {
            Owner.Attack.ChangeWeapon(m_defaultWeapon);
        }
    }
}