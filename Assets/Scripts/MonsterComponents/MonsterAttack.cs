using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LOONACIA.Unity;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/*
 * 1인칭, 3인칭 공격을 시전, 데미지 처리
 */
[RequireComponent(typeof(ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
    private static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");
    
    private static readonly int s_skillAnimationKey = Animator.StringToHash("Skill");
    
    private static readonly int s_targetCheckAnimationKey = Animator.StringToHash("TargetCheck");

    private Weapon m_firstPersonAttack;
    
    private Weapon m_thirdPersonAttack;

    private Weapon m_firstPersonSkill;
    
    private Weapon m_thirdPersonSkill;

    //현재 미사용, 등록만 되어있음
    private Weapon m_firstPersonBlockPush;
    
    private Weapon m_thirdPersonBlockPush;

    [SerializeField]
    private MonsterAttackData m_data;

    private Monster m_actor;

    private ActorStatus m_status;

    public MonsterAttackData Data => m_data;

    public bool CanAttack { get; set; } = true;

    public bool IsAttacking 
    {
        get
        {
            if (AttackWeapon == null) return false;
            return AttackWeapon.State is Weapon.AttackState.IDLE or Weapon.AttackState.FOLLOW_THROUGH;
        }
    }

    public Weapon AttackWeapon => m_actor.IsPossessed ? m_firstPersonAttack : m_thirdPersonAttack;
    
    public Weapon SkillWeapon => m_actor.IsPossessed ? m_firstPersonSkill : m_thirdPersonSkill;
    
    public Weapon BlockPushWeapon => m_actor.IsPossessed ? m_firstPersonBlockPush : m_thirdPersonBlockPush;

    private void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<ActorStatus>();

        Weapon[] fpWeapons = m_actor.FirstPersonAnimator.GetComponents<Weapon>();
        Weapon[] tpWeapons = m_actor.ThirdPersonAnimator.GetComponents<Weapon>();
        RegisterWeaponComponents(fpWeapons, ref m_firstPersonAttack, ref m_firstPersonSkill,
            ref m_firstPersonBlockPush);
        RegisterWeaponComponents(tpWeapons, ref m_thirdPersonAttack, ref m_thirdPersonSkill,
            ref m_thirdPersonBlockPush);
    }

    private void Start()
    {
        m_status.Damage = m_data.Attack.Damage;

        RegisterHitEvents();
    }

    private void OnDestroy()
    {
        UnregisterHitEvents();
    }

    public void Attack()
    {
        //TODO: KnockBack, KnockDown 중 공격 못하게 할 것인가?
        if (!CanAttack)
        {
            return;
        }

        m_actor.Animator.SetTrigger(s_attackAnimationKey);
        AttackWeapon.StartAttack();
    }

    public void Skill()
    {
        //TODO: KnockBack, KnockDown 중 스킬 못하게 할 것인가?
        if (!CanAttack || IsAttacking)
        {
            return;
        }

        m_actor.Animator.SetTrigger(s_skillAnimationKey);
        SkillWeapon.StartAttack();
    }

    private void HandleHitEvent(MonsterAttackData.AttackData data, IEnumerable<WeaponAttackInfo> info)
    {
        //int damage = m_status.Damage;
        //TODO: 플레이어의 공격 정보 반영하여 데미지 처리
        HandleHitCore(data, info);
    }

    private void HandleHitCore(MonsterAttackData.AttackData data, IEnumerable<WeaponAttackInfo> info)
    {
        // 공격 성공 시 애니메이션 실행 
        //StartCoroutine(AttackImpact());
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);


        foreach (var hit in info)
        {
            IHealth health = hit.HitObject.Health;
            // 빙의되지 않은 몬스터가 타겟이 아닌 대상을 공격하는 경우
            if (!m_actor.IsPossessed &&
                health.gameObject.TryGetComponent<Actor>(out var actor) && !m_actor.Targets.Contains(actor))
            {
                continue;
            }

            Debug.Log($"{health.gameObject.name} is hit by {gameObject.name}, damage: {data.Damage}");

            //데미지 처리
            DamageInfo damageInfo = new DamageInfo(data.Damage, hit.AttackDirection, m_actor);

            //넉다운 적용
             if (data.KnockDownTime>0f)
             {
                 
                 hit.HitObject.Status.SetKnockDown(data.KnockDownTime);
             }

            //넉백 적용
            //TODO: 공격, 스킬, 밀쳐내기 등의 넉백여부 구분 필요
            if (hit.KnockBackDirection != Vector3.zero)
            {
                MonsterMovement movement = health.gameObject.GetComponent<MonsterMovement>();

                //TODO: 공격 종류별로 넉백 파워 수정 필요
                movement.TryKnockBack(hit.KnockBackDirection, data.KnockBackPower);
            }
            
            //BT의 Hit이벤트가 등록되어있어 CC등 처리 후 마지막에 실행
            health.TakeDamage(damageInfo);
        }
    }

    public void OnHitEvent(object o, IEnumerable<WeaponAttackInfo> attackInfo)
    {
        //대상 무기의 공격 데이터를 설정정함
        Weapon hitWeapon = o as Weapon;
        if (hitWeapon == null)
        {
            Debug.LogError("무기가 아닌 오브젝트가 공격 호출함: "+o.ToString());
            return;
        }
        MonsterAttackData.AttackData data = GetWeaponDataByType(hitWeapon.Type);

        var hits = attackInfo.Where(hit => hit.HitObject.gameObject != gameObject);

        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);

        HandleHitEvent(data, hits);
    }

    private void OnValidate()
    {
        if (m_data == null)
        {
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
        }
    }

    private void RegisterHitEvents()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>(true);
        foreach (var weapon in weapons)
        {
            weapon.OnHitEvent += OnHitEvent;
        }
    }

    private void UnregisterHitEvents()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>(true);
        foreach (var weapon in weapons)
        {
            weapon.OnHitEvent -= OnHitEvent;
        }
    }

    private void RegisterWeaponComponents(Weapon[] weapons, ref Weapon attackWeapon, ref Weapon skillWeapon, ref Weapon blockPushWeapon)
    {
        foreach (var weapon in weapons)
        {
            switch (weapon.Type)
            {
                case Weapon.WeaponType.ATTACK_WEAPON:
                    if (attackWeapon != null)
                    {
                        Debug.LogError("AttackWeapon 중복 등록됨");
                    }

                    attackWeapon = weapon;
                    break;
                case Weapon.WeaponType.SKILL_WEAPON:
                    if (skillWeapon != null)
                    {
                        Debug.LogError("SkillWeapon 중복 등록됨");
                    }

                    skillWeapon = weapon;
                    break;
                case Weapon.WeaponType.BLOCKPUSH_WEAPON:
                    if (blockPushWeapon != null)
                    {
                        Debug.LogError("BlockPushWeapon 중복 등록됨");
                    }

                    blockPushWeapon = weapon;
                    break;
                default:
                    Debug.LogError("초기화 오류: 등록되지 않은 Weapon 등록됨");
                    break;
            }
        }

    }

    private MonsterAttackData.AttackData GetWeaponDataByType(Weapon.WeaponType type)
    {
        return type switch
        {
            Weapon.WeaponType.SKILL_WEAPON => m_data.Skill,
            Weapon.WeaponType.ATTACK_WEAPON => m_data.Attack,
            Weapon.WeaponType.BLOCKPUSH_WEAPON => m_data.BlockPush,
            _ => null
        };
    }
}