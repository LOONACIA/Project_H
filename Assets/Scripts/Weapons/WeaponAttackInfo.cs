using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponAttackInfo
{
    #region PublicVariables

    //공격이 들어온 방향
    private Vector3 attackDirection;

    //피격자
    private IHealth hitObject;
    
    private Vector3 knockBackDirection;

    #endregion

    #region Properties

    public Vector3 AttackDirection => attackDirection;
    public IHealth HitObject => hitObject;
    public Vector3 KnockBackDirection => knockBackDirection;

    #endregion


    #region Constructor

    public WeaponAttackInfo(IHealth hitObject)
        : this(hitObject, new Vector3(),new Vector3())
    {
    }

    public WeaponAttackInfo(IHealth hitObject, Vector3 attackDirection)
        : this(hitObject,new Vector3(),new Vector3())
    {
    }

    public WeaponAttackInfo(
        IHealth hitObject,
        Vector3 attackDirection,
        Vector3 knockBackDirection)
    {
        this.attackDirection = attackDirection;
        this.hitObject = hitObject;
        
        this.knockBackDirection = knockBackDirection;
    }

    #endregion
}