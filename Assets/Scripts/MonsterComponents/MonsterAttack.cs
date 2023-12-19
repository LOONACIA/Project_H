using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/*
 * 1인칭, 3인칭 공격을 시전, 데미지 처리
 */
[RequireComponent(typeof(ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
    private static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");

    private static readonly int s_skillAnimationKey = Animator.StringToHash("Skill");

    private static readonly int s_targetCheckAnimationKey = Animator.StringToHash("TargetCheck");

    private Vector3 m_target;

    [SerializeField]
    private MonsterAttackData m_data;

    private Monster m_actor;

    public MonsterAttackData Data => m_data;

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

    public void Ability()
    {
        //TODO: KnockBack, KnockDown 중 스킬 못하게 할 것인가?
        if (m_actor.Status.SkillCoolTime < 1f)
        {
            return;
        }

        m_actor.Animator.SetTrigger(s_skillAnimationKey);
        m_actor.Status.SkillCoolTime = 0f;
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