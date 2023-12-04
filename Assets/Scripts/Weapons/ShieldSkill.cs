using System;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldSkill : Weapon
{
    // 쉴드량
    [SerializeField]
    private float m_shieldPoint;

    // 쉴드 지속 시간
    [SerializeField]
    private float m_shieldTimeLimit;

    // 쉴드 프리팹
    [SerializeField]
    private GameObject m_shieldPrefab;

    // 쉴드 오브젝트 생성 위치
    [SerializeField]
    private Vector3 m_shieldOffset;    

    private Actor m_actor;
    private ActorStatus m_actorStatus;


    protected override void Attack()
    {
        Excute();
    }


    private void Start()
    {
        m_actor = gameObject.GetComponentInParent<Actor>(true);
        m_actorStatus = gameObject.GetComponentInParent<ActorStatus>(true);
    }

    protected override void OnLeadInMotion()
    {
        //Excute(gameObject, EventArgs.Empty);
    }

    // 쉴드 생성 & MonsterStatus로 전달
    private void Excute()
    {
        // TODO : 쉴드 생성 이펙트 실행

        GameObject shieldObject;
        if (m_shieldPrefab != null)
        {
            shieldObject = Instantiate(m_shieldPrefab);
            shieldObject.transform.localPosition = m_shieldOffset;
            shieldObject.transform.SetParent(m_actor.transform);
        }
        else
        {
            shieldObject = new();
        }

        Shield shield = shieldObject.GetOrAddComponent<Shield>();
        if (shield != null)
        {
            shield.Init(m_shieldPoint, m_shieldTimeLimit, shieldObject);
            m_actorStatus.Shield = shield;
        }
    }
}

