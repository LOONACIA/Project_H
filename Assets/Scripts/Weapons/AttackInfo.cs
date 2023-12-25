using UnityEngine;

/// <summary>
/// The information of an attack.
/// </summary>
public readonly struct AttackInfo
{
    public AttackInfo(GameObject attacker, IHealth victim, int damage, Vector3 hitPoint, Vector3 attackDirection)
    {
        Attacker = attacker;
        Victim = victim;
        Damage = damage;
        HitPoint = hitPoint;
        AttackDirection = attackDirection.normalized;
    }
    
    /// <summary>
    /// The attacker of this attack. Can be null.
    /// </summary>
    public GameObject Attacker { get; }

    /// <summary>
    /// The victim of this attack. Cannot be null.
    /// </summary>
    public IHealth Victim { get; }

    /// <summary>
    /// The damage of this attack.
    /// </summary>
    public int Damage { get; }
    
    /// <summary>
    /// The point where this attack hit.
    /// </summary>
    public Vector3 HitPoint { get; }
    
    /// <summary>
    /// The normal of the point where this attack hit. From the attacker to the victim.
    /// </summary>
    public Vector3 AttackDirection { get; }
}