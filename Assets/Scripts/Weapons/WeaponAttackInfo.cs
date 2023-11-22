using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponAttackInfo
{
    #region Constructor

    public WeaponAttackInfo(Actor hitObject)
        : this(hitObject, Vector3.zero, Vector3.zero)
    {
    }

    public WeaponAttackInfo(Actor hitObject, Vector3 attackDirection)
        : this(hitObject, Vector3.zero, Vector3.zero)
    {
    }

    public WeaponAttackInfo(
        Actor hitObject,
        Vector3 attackDirection,
        Vector3 knockBackDirection)
    {
        AttackDirection = attackDirection;
        HitObject = hitObject;
        KnockBackDirection = knockBackDirection;
    }

    #endregion

    #region Properties
    public Vector3 AttackDirection { get; }

    public Actor HitObject { get; }

    public Vector3 KnockBackDirection { get; }
    #endregion
}