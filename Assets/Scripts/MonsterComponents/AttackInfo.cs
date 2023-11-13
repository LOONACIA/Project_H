using System;
using UnityEngine;

public struct AttackInfo
{
    public AttackInfo(Actor attacker, IHealth victim, int damage, Vector3 hitNormal)
    {
        Attacker = attacker;
        Victim = victim;
        Damage = damage;
        HitNormal = hitNormal;
    }

    public Actor Attacker { get; }

    public IHealth Victim { get; }

    public int Damage { get; }
    
    public Vector3 HitNormal { get; }
}
