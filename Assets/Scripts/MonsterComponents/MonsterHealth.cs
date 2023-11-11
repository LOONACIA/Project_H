using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MonsterStatus))]
public class MonsterHealth : MonoBehaviour, IHealth
{
    private static readonly int s_deadAnimationKey = Animator.StringToHash("Dead");
    
    [SerializeField]
    private MonsterHealthData m_data;

    private Monster m_actor;

    private MonsterStatus m_status;

    public event EventHandler<Actor> Damaged;

    public event EventHandler Dying;

    public event EventHandler Died;

    public int CurrentHp => m_status.Hp;

    public int MaxHp => m_data.MaxHp;

    public bool IsDead => CurrentHp <= 0;

    protected void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<MonsterStatus>();
    }

    protected virtual void Start()
    {
        m_status.Hp = m_data.MaxHp;
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
    
    public void TakeDamage(int damage, Actor attacker)
    {
        if (IsDead)
        {
            return;
        }

        m_status.Hp -= damage;
        OnDamaged(attacker);
    }

    public void Kill()
    {
        m_status.Hp = 0;
        OnDamaged(null);
    }
    
    private void OnDamaged(Actor attacker)
    {
        Damaged?.Invoke(this, attacker);
        
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
