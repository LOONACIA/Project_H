using LOONACIA.Unity;
using System;
using UnityEngine;

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
    
    [field: SerializeField]
    public AbilityType AbilityType { get; private set; }

    public Vector3 Target
    {
        get => m_target;
        set
        {
            m_target = value;
            CurrentWeapon.Target = value;
        }
    }

    public bool IsAttacking => CurrentWeapon != null && CurrentWeapon.IsAttacking;

    [field: SerializeField]
    [field: ReadOnly]
    public Weapon CurrentWeapon { get; private set; }

    private void Awake()
    {
        m_actor = GetComponent<Monster>();
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
    }

    private void OnDisable()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.AttackHit -= OnAttackHit;
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
            CurrentWeapon.UnEquip();
        }
        
        CurrentWeapon = weapon;
        
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Owner = m_actor;
            CurrentWeapon.AttackHit -= OnAttackHit;
            CurrentWeapon.AttackHit += OnAttackHit;
            CurrentWeapon.Equip(m_actor);
        }
    }

    public void Attack()
    {
        //TODO: KnockBack, KnockDown 중 공격 못하게 할 것인가?
        if (!CurrentWeapon.CanAttack)
        {
            return;
        }

        if (m_actor.IsPossessed)
        {
            Target = default;
        }

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

        if (AbilityType == AbilityType.Trigger || !isToggled)
        {
            m_actor.Status.SkillCoolTime = 0f;
        }
    }
    
    private void OnAttackHit(object sender, EventArgs e)
    {
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);
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
        if (m_actor.Status.SkillCoolTime < 1f)
        {
            //TODO: 부동소수점, deltaTime 이슈로 실제 시간과 작은 오차 발생 가능, 해결 필요한지 확인
            float full = m_data.SkillCoolTime;
            float cur = m_actor.Status.SkillCoolTime * full + Time.deltaTime;

            //TODO: 1/full 미리 계산해두기(최적화)
            m_actor.Status.SkillCoolTime = cur / full;
        }
    }
}

public enum AbilityType
{
    Trigger,
    Toggle
}