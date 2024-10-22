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
    private int particleCoef = 3;

    private Monster m_actor;

    private ActorStatus m_status;

    [Tooltip("Block이 가능한 범위를 나타냅니다. x-z평면 기준")]
    [SerializeField]
    private float m_blockAngle = 180f;

    public event RefEventHandler<AttackInfo> Damaged;

    public event RefEventHandler<AttackInfo> Blocked;

    public event RefEventHandler<AttackInfo> Dying;

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
    
    public void TakeDamage(in AttackInfo info)
    {
        if (IsDead)
        {
            return;
        }

        // 방어 모션 중에 공격 받을 시 데미지 무효, 충격 받는 모션 실행
        if (m_status.IsBlocking && CheckBlockDirection(info))
        {
            PlayBlockAnimation();
            Blocked?.Invoke(this, info);

            //사운드 출력
            PlayShieldSound();
            return;
        }

        // 쉴드를 가지고 있는 경우에 데미지 무효
        if (m_status.Shield != null)
        {
            m_status.Shield.TakeDamage(info);
            OnDamaged(info);
            return;
        }

        // 피격 모션 실행 
        if (!m_actor.IsPossessed)
        {
            PlayHitAnimation(info.AttackDirection, info.Attacker);

            //Sound 출력
            PlayHitSound();
        } 
        else
        {
            // 플레이어 피격시 이펙트 실행
            GameManager.Effect.ShowHitVignetteEffect();
        }

        m_status.Hp -= info.Damage;
        
        OnDamaged(info);
    }

    public void Kill()
    {
        //강제 사망처리 코드
        m_status.Hp = 0;
        OnDamaged(new(null, this, MaxHp, Vector3.zero, Vector3.zero));
    }
    
    private void OnDamaged(in AttackInfo info)
    {
        Damaged?.Invoke(this, info);

        PlayHitVfx(info);

        if (IsDead)
        {
            OnDying(info);
            bool hasAnimation = m_actor.Animator.parameters.Any(param => param.name == "Dead");
            if (hasAnimation)
            {
                m_actor.Animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_DEAD);
            }
            // Dead 사운드출력
            PlayDeadSound();

            OnDied();
        }
    }

    private void OnDying(AttackInfo info)
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

    private void PlayHitAnimation(Vector3 attackDirection, GameObject attacker)
    {
        if (attacker != null)
        {
            var hitDirectionX = transform.InverseTransformDirection(attackDirection);
            var hitDirectionZ =
                m_actor.transform.InverseTransformDirection((transform.position - attacker.transform.position)
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

    private void PlayHitVfx(in AttackInfo damage)
    {
        if (hitVfx != null)
        {
            hitVfx.SetInt(ConstVariables.VFX_GRAPH_PARAMETER_PARTICLE_COUNT, damage.Damage * particleCoef);
            hitVfx.SetVector3(ConstVariables.VFX_GRAPH_PARAMETER_DIRECTION, damage.AttackDirection);
            hitVfx.transform.position = damage.HitPoint;
            hitVfx.SendEvent(ConstVariables.VFX_GRAPH_EVENT_ON_PLAY);
        }
    }

    private bool CheckBlockDirection(in AttackInfo info)
    {
        //대상과 나의 x-z 2차원 좌표를 기준으로 체크합니다.
        //내가 보고있는 방향을 기준으로 각도를 체크합니다.
        //LOL의 판테온과 거의 같은 로직
        Vector3 dir = info.Attacker.transform.position - transform.position;
        dir.y = 0f;

        Vector3 front = transform.forward;
        front.y = 0f;

        //front 벡터와 공격받은 벡터가 지정한 각도값 내에 있을경우 Block
        return Vector3.Angle(front, dir) < m_blockAngle * 0.5f;
    }

    #region 사운드 출력
    private MonsterSFXPlayer GetSFX()
    {
        return GetComponent<Monster>().m_thirdPersonAnimator.GetComponent<MonsterSFXPlayer>();
    }
    
    private void PlayHitSound()
    {
        int num = UnityEngine.Random.Range(0, 3);

        MonsterSFXPlayer sfx = GetSFX();

        switch (num) 
        {
            case 0:
                GameManager.Sound.PlayClipAt(sfx.monsterSFX.Hit1, transform.position);
                break;

            case 1:
                GameManager.Sound.PlayClipAt(sfx.monsterSFX.Hit2, transform.position);
                break;

            case 2:
                GameManager.Sound.PlayClipAt(sfx.monsterSFX.Hit3, transform.position);
                break;
        }
    }

    private void PlayShieldSound()
    {
        int num = UnityEngine.Random.Range(0, 3);

        MonsterSFXPlayer sfx = GetSFX();

        switch (num)
        {
            case 0:
                sfx.OnPlayShield1();
                break;

            case 1:
                sfx.OnPlayShield2();
                break;

            case 2:
                sfx.OnPlayShield3();
                break;
        }   
    }

    private void PlayDeadSound()
    {
        if (!m_actor.IsPossessed)
        {
            PlayerTPDeadSound();
        }
    }

    private void PlayerTPDeadSound()
    {
        MonsterSFXPlayer sfx = GetSFX();

        int num = UnityEngine.Random.Range(0, 3);

        switch (num)
        {
            case 0:
                GameManager.Sound.PlayClipAt(sfx.monsterSFX.TPDeath1, transform.position);
                break;

            case 1:
                GameManager.Sound.PlayClipAt(sfx.monsterSFX.TPDeath2, transform.position);
                break;

            case 2:
                GameManager.Sound.PlayClipAt(sfx.monsterSFX.TPDeath3, transform.position);
                break;
        }
    }
    #endregion
}