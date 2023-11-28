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
