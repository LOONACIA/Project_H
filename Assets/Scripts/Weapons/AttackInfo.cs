using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackInfo
{

    #region PublicVariables

    public int damage;

    //공격이 들어온 방향
    public Vector3 attackDirection;

    //공격자
    public Monster attacker;

    //피격자
    public IHealth hitObject;

    #region KnockDown

    public float knockDownTime;

    #endregion

    #region KnockBack

    public float knockBackPower;
    public bool isKnockBackOverwrite;
    public Vector3 knockBackDirection;

    #endregion

    #endregion


    #region Constructor

    public AttackInfo()
        : this(0, new Vector3(), null, null, 0f, 0f, false, new Vector3())
    {
    }

    public AttackInfo(int damage, Vector3 attackDirection, Monster attacker, IHealth hitObject)
        : this(damage, attackDirection, attacker, hitObject, 0f, 0f, false, new Vector3())
    {
    }

    public AttackInfo(int damage,
        Vector3 attackDirection,
        Monster attacker,
        IHealth hitObject,
        float knockDownTime,
        float knockBackPower,
        bool isKnockBackOverwrite,
        Vector3 knockBackDirection)
    {
        this.damage = damage;
        this.attackDirection = attackDirection;
        this.attacker = attacker;
        this.hitObject = hitObject;


        this.knockDownTime = knockDownTime;

        this.knockBackPower = knockBackPower;
        this.isKnockBackOverwrite = isKnockBackOverwrite;
        this.knockBackDirection = knockBackDirection;
    }

    /// <summary>
    /// MonsterAttackData를 받아 AttackInfo에 대응하는 값에 대입합니다.
    /// </summary>
    public AttackInfo(WeaponData weaponData, Vector3 attackDirection, Monster attacker, IHealth hitObject)
        : this(weaponData, attackDirection, attacker, hitObject, new Vector3())
    {
    }

    public AttackInfo(WeaponData weaponData, Vector3 attackDirection, Monster attacker, IHealth hitObject, Vector3 knockBackDirection)
        : this(weaponData.Damage, attackDirection, attacker, hitObject, weaponData.KnockDownTime, weaponData.KnockBackPower, weaponData.IsKnockBackOverwrite, knockBackDirection)
    {
    }    

    #endregion
}