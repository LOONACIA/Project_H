using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponAttackInfo
{
    #region Constructor

    public WeaponAttackInfo(Actor hitObject)
        : this(hitObject, Vector3.zero, hitObject.transform.position, Vector3.zero)
    {
    }

    public WeaponAttackInfo(Actor hitObject, Vector3 attackDirection)
        : this(hitObject, attackDirection, Vector3.zero, Vector3.zero)
    {
    }
    
    public WeaponAttackInfo(Actor hitObject, Vector3 attackDirection, Vector3 hitPosition)
        : this(hitObject, attackDirection, hitPosition, Vector3.zero)
    {
    }


    public WeaponAttackInfo(
        Actor hitObject,
        Vector3 attackDirection,
        Vector3 hitPosition,
        Vector3 knockBackDirection)
    {
        HitObject = hitObject;
        AttackDirection = attackDirection;
        HitPosition = hitPosition;
        KnockBackDirection = knockBackDirection;
    }

    #endregion

    #region Properties

    public Vector3 HitPosition { get; }
    public Vector3 AttackDirection { get; }

    public Actor HitObject { get; }

    public Vector3 KnockBackDirection { get; }
    #endregion
}