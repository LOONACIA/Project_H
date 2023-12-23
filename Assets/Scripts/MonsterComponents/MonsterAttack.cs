using LOONACIA.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * 1인칭, 3인칭 공격을 시전
 */
[RequireComponent(typeof(ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
    private static readonly int s_attackAnimationKey = Animator.StringToHash(ConstVariables.ANIMATOR_PARAMETER_ATTACK);

    private static readonly int s_abilityAnimationKey = Animator.StringToHash(ConstVariables.ANIMATOR_PARAMETER_ABILITY);

    private static readonly int s_targetCheckAnimationKey = Animator.StringToHash(ConstVariables.ANIMATOR_PARAMETER_TARGET_CHECK);

    private Vector3 m_target;

    [SerializeField]
    private MonsterAttackData m_data;

    private Monster m_actor;

    private NavMeshAgent m_agent;
    
    private Ability[] m_abilities;

    private bool m_isAttackTriggered;

    private int m_diedVictimCount = 0;

    [field: SerializeField]
    public AbilityType AbilityType { get; private set; }

    public Vector3 Target
    {
        get => m_target;
        set
        {
            m_target = value;
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Target = value;
            }
        }
    }

    public bool IsAttacking => (CurrentWeapon != null && CurrentWeapon.IsAttacking) || m_isAttackTriggered;

    [field: SerializeField]
    [field: ReadOnly]
    public Weapon CurrentWeapon { get; private set; }

    private void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_abilities = GetComponentsInChildren<Ability>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        m_actor.Status.SkillCoolTime = 1f;

        if (CurrentWeapon != null)
        {
            CurrentWeapon.Owner = m_actor;
            CurrentWeapon.AttackHit -= OnAttackHit;
            CurrentWeapon.AttackHit += OnAttackHit;
        }
    }

    private void OnEnable()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Owner = m_actor;
            CurrentWeapon.AttackHit -= OnAttackHit;
            CurrentWeapon.AttackHit += OnAttackHit;
        }
        
        foreach (Ability ability in m_abilities.AsSpan())
        {
            ability.Owner = m_actor;
            ability.StateChanged += OnAbilityStateChanged;
        }
    }

    private void OnDisable()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.AttackHit -= OnAttackHit;
        }
        
        foreach (Ability ability in m_abilities.AsSpan())
        {
            ability.StateChanged -= OnAbilityStateChanged;
        }
    }

    private void Update()
    {
        UpdateSkillCoolTime();
    }

    public void ChangeWeapon(Weapon weapon)
    {
        if (weapon == CurrentWeapon)
        {
            return;
        }
        
        if (CurrentWeapon != null)
        {
            CurrentWeapon.AttackHit -= OnAttackHit;
            CurrentWeapon.StateChanged -= OnWeaponStateChanged;
            CurrentWeapon.UnEquip();
        }
        
        CurrentWeapon = weapon;
        
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Owner = m_actor;
            CurrentWeapon.AttackHit -= OnAttackHit;
            CurrentWeapon.AttackHit += OnAttackHit;
            CurrentWeapon.Equip(m_actor);
            CurrentWeapon.StateChanged += OnWeaponStateChanged;
        }

        m_isAttackTriggered = false;
    }

    public void Attack()
    {
        //TODO: KnockBack, KnockDown 중 공격 못하게 할 것인가?
        if (!CurrentWeapon.CanAttack)
        {
            return;
        }

        if (!m_actor.IsPossessed)
        {
            //3인칭인 경우 Avoidance값을 높임
            m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        m_isAttackTriggered = true;
        m_actor.Animator.SetTrigger(s_attackAnimationKey);
    }

    public void Ability(bool isToggled)
    {
        //TODO: KnockBack, KnockDown 중 스킬 못하게 할 것인가?
        if (m_actor.Status.SkillCoolTime < 1f)
        {
            return;
        }

        switch (AbilityType)
        {
            case AbilityType.Trigger when isToggled:
                m_actor.Animator.SetTrigger(s_abilityAnimationKey);
                break;
            case AbilityType.Toggle:
                m_actor.Animator.SetBool(s_abilityAnimationKey, isToggled);
                break;
            default:
                return;
        }
    }

    private void OnWeaponStateChanged(object sender, WeaponState e)
    {
        m_isAttackTriggered = false;

        if(e == WeaponState.Attack)
        {
            //공격 스테이트로 진입 시, 타격 수 count 초기화
            m_diedVictimCount = 0;
        }
    }
    
    private void OnAbilityStateChanged(object sender, AbilityState e)
    {
        if (m_data.SkillCoolTime == 0)
        {
            return;
        }

        if (AbilityType == AbilityType.Trigger && e == AbilityState.Activate)
        {
            m_actor.Status.SkillCoolTime = 0f;
        }
    }
            
    private void OnAttackHit(object sender, IEnumerable<AttackInfo> e)
    {
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);

        //타격 이펙트, 3명 이상이 죽었을 경우 흔듭니다.
        foreach(var info in e)
        {
            if (info.Victim.CurrentHp <= 0f)
            {
                //Debug.Log("target count: " + targetCount);
                m_diedVictimCount += 1;
                if (m_diedVictimCount >= 3)
                {
                    GameManager.Effect.ChangeTimeScale(this, 0.3f, 0.5f);
                }
            }
        }
    }

    private void OnValidate()
    {
        if (m_data == null)
        {
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
        }
    }

    private void UpdateSkillCoolTime()
    {
        float coolTime = m_data.SkillCoolTime;
        if (coolTime <= 0f)
        {
            return;
        }
        
        if (m_actor.Status.SkillCoolTime < 1f)
        {
            //TODO: 부동소수점, deltaTime 이슈로 실제 시간과 작은 오차 발생 가능, 해결 필요한지 확인
            float cur = m_actor.Status.SkillCoolTime * coolTime + Time.deltaTime;

            //TODO: 1/full 미리 계산해두기(최적화)
            m_actor.Status.SkillCoolTime = cur / coolTime;
        }
    }
}

public enum AbilityType
{
    Trigger,
    Toggle
}