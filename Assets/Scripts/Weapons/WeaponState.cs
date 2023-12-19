public enum WeaponState
{
    /// <summary>
    /// 기본 상태
    /// </summary>
    Idle,
    
    /// <summary>
    /// 공격 선딜 상태
    /// </summary>
    WaitAttack,
    
    /// <summary>
    /// 공격 상태
    /// </summary>
    Attack,
    
    /// <summary>
    /// 공격 후딜 상태
    /// </summary>
    Recovery
}