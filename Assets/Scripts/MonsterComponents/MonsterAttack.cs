using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private Vector3 m_target;

    [SerializeField]
    private MonsterAttackData m_data;

    private Monster m_actor;

    private ActorStatus m_status;

    public MonsterAttackData Data => m_data;

    public bool CanAttack { get; set; } = true;

    public Vector3 Target
    {
        get => m_target;
        set
        {
            m_target = value;
            AttackWeapon.Target = value;
        }
    }

    public bool IsAttacking => AttackWeapon != null && AttackWeapon.IsAttacking;

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
        m_status.SkillCoolTime = 1f;

        RegisterHitEvents();
    }

    private void OnDestroy()
    {
        UnregisterHitEvents();
    }

    private void Update()
    {
        UpdateSkillCoolTime();
    }

    public void Attack()
    {
        //TODO: KnockBack, KnockDown 중 공격 못하게 할 것인가?
        if (!CanAttack)
        {
            return;
        }

        if (m_actor.IsPossessed)
        {
            Target = default;
        }

        m_actor.Animator.SetTrigger(s_attackAnimationKey);
        AttackWeapon.Target = Target;
        AttackWeapon.StartAttack();
    }

    public void Skill()
    {
        //TODO: KnockBack, KnockDown 중 스킬 못하게 할 것인가?
        if (!CanAttack || m_actor.Status.SkillCoolTime < 1f)
        {
            return;
        }

        m_actor.Animator.SetTrigger(s_skillAnimationKey);
        SkillWeapon.StartAttack();
        m_actor.Status.SkillCoolTime = 0f;
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
            DamageInfo damageInfo = new DamageInfo(data.Damage, hit.AttackDirection, hit.HitPosition, m_actor);

            //넉다운 적용
            if (data.KnockDownTime > 0f)
            {
                if (hit.HitObject.Status.CanKnockDown)
                    hit.HitObject.Status.SetKnockDown(data.KnockDownTime);
            }

            //넉백 적용
            if (data.KnockBackPower != 0f)
            {
                MonsterMovement movement = health.gameObject.GetComponent<MonsterMovement>();

                if (hit.HitObject.Status.CanKnockBack)
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
            Debug.LogError("무기가 아닌 오브젝트가 공격 호출함: " + o.ToString());
            return;
        }

        MonsterAttackData.AttackData data = GetWeaponDataByType(m_actor.IsPossessed, hitWeapon.Type);

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

    public void UpdateSkillCoolTime()
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

    private void RegisterWeaponComponents(Weapon[] weapons, ref Weapon attackWeapon, ref Weapon skillWeapon,
        ref Weapon blockPushWeapon)
    {
        foreach (var weapon in weapons)
        {
            switch (weapon.Type)
            {
                case Weapon.WeaponType.AttackWeapon:
                    if (attackWeapon != null)
                    {
                        Debug.LogError("AttackWeapon 중복 등록됨");
                    }

                    attackWeapon = weapon;
                    break;
                case Weapon.WeaponType.SkillWeapon:
                    if (skillWeapon != null)
                    {
                        Debug.LogError("SkillWeapon 중복 등록됨");
                    }

                    skillWeapon = weapon;
                    break;
                case Weapon.WeaponType.BlockPushWeapon:
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

    private MonsterAttackData.AttackData GetWeaponDataByType(bool isPossessed, Weapon.WeaponType type)
    {
        if (isPossessed)
        {
            return type switch
            {
                Weapon.WeaponType.SkillWeapon => m_data.PossessedSkill,
                Weapon.WeaponType.AttackWeapon => m_data.PossessedAttack,
                Weapon.WeaponType.BlockPushWeapon => m_data.PossessedBlockPush,
                _ => null
            };
        }
        else
        {
            return type switch
            {
                Weapon.WeaponType.SkillWeapon => m_data.Skill,
                Weapon.WeaponType.AttackWeapon => m_data.Attack,
                Weapon.WeaponType.BlockPushWeapon => m_data.BlockPush,
                _ => null
            };
        }
    }
}