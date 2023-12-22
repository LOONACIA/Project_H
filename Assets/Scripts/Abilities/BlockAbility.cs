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
        Owner.Attack.ChangeWeapon(m_defaultWeapon);
    }
}