using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MonsterStatus))]
public class MonsterHealth : MonoBehaviour, IHealth
{
    private static readonly int s_deadAnimationKey = Animator.StringToHash("Dead");
    
    [SerializeField]
    private MonsterHealthData m_data;

    private Monster m_actor;

    private MonsterStatus m_status;

    public event EventHandler<int> HealthChanged;

    public event EventHandler Dying;

    public event EventHandler Died;

    public int CurrentHp
    {
        get => m_status.Hp;
        set
        {
            var diff = value - m_status.Hp;
            m_status.Hp = Mathf.Clamp(value, 0, m_data.MaxHp);
            OnHealthChanged(diff);
        }
    }

    public int MaxHp => m_data.MaxHp;

    public bool IsDead => CurrentHp <= 0;

    protected void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<MonsterStatus>();
    }

    protected virtual void Start()
    {
        CurrentHp = m_data.MaxHp;
    }

    private void OnEnable()
    {
        var receiver = GetComponentInChildren<DeathAnimationEventReceiver>();
        if (receiver != null)
        {
            receiver.DeathAnimationEnd += OnDied;
        }
    }

    private void OnDisable()
    {
        var receiver = GetComponentInChildren<DeathAnimationEventReceiver>();
        if (receiver != null)
        {
            receiver.DeathAnimationEnd -= OnDied;
        }
    }

    public void Kill()
    {
        CurrentHp = 0;
    }
    
    private void OnHealthChanged(int amount)
    {
        HealthChanged?.Invoke(this, amount);
        if (amount < 0)
        {
            GameManager.Effect.PlayBloodEffect(gameObject, transform.rotation, 0.5f);
        }

        if (IsDead)
        {
            Dying?.Invoke(this, EventArgs.Empty);
            bool hasAnimation = m_actor.Animator.parameters.Any(param => param.name == "Dead");
            if (hasAnimation)
            {
                m_actor.Animator.SetTrigger(s_deadAnimationKey);
            }
        }
    }
    
    private void OnDied(object sender, EventArgs e)
    {
        Death();
    }

    private void Death()
    {
        Died?.Invoke(this, EventArgs.Empty);
    }

    private void OnValidate()
    {
        if (m_data == null)
        {
            Debug.LogWarning($"{nameof(m_data)} is null");
        }
    }
}
