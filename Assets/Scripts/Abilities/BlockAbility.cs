using UnityEngine;

public class BlockAbility : Ability
{
    [SerializeField]
    private Weapon m_defaultWeapon;
    
    [SerializeField]
    private Weapon m_blockWeapon;

    protected override void RegisterEvents(IEventProxy eventProxy)
    {
        base.RegisterEvents(eventProxy);
        
        eventProxy.AddHandler(nameof(OnBlockExit), OnBlockExit);
    }

    protected override void UnregisterEvents(IEventProxy eventProxy)
    {
        base.UnregisterEvents(eventProxy);
        
        eventProxy.RemoveHandler(nameof(OnBlockExit), OnBlockExit);
    }

    protected override void OnIdleState()
    {
        if (m_blockWeapon.IsEquipped)
        {
            Owner.Attack.ChangeWeapon(m_defaultWeapon);
        }
    }

    protected override void OnActivateState()
    {
        Owner.Attack.ChangeWeapon(m_blockWeapon);
    }

    private void OnBlockExit()
    {
        if (!Owner.Status.IsBlocking && m_blockWeapon.IsEquipped)
        {
            Owner.Attack.ChangeWeapon(m_defaultWeapon);
        }
    }
}