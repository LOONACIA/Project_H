using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ActorStatus : MonoBehaviour
{
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
    
    [SerializeField]
    [ReadOnly]
    private bool m_isKnockBack;
    
    private Shield m_shield;

    public int Hp
    {
        get => m_hp;
        set
        {
            if (m_hp != value)
            {
                m_hp = value;
                HpChanged?.Invoke(this, m_hp);
            }
        }
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

            if (m_shield != null)
                m_shield.ShieldChanged -= OnShieldChanged;

            m_shield = value;

            if (m_shield != null) 
                m_shield.ShieldChanged += OnShieldChanged;

            ShieldChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsKnockedDown => m_knockDownTime > 0f;
    //주의: IsKnockBack값의 수정은 Monster류, Actor류 클래스에서만 일어나야함.
    public bool IsKnockBack
    {
        get => m_isKnockBack;
        set => m_isKnockBack = value;   
    }

    public float KnockDownTime
    {
        get => m_knockDownTime;
        private set => m_knockDownTime = value;
    }
    
    public event EventHandler<int> HpChanged;

    public event EventHandler ShieldChanged;

    public void SetKnockDown(float duration)
    {
        if (KnockDownTime > duration) return;
        else
        {
            KnockDownTime = duration;
        }
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
        if (!Shield.IsValid)
        {
            if (Shield.ShieldObject != null)
                Destroy(Shield.ShieldObject);

            Shield = null;
        }
    }

    private void OnShieldChanged(object sender, EventArgs e)
    {
        ShieldChanged?.Invoke(this, EventArgs.Empty);
    }
}
