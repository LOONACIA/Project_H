using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(WeaponData), menuName = "Data/" + nameof(WeaponData))]
public class WeaponData : ScriptableObject
{
    #region PrivateFields
    [SerializeField]
    private int m_damage;

    #region KnockDown

    [Header("KnockDown")]
    [SerializeField]
    private float m_knockDownTime;

    #endregion

    #region KnockBack

    [Header("KnockBack")]
    [SerializeField]
    private float m_knockBackPower;
    
    [SerializeField]
    private bool m_isKnockBackOverwrite;

    [SerializeField]
    private Vector3 m_knockBackDirection;

    #endregion
    
    #endregion

    #region PublicProperties

    #region Damage

    public int Damage => m_damage;

    #endregion

    #region KnockDown

    public float KnockDownTime => m_knockDownTime;

    #endregion

    #region KnockBack
    
    public float KnockBackPower => m_knockBackPower;
    public bool IsKnockBackOverwrite => m_isKnockBackOverwrite;
    public Vector3 KnockBackDirection => m_knockBackDirection;

    #endregion

    #endregion
}
