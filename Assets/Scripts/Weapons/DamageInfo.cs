using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DamageInfo
{
    #region Constructor
    
    public DamageInfo(int damage, Vector3 attackDirection, Actor attacker)
    {
        Damage = damage;
        AttackDirection = attackDirection;
        Attacker = attacker;
    }

    #endregion

    #region Properties

    public int Damage { get; }
    public Vector3 AttackDirection { get; }
    public Actor Attacker { get; }
    
    #endregion
}