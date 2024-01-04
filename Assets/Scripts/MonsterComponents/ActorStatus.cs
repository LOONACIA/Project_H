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
    private bool m_isBlocking;

    [SerializeField]
    [ReadOnly]
    [Tooltip("CanKnockBack이 false면 넉백되지 않습니다.")]
    private bool m_canKnockBack = true;
    
    [SerializeField]
    [ReadOnly]
    [Tooltip("CanKnockDown이 false면 넉다운되지 않습니다.")]
    private bool m_canKnockDown = true;

    [SerializeField]
    [ReadOnly]
    private float m_knockDownTime;

    [SerializeField]
    [ReadOnly]
    [Tooltip("JumpPad로 인해 공중에 떠있는 상태")]
    private bool m_isFlying;
    
    [SerializeField]
    [ReadOnly]
    private float m_abilityRate;

    [SerializeField]
    [ReadOnly]
    private bool m_isKnockBack;

    [SerializeField]
    [ReadOnly]
    private Shield m_shield;

    [SerializeField]
    [ReadOnly]
    private float m_skillCoolTime = 1;
    
    [FormerlySerializedAs("m_dashCount")]
    [SerializeField]
    [ReadOnly]
    private int m_currentDashCount;
    
    private float m_dashCooldownCounter;

    public int Hp
    {
        get => m_hp;
        set
        {
            if (m_hp != value)
            {
                m_hp = Mathf.Max(value, 0);
                HpChanged?.Invoke(this, m_hp);
            }
        }
    }
    
    public bool CanKnockBack
    {
        get => m_canKnockBack;
        set => m_canKnockBack = value;
    }
    public bool CanKnockDown
    {
        get => m_canKnockDown;
        set => m_canKnockDown = value;
    }

    public float AbilityRate
    {
        get => m_abilityRate;
        set
        {
            m_abilityRate = Mathf.Clamp(value, 0, 1);
            AbilityRateChanged?.Invoke(this, m_abilityRate);
        }
    }

    [field: SerializeField]
    [field: ReadOnly]
    public bool IsStunned { get; set; }

    public bool IsBlocking
    {
        get => m_isBlocking;
        set => m_isBlocking = value;
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

    public bool IsFlying
    {
        get => m_isFlying;
        set => m_isFlying = value;
    }

    /// <summary>
    /// 상태이상 중 하나라도 걸려있는지 확인
    /// </summary>
    public bool IsInCrowdControl
    {
        get => IsStunned || IsKnockedDown || IsKnockBack || IsFlying;
    }

    public Shield Shield
    {
        get => m_shield;
        set
        {
            // 기존에 생성한 오브젝트 제거
            if (m_shield != null)
            {
                m_shield.ShieldChanged -= ShieldChanged;
                m_shield.Destroyed -= ShieldDestroyed;
                m_shield.Destroy(new());
            }

            m_shield = value;
            
            if (m_shield != null)
            {
                m_shield.ShieldChanged += ShieldChanged;
                m_shield.Destroyed += ShieldDestroyed;
                ShieldChanged?.Invoke(m_shield, m_shield.ShieldAmount / m_shield.MaxShieldAmount);
            }
        }
    }

    public bool HasCooldown { get; set; }

    /// <summary>
    /// 0~1로 표현되며, 1일때 스킬 사용 가능.
    /// </summary>
    public float SkillCoolTime
    {
        get => m_skillCoolTime;
        set
        {
            m_skillCoolTime = Mathf.Clamp(value, 0, 1);
            SkillCoolTimeChanged?.Invoke(this, m_skillCoolTime);
        }
    }
    
    public int MaxDashCount { get; set; }

    public int CurrentDashCount
    {
        get => m_currentDashCount;
        set
        {
            if (m_currentDashCount == value)
            {
                return;
            }
            
            m_currentDashCount = Mathf.Clamp(value, 0, MaxDashCount);
            if (m_currentDashCount == MaxDashCount - 1)
            {
                m_dashCooldownCounter = 0;
            }
            
            DashCountChanged?.Invoke(this, m_currentDashCount);
        }
    }

    public float DashCoolTime { get; set; }

    public event EventHandler<int> HpChanged;
    
    public event EventHandler<float> ShieldChanged; 
    
    public event EventHandler ShieldDestroyed;
    
    public event EventHandler<float> AbilityRateChanged;

    public event EventHandler<float> SkillCoolTimeChanged;
    
    public event EventHandler<int> DashCountChanged;
    
    public event EventHandler<float> DashCoolTimeChanged;

    public void SetKnockDown(float duration)
    {
        if (KnockDownTime > duration)
        {
            return;
        }
        
        KnockDownTime = duration;
    }

    private void Update()
    {
        UpdateKnockDownTime();
        UpdateShield();
        UpdateDashCoolDown();
    }
    
    private void UpdateDashCoolDown()
    {
        m_dashCooldownCounter = Mathf.Clamp(m_dashCooldownCounter + Time.deltaTime, 0, DashCoolTime);
        if (MaxDashCount != CurrentDashCount)
        {
            DashCoolTimeChanged?.Invoke(this, m_dashCooldownCounter / DashCoolTime);
        }

        if (MaxDashCount > CurrentDashCount && m_dashCooldownCounter >= DashCoolTime)
        {
            //최대 대쉬 카운트가 아니면서
            //마지막 대쉬 시간부터 쿨타임이 지났다면 대쉬 카운트 +1
            CurrentDashCount += 1;
            
            m_dashCooldownCounter = 0;
        }
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
        if (Shield == null)
        {
            return;
        }

        // 쉴드가 더이상 유효하지 않으면 제거
        if (!Shield.IsValid)
        {
            Shield.Destroy(new());

            Shield = null;
        }
    }
}
