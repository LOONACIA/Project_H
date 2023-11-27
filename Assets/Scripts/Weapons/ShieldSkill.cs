using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShieldSkill : Weapon
{
    // 쉴드 지속 시간
    [SerializeField]
    private float m_shieldPoint;

    // 쉴드량
    [SerializeField]
    private float m_shieldDuration;

    // 쉴드 프리팹
    [SerializeField]
    private GameObject m_shieldPrefab;

    // 쉴드 오브젝트 생성 위치
    [SerializeField]
    private Vector3 m_shieldOffset;

    private Actor m_actor;
    private ActorStatus m_actorStatus;

    private void Start()
    {
        m_actor = gameObject.GetComponentInParent<Actor>(true);
        m_actorStatus = gameObject.GetComponentInParent<ActorStatus>(true);
    }

    protected override void Attack()
    {
        // todo : 쉴드 생성 이펙트 실행

        // 쉴드 프리팹 생성 & MonsterStatus로 전달
        if (m_shieldPrefab != null)
        {
            GameObject shieldObject = Instantiate(m_shieldPrefab);
            shieldObject.transform.SetParent(m_actor.transform);
            shieldObject.transform.localPosition = m_shieldOffset;
            
            m_actorStatus.Shield = new Shield(m_shieldPoint, m_shieldDuration, shieldObject);
        }
        else
        {
            m_actorStatus.Shield = new Shield(m_shieldPoint, m_shieldDuration);
        }
    }
}

public class Shield
{
    // 쉴드 생성한 시간
    private float m_startTime;

    private float m_shieldPoint;
    
    // 쉴드 지속 시간
    public float ShieldDuration { get; private set; }

    // 쉴드 프리팹
    public GameObject ShieldObject { get; private set; }

    // 남은 쉴드량
    public float ShieldPoint 
    { 
        get => Mathf.Max(m_shieldPoint, 0); 
        private set => m_shieldPoint = value; 
    }

    public float MaxShieldPoint { get; private set; }
    
    public bool IsValid => Time.time - m_startTime < ShieldDuration && ShieldPoint > 0;

    public event EventHandler ShieldChanged;

    public Shield(float shieldPoint, float shieldDuration, GameObject shieldObject = null)
    {
        MaxShieldPoint = shieldPoint;
        ShieldPoint = MaxShieldPoint;
        ShieldDuration = shieldDuration;

        if (shieldObject != null) 
            ShieldObject = shieldObject;

        m_startTime = Time.time;
    }

    public void TakeDamage(float damage)
    {
        ShieldPoint -= damage;
        ShieldChanged?.Invoke(this, null);
    }
}
