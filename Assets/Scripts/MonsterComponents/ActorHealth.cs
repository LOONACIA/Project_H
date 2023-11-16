using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(ActorStatus))]
public class ActorHealth : MonoBehaviour, IHealth
{
    private static readonly int s_deadAnimationKey = Animator.StringToHash("Dead");

    private static readonly int s_hitAnimationKey = Animator.StringToHash("Hit");

    private static readonly int s_blockImpactIndexAnimationKey = Animator.StringToHash("BlockImpactIndex");
    
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
    
    
    //데미지 리스트 테스트용입니다.
    private List<Vector3> m_damagedDirectionList = new();

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

    public void TakeDamage(AttackInfo attackInfo, Actor attacker)
    {
        if (IsDead)
        {
            return;
        }

        //공격 방향 테스트용 코드, Gizmo에서 사용합니다.
        m_damagedDirectionList.Add(attackInfo.attackDirection);

        // 방어 모션 중에 공격 받을 시 데미지 무효, 충격 받는 모션 실행
        if (m_status.IsBlocking)
        {
            PlayBlockAnimation();
            return;
        }

        // 피격 모션 실행 
        //if (!m_actor.IsPossessed)
        //{
        //    PlayHitAnimation(attackInfo, attacker);
        //}

        m_status.Hp -= attackInfo.damage;
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


    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        
        foreach (var t in m_damagedDirectionList)
        {  
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position -t + Vector3.up ,transform.position + (t) + Vector3.up);
        }
    }

    private void PlayHitAnimation(AttackInfo attackInfo = null, Actor attacker = null)
    {
        if (attackInfo != null && attacker != null)
        {
            var hitDirectionX = m_actor.transform.InverseTransformDirection(attackInfo.attackDirection);
            var hitDirectionZ = m_actor.transform.InverseTransformDirection((m_actor.transform.position - attacker.transform.position).normalized);

            if (hitDirectionZ.z > 0)
            {
                m_actor.Animator.SetFloat("HitDirectionX", 0);
                m_actor.Animator.SetFloat("HitDirectionZ", hitDirectionZ.z);
            }
            else
            { 
                m_actor.Animator.SetFloat("HitDirectionX", hitDirectionX.x >= 0 ? 1 : -1);
                m_actor.Animator.SetFloat("HitDirectionZ", 0);
            }
        }

        //m_actor.Animator.SetTrigger("Hit");
    }

    private void PlayBlockAnimation()
    {
        m_actor.Animator.SetTrigger(s_hitAnimationKey);
        m_actor.Animator.SetFloat(s_blockImpactIndexAnimationKey, UnityEngine.Random.Range(0, 3));
    }
}
