using LOONACIA.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAbility : Ability
{
    [SerializeField]
    [Tooltip("쉴드 프리팹")]
    private GameObject m_shieldPrefab;
    
    [SerializeField]
    [Tooltip("쉴드 프리팹이 생성될 부모 오브젝트")]
    private Transform m_shieldParent;

    [SerializeField]
    [Tooltip("쉴드 오브젝트 생성 위치")]
    private Vector3 m_shieldOffset;
    
    [SerializeField]
    [Tooltip("쉴드량")]
    private float m_shieldAmount;
    
    [SerializeField]
    [Tooltip("쉴드 지속 시간")]
    private float m_shieldDuration;

    protected override void OnActivateState()
    {
        base.OnActivateState();
        
        CreateShield();
    }

    private void CreateShield()
    {
        GameObject shieldObject;
        if (m_shieldPrefab != null)
        {
            shieldObject = Instantiate(m_shieldPrefab, m_shieldParent);
            shieldObject.transform.localPosition = m_shieldOffset;
        }
        else
        {
            shieldObject = new();
        }

        Shield shield = shieldObject.GetOrAddComponent<Shield>();
        if (shield != null)
        {
            shield.Init(m_shieldAmount, m_shieldDuration, shieldObject);
            Owner.Status.Shield = shield;
        }
    }
}