using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ActorStatus))]
public class ActorHealth : MonoBehaviour, IHealth
{
    private static readonly int s_deadAnimationKey = Animator.StringToHash("Dead");

    private static readonly int s_hitAnimationKey = Animator.StringToHash("Hit");

    private static readonly int s_blockImpactIndexAnimationKey = Animator.StringToHash("BlockImpactIndex");

    private static readonly int s_hitDirectionXAnimationKey = Animator.StringToHash("HitDirectionX");

    private static readonly int s_hitDirectionYAnimationKey = Animator.StringToHash("HitDirectionY");
    
    [SerializeField]
    private MonsterHealthData m_data;

    private Monster m_actor;

    private ActorStatus m_status;

    public event EventHandler<Actor> Damaged;

    public event EventHandler Dying;

    public event EventHandler Died;

    public int CurrentHp => m_status.Hp;

    public int MaxHp => m_data.MaxHp;

    public bool IsDead => CurrentHp <= 0;

    protected void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<ActorStatus>();
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

        // 방어 모션 중에 공격 받을 시, 데미지 계산 스킵 & 충격 받는 모션 실행
        if (m_status.IsBlocking) 
        {
            PlayBlockAnimation();
            return;
        }

        // 몬스터가 데미지를 입을 시, 피격 애니메이션 실행
        if (!m_actor.IsPossessed)
        {
            PlayHitAnimation();
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
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
        }
    }

    private void PlayBlockAnimation()
    {
        m_actor.Animator.SetTrigger(s_hitAnimationKey);
        m_actor.Animator.SetFloat(s_blockImpactIndexAnimationKey, UnityEngine.Random.Range(0, 3));
    }

    private void PlayHitAnimation()
    {
        // temp
        bool isKnockDown = false;
        Vector2 tempVec = Random.insideUnitCircle;
        tempVec.x = tempVec.x > 0 ? 1 : -1;
        tempVec.y = tempVec.y > 0 ? 1 : -1;

        if (isKnockDown)
        {
            m_actor.Animator.SetTrigger("KnockDown");
        }
        else
        { 
            m_actor.Animator.SetFloat(s_hitDirectionXAnimationKey, tempVec.x);
            m_actor.Animator.SetFloat(s_hitDirectionYAnimationKey, tempVec.y);
        }

        m_actor.Animator.SetTrigger(s_hitAnimationKey);
    }
}
