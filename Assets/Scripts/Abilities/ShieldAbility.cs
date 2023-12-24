using LOONACIA.Unity;
using System;
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
            shieldObject = new() { name = "Shield" };
            shieldObject.transform.SetParent(m_shieldParent);
        }

        Shield shield = shieldObject.GetOrAddComponent<Shield>();
        if (shield != null)
        {
            Owner.Status.AbilityRate = 1;
            shield.Init(m_shieldAmount, m_shieldDuration);
            shield.ShieldChanged += OnShieldChanged;
            shield.Destroyed += OnShieldDestroyed;
            Owner.Status.Shield = shield;
        }
    }

    private void OnShieldDestroyed(object sender, EventArgs e)
    {
        if (sender is not Shield shield)
        {
            return;
        }
        
        Owner.Status.AbilityRate = 0;
        shield.ShieldChanged -= OnShieldChanged;
        shield.Destroyed -= OnShieldDestroyed;
    }

    private void OnShieldChanged(object sender, float e)
    {
        Owner.Status.AbilityRate = e;
    }
}