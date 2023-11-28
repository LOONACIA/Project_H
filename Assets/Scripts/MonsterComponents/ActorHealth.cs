using System;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(ActorStatus))]
public class ActorHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private MonsterHealthData m_data;

    [SerializeField]
    private VisualEffect hitVfx;

    [SerializeField]
    private int particleCoef = 7;

    private Monster m_actor;

    private ActorStatus m_status;

    public event EventHandler<DamageInfo> Damaged;

    public event EventHandler<DamageInfo> Dying;

    public event EventHandler Died;

    public int CurrentHp => m_status.Hp;

    public int MaxHp => m_data.MaxHp;

    public bool IsDead => CurrentHp <= 0;
    
    protected void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<ActorStatus>();
        m_status.Hp = m_data.MaxHp;
    }

    private void OnEnable()
    {
        // var receiver = GetComponentInChildren<DeathAnimationEventReceiver>();
        // if (receiver != null)
        // {
        //     receiver.DeathAnimationEnd += OnDied;
        // }
    }

    private void OnDisable()
    {
        // var receiver = GetComponentInChildren<DeathAnimationEventReceiver>();
        // if (receiver != null)
        // {
        //     receiver.DeathAnimationEnd -= OnDied;
        // }
    }

    public void TakeDamage(DamageInfo info)
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

        // 쉴드를 가지고 있는 경우에 체력 대신 쉴드량 감소
        if (m_status.Shield != null)
        {
            m_status.Shield.TakeDamage(info.Damage);
        }
        else
        {
            // 몬스터가 피격시 애니메이션 실행 
            if (!m_actor.IsPossessed)
            {
                PlayHitAnimation(info.AttackDirection, info.Attacker);
            }

            m_status.Hp -= info.Damage;
        }

        OnDamaged(info);
    }

    public void Kill()
    {
        //강제 사망처리 코드
        m_status.Hp = 0;
        OnDamaged(new DamageInfo(0, Vector3.zero, Vector3.zero, null));
    }

    private void OnDamaged(DamageInfo info)
    {
        Damaged?.Invoke(this, info);

        PlayVfx(info);
        if (IsDead)
        {
            bool hasAnimation = m_actor.Animator.parameters.Any(param => param.name == "Dead");
            if (hasAnimation)
            {
                m_actor.Animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_DEAD);
            }
            OnDying(info);
            OnDied();
        }
    }

    private void OnDying(DamageInfo info)
    {
        Dying?.Invoke(this, info);
    }

    private void OnDied()
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

    private void PlayHitAnimation(Vector3 attackDirection = new Vector3(), Actor attacker = null)
    {
        if (attacker != null)
        {
            var hitDirectionX = m_actor.transform.InverseTransformDirection(attackDirection);
            var hitDirectionZ =
                m_actor.transform.InverseTransformDirection((m_actor.transform.position - attacker.transform.position)
                    .normalized);

            if (hitDirectionZ.z > 0)
            {
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_X, 0);
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_Z, hitDirectionZ.z);
            }
            else
            {
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_X,
                    hitDirectionX.x >= 0 ? 1 : -1);
                m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_HIT_DIRECTION_Z, 0);
            }
        }
    }

    private void PlayBlockAnimation()
    {
        m_actor.Animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
        m_actor.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_BLOCK_IMPACK_INDEX, UnityEngine.Random.Range(0, 3));
    }

    private void PlayVfx(DamageInfo damage)
    {
        if (hitVfx != null)
        {
            hitVfx.SetInt(ConstVariables.VFX_GRAPH_PARAMETER_PARTICLE_COUNT, damage.Damage * particleCoef);
            hitVfx.SetVector3(ConstVariables.VFX_GRAPH_PARAMETER_DIRECTION, damage.AttackDirection);
            hitVfx.transform.position = damage.HitPosition;
            hitVfx.SendEvent(ConstVariables.VFX_GRAPH_EVENT_ON_PLAY);
        }
    }
}