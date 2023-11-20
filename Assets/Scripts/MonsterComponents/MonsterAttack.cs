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
[RequireComponent(typeof (ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
    public static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");
    public static readonly int s_skillAnimationKey = Animator.StringToHash("Skill");
    public static readonly int s_targetCheckAnimationKey = Animator.StringToHash("TargetCheck");

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

    [field: SerializeField]
    public bool IsAttacking { get; set; }

    public Weapon AttackWeapon => m_actor.IsPossessed ? m_firstPersonAttack : m_thirdPersonAttack;
    public Weapon SkillWeapon => m_actor.IsPossessed ? m_firstPersonSkill : m_thirdPersonSkill;

    private void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<ActorStatus>();
        
        Weapon[] wpWeapons = m_actor.FirstPersonAnimator.GetComponents<Weapon>();
        Weapon[] tpWeapons = m_actor.ThirdPersonAnimator.GetComponents<Weapon>();
        RegisterWeaponComponents(wpWeapons, ref m_firstPersonAttack, ref m_firstPersonSkill, ref m_firstPersonBlockPush);
        RegisterWeaponComponents(tpWeapons, ref m_thirdPersonAttack, ref m_thirdPersonSkill, ref m_thirdPersonBlockPush);
    }

    private void Start()
    {
        m_status.Damage = m_data.Damage;
        
        RegisterHitEvents();
    }

    private void OnDestroy()
    {
        UnregisterHitEvents();
    }

    public void Attack()
    {
        if (!CanAttack || IsAttacking)
        {
            return;
        }
        m_actor.Animator.SetTrigger(s_attackAnimationKey);
        AttackWeapon.StartAttack();
    }

    public void Skill()
    {
        if (!CanAttack || IsAttacking )
        {
            return;
        }
        m_actor.Animator.SetTrigger(s_skillAnimationKey);
        SkillWeapon.StartAttack();
    }

    private void HandleHitEvent(IEnumerable<WeaponAttackInfo> info)
    {
        //int damage = m_status.Damage;
        //TODO: 플레이어의 공격 정보 반영하여 데미지 처리
        HandleHitCore(info);
    }

    private void HandleHitCore(IEnumerable<WeaponAttackInfo> info)
    {
        // 공격 성공 시 애니메이션 실행 
        //StartCoroutine(AttackImpact());
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);


        foreach (var hit in info)
        {
            IHealth health = hit.HitObject;
            // 빙의되지 않은 몬스터가 타겟이 아닌 대상을 공격하는 경우
            if (!m_actor.IsPossessed &&
                health.gameObject.TryGetComponent<Actor>(out var actor) && !m_actor.Targets.Contains(actor))
            {
                continue;
            }

            Debug.Log($"{health.gameObject.name} is hit by {gameObject.name}, damage: {m_data.Damage}");
            
            //데미지 처리
            health.TakeDamage(m_data.Damage,hit.AttackDirection, m_actor);

            //넉다운 적용
            // if (m_data.knockDownTime>0f)
            // {
            //     m_status.SetKnockDown(weaponAttackInfo.knockDownTime);
            // }

            //넉백 적용
            //TODO: 공격, 스킬, 밀쳐내기 등의 넉백여부 구분 필요
            if (hit.KnockBackDirection != Vector3.zero)
            {
                MonsterMovement movement = health.gameObject.GetComponent<MonsterMovement>();

                Debug.Log(hit.KnockBackDirection);
                //TODO: 공격 종류별로 넉백 파워 수정 필요
                movement.TryKnockBack(hit.KnockBackDirection, 14);
            }
        }

    }

    public void OnHitEvent(object o, IEnumerable<WeaponAttackInfo> attackInfo)
    {
        var hits = attackInfo.Where(hit => hit.HitObject.gameObject != gameObject);
        
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);
        
        HandleHitEvent(hits);
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
        foreach(var weapon in weapons)
        {
            weapon.onHitEvent += OnHitEvent;
        }
    }
    
    private void UnregisterHitEvents()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>(true);
        foreach (var weapon in weapons)
        {
            weapon.onHitEvent -= OnHitEvent;
        }
    }

    private IEnumerator AttackImpact()
    { 
        float startTime = Time.time;

        //m_actor.Animator.GetCurrentAnimatorStateInfo(0).speed = -1;

        m_actor.Animator.speed = -1;
        yield return new WaitForSeconds(0.1f);
        m_actor.Animator.speed = 1;

        //m_actor.Animator.speed = -2;

        //while (Time.time - startTime < 5f)
        //{
        //    Debug.Log(m_actor.Animator.speed);    
        //    yield return null;
        //}

        //m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);
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
}