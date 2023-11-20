using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ActorStatus))]
public class ActorHealth : MonoBehaviour, IHealth
{
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

    public void TakeDamage(int damage, Vector3 attackDirection, Actor attacker)
    {
        if (IsDead)
        {
            return;
        }

        // 방어 모션 중에 공격 받을 시 데미지 무효, 충격 받는 모션 실행
        if (m_status.IsBlocking)
        {
            PlayBlockAnimation();
            return;
        }

        // 피격 모션 실행 
        if (!m_actor.IsPossessed)
        {
            PlayHitAnimation(attackDirection, attacker);
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
                m_actor.Animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_DEAD);
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

    private void PlayHitAnimation(Vector3 attackDirection = new Vector3(), Actor attacker = null)
    {
        if (attacker != null)
        {
            var hitDirectionX = m_actor.transform.InverseTransformDirection(attackDirection);
            var hitDirectionZ = m_actor.transform.InverseTransformDirection((m_actor.transform.position - attacker.transform.position).normalized);

            if (hitDirectionZ.z > 0)
            {
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_X, 0);
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_Z, hitDirectionZ.z);
            }
            else
            { 
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_X, hitDirectionX.x >= 0 ? 1 : -1);
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_Z, 0);
            }
        }

        //m_actor.Animator.SetTrigger("Hit");
    }

    private void PlayBlockAnimation()
    {
        m_actor.Animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
        m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_BLOCK_IMPACK_INDEX, UnityEngine.Random.Range(0, 3));
    }
}
