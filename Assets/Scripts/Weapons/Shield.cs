using System;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class Shield : MonoBehaviour
{
    // 쉴드량
    [SerializeField]
    private float m_shieldAmount = 30;

    // 쉴드 제한 시간 여부
    [SerializeField]
    private bool m_hasTimeLimit;

    // 쉴드 제한 시간
    [SerializeField]
    private float m_timeLimit;

    // 쉴드 생성한 시간
    private float m_startTime;

    [SerializeField]
    private VisualEffect hitVfx;

    private Breakable[] m_breakables;

    private GameObject m_bodyPartCollector;

    // 남은 쉴드량
    public float ShieldAmount
    {
        get => Mathf.Max(m_shieldAmount, 0);
        private set => m_shieldAmount = value;
    }

    public float MaxShieldAmount { get; private set; }

    public bool IsValid
    {
        get
        {
            // 쉴드 시간 제한이 있고, 지속 시간보다 오랫동안 존재했을 경우 false 반환
            if (m_hasTimeLimit && Time.time - m_startTime > m_timeLimit)
            {
                return false;
            }

            // 쉴드량을 모두 소모했을 경우 false 반환
            return ShieldAmount > 0;
        }
    }
    
    public event EventHandler<float> ShieldChanged;
    
    public event EventHandler Destroyed;

    public void Init(float shieldPoint, float timeLimit)
    {
        MaxShieldAmount = shieldPoint;
        ShieldAmount = MaxShieldAmount;
        m_timeLimit = timeLimit;

        // 쉴드 제한 시간이 0 이하면, 제한 시간이 없는 것으로 처리
        m_hasTimeLimit = timeLimit > 0;

        m_startTime = Time.time;
    }

    public void TakeDamage(in AttackInfo damageInfo)
    {
        ShieldAmount -= damageInfo.Damage;
        PlayHitVfx(damageInfo);
        if (ShieldAmount <= 0)
        {
            Destroy(damageInfo);
        }
        
        float percent = ShieldAmount / MaxShieldAmount;
        ShieldChanged?.Invoke(this, percent);
    }

    public void Destroy(in AttackInfo damageInfo)
    {
        // SFX
        GameManager.Sound.PlayClipAt(GameManager.Sound.ObjectDataSounds.BreakShield, transform.position);

        if (damageInfo.Attacker != null)
        {
            ReplaceParts(damageInfo);
        }
        
        Destroyed?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    private void Awake()
    {
        Init(m_shieldAmount, m_timeLimit);

        m_breakables = GetComponentsInChildren<Breakable>(true);

        m_bodyPartCollector = GameObject.Find("BodyPartCollector");
        if (m_bodyPartCollector == null)
        {
            m_bodyPartCollector = new() { name = "BodyPartCollector" };
        }
    }

    private void Update()
    {
        if (m_hasTimeLimit && Time.time - m_startTime > m_timeLimit)
        {
            Destroy(new ());
        }
    }

    private void PlayHitVfx(in AttackInfo damage)
    {
        if (hitVfx != null)
        {
            hitVfx.SetInt(ConstVariables.VFX_GRAPH_PARAMETER_PARTICLE_COUNT, damage.Damage * 2);
            hitVfx.SetVector3(ConstVariables.VFX_GRAPH_PARAMETER_DIRECTION, damage.AttackDirection);
            hitVfx.transform.position = damage.HitPoint;
            hitVfx.SendEvent(ConstVariables.VFX_GRAPH_EVENT_ON_PLAY);
        }
    }

    private void ReplaceParts(in AttackInfo damageInfo)
    {
        foreach (var breakable in m_breakables)
        {
            breakable.gameObject.SetActive(true);

            breakable.ReplacePart(damageInfo, 0.8f);
            breakable.transform.SetParent(m_bodyPartCollector.transform);
        }

        gameObject.SetActive(false);
    }
}