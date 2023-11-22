using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DamageInfo
{
    #region Constructor
    
    public DamageInfo(int damage, Vector3 attackDirection,Vector3 hitPosition, Actor attacker)
    {
        Damage = damage;
        AttackDirection = attackDirection;
        Attacker = attacker;
        HitPosition = hitPosition;
    }

    #endregion

    #region Properties

    public int Damage { get; }
    public Vector3 AttackDirection { get; }
    public Vector3 HitPosition { get; }
    public Actor Attacker { get; }
    
    #endregion
}