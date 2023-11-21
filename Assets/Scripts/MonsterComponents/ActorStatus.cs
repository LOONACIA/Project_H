using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ActorStatus : MonoBehaviour
{
    public event EventHandler OnShieldChanged;


    [SerializeField]
    [ReadOnly]
    [Tooltip("Hp는 " + nameof(ActorHealth) + "에서 관리됨")]
    private int m_hp;

    [SerializeField]
    [ReadOnly]
    private int m_damage;

    [SerializeField]
    [ReadOnly]
    private bool m_isBlocking;

    [SerializeField]
    [ReadOnly]
    private float m_knockDownTime;

    private Shield m_shield;

    private BehaviorTree m_behaviorTree;



    public int Hp
    {
        get => m_hp;
        set => m_hp = value;
    }

    public int Damage
    {
        get => m_damage;
        set => m_damage = value;
    }

    public bool IsBlocking
    {
        get => m_isBlocking;
        set => m_isBlocking = value;
    }

    public Shield Shield
    {
        get => m_shield;
        set
        {
            // 기존에 생성한 오브젝트 제거
            if (m_shield?.ShieldObject != null)
                Destroy(m_shield.ShieldObject);

            m_shield = value;

            if (m_shield != null) 
                m_shield.OnShieldChanged += ChangeShield;

            OnShieldChanged?.Invoke(this, null);
        }
    }

    public bool IsKnockedDown => m_knockDownTime > 0f;
    public float KnockDownTime

    {
        get => m_knockDownTime;
        private set
        {
            m_knockDownTime = value;
        }
    }

    public void SetKnockDown(float duration)
    {
        if (KnockDownTime > duration) return;
        else
        {
            KnockDownTime = duration;
        }
    }

    private void Awake()
    {
        m_behaviorTree = GetComponent<BehaviorTree>();
        //if (m_behaviorTree)
        //{
        //    SharedVariable holder = m_behaviorTree.GetVariable("KnockDownTime");
        //    m_aiKnockDownTime = holder as SharedFloat;
        //}
    }

    private void Update()
    {
        UpdateKnockDownTime();
        UpdateShield();
    }

    /// <summary>
    /// 남은 기절 시간을 실시간으로 업데이트합니다.
    /// </summary>
    private void UpdateKnockDownTime()
    {
        if (!IsKnockedDown)
        {
            KnockDownTime = 0f;
            return;
        }

        KnockDownTime -= Time.deltaTime;
        if (KnockDownTime < 0.0f)
        {
            KnockDownTime = 0f;
        }
    }

    private void UpdateShield()
    {
        if (Shield == null) return;

        // 쉴드가 더이상 유효하지 않으면 제거
        if (!Shield.IsVaild)
        {
            if (Shield.ShieldObject != null)
                Destroy(Shield.ShieldObject);

            Shield = null;
        }
    }

    private void ChangeShield(object sender, EventArgs e)
    {
        OnShieldChanged?.Invoke(this, null);
    }
}
