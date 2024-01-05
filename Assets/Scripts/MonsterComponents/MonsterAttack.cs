using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (m_abilities.Length == 0)
        {
            m_abilities = GetComponentsInChildren<Ability>();
            foreach (Ability ability in m_abilities.AsSpan())
            {
                ability.Owner = m_actor;
                ability.StateChanged -= OnAbilityStateChanged;
                ability.StateChanged += OnAbilityStateChanged;
            }
        }
        
        m_actor.Status.HasCooldown = m_data.SkillCoolTime > 0f;
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
            ability.StateChanged -= OnAbilityStateChanged;
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
        if (m_abilities.Length == 0)
        {
            m_abilities = GetComponentsInChildren<Ability>();
            foreach (Ability ability in m_abilities.AsSpan())
            {
                ability.Owner = m_actor;
                ability.StateChanged -= OnAbilityStateChanged;
                ability.StateChanged += OnAbilityStateChanged;
            }
        }
        
        //TODO: KnockBack, KnockDown 중 스킬 못하게 할 것인가?
        if (m_actor.Status.HasCooldown && m_actor.Status.SkillCoolTime < 1f)
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

        //스킬 사운드
        //gameObject.FindChild<MonsterSFXPlayer>().OnPlaySkill();
    }

    private void OnWeaponStateChanged(object sender, WeaponState e)
    {
        m_isAttackTriggered = false;

        if (e == WeaponState.Attack)
        {
            if (m_actor.IsPossessed)
            {
                Target = default;
            }
            
            //공격 스테이트로 진입 시, 타격 수 count 초기화
            m_diedVictimCount = 0;
            m_bool = true;
            m_eliteBool = true;

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

    private bool m_bool;
    private bool m_eliteBool;
            
    private void OnAttackHit(object sender, IEnumerable<AttackInfo> e)
    {
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);

        // 죽은 몬스터 수 체크
        m_diedVictimCount += e.Count(info => info.Victim.CurrentHp <= 0f);
        
        if(m_actor is EliteBoss && m_diedVictimCount >= 5 && m_eliteBool)
        {
            //시간 조절
            GameManager.Effect.ChangeTimeScale(this, 0f, 0.3f, 1000f, 1000f);
            
            GameObject light = ManagerRoot.Resource.Instantiate(GameManager.Settings.AttackLight);
            light.transform.position = Camera.main.transform.position + transform.forward * 0.5f;

            GameObject light2 = ManagerRoot.Resource.Instantiate(GameManager.Settings.AttackLight);
            light2.transform.position = Camera.main.transform.position;

            m_eliteBool = false;

            GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.EliteBossAttackEffectStart);

            StartCoroutine(IE_EliteBossAttackEffect());
        }
        else if (m_diedVictimCount >= 2 && m_bool)
        {
            //시간 조절
            GameManager.Effect.ChangeTimeScale(this, 0f, 0.1f, 1000f, 1000f);
            //카메라 쉐이크
            if (m_actor is Monster { Data: { Type: ActorType.Melee } })
            {
                GameManager.Effect.ShakeCamera(10, 0.6f);
            }
            else if(m_actor is EliteBoss)
            {
                GameManager.Effect.ShakeCamera(5, 0.6f);
            }

            //빛
            GameObject light = ManagerRoot.Resource.Instantiate(GameManager.Settings.AttackLight);
            light.transform.position = Camera.main.transform.position + transform.forward * 0.5f;

            m_bool = false;
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
            float cur = m_actor.Status.SkillCoolTime * coolTime + Time.deltaTime;
            m_actor.Status.SkillCoolTime = cur / coolTime;
        }
    }

    private IEnumerator IE_EliteBossAttackEffect()
    {
        yield return new WaitForSecondsRealtime(0.3f);

        GameManager.Effect.ShakeCamera(7, 0.6f);

        GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.EliteBossAttackEffectExplosion);
    }
}

public enum AbilityType
{
    Trigger,
    Toggle
}