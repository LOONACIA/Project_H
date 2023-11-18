using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LOONACIA.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/*
 * 1인칭, 3인칭 공격을 시전, 데미지 처리
 */
[RequireComponent(typeof (ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
    public static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");
    public static readonly int s_targetCheckAnimationKey = Animator.StringToHash("TargetCheck");

    [SerializeField] private Weapon firstPersonAttack;
    [SerializeField] private Weapon thirdPersonAttack;
    
    
    [SerializeField] private Weapon firstPersonSkill;
    [SerializeField] private Weapon thirdPersonSkill;
    
    [SerializeField]
    private MonsterAttackData m_data;

    private Monster m_actor;

    private ActorStatus m_status;

    public MonsterAttackData Data => m_data;

    public bool CanAttack { get; set; } = true;

    public bool IsAttacking { get; protected set; }

    public Weapon AttackWeapon => m_actor.IsPossessed ? firstPersonAttack : thirdPersonAttack;
    public Weapon SkillWeapon => m_actor.IsPossessed ? firstPersonSkill : thirdPersonSkill;

    private void Awake()
    {
        m_actor = GetComponent<Monster>();
        m_status = GetComponent<ActorStatus>();
    }

    private void Start()
    {
        m_status.Damage = m_data.Damage;
    }

    public void Attack()
    {
        if (!CanAttack || IsAttacking)
        {
            return;
        }

        AttackWeapon.StartAttack(m_actor);
    }

    public void Skill()
    {
        if (!CanAttack || IsAttacking)
        {
            return;
        }
        
        SkillWeapon.StartAttack(m_actor);
    }

    private void HandleHitEvent(IEnumerable<AttackInfo> info)
    {
        //int damage = m_status.Damage;
        HandleHitCore(info);
    }

    private void HandleHitCore(IEnumerable<AttackInfo> info)
    {
        // 공격 성공 시 애니메이션 실행 
        //StartCoroutine(AttackImpact());
        m_actor.Animator.SetTrigger(s_targetCheckAnimationKey);


        foreach (var hit in info)
        {
            IHealth health = hit.hitObject;
            // 빙의되지 않은 몬스터가 타겟이 아닌 대상을 공격하는 경우
            if (!m_actor.IsPossessed &&
                health.gameObject.TryGetComponent<Actor>(out var actor) && !m_actor.Targets.Contains(actor))
            {
                continue;
            }

            Debug.Log($"{health.gameObject.name} is hit by {gameObject.name}, damage: {hit.damage}");
            health.TakeDamage(hit, m_actor);
        }

    }

    public void OnHitEvent(IEnumerable<AttackInfo> attackInfo)
    {
        var hits = attackInfo.Where(hit => hit.hitObject.gameObject != gameObject);
        HandleHitEvent(attackInfo);
    }

    private void OnValidate()
    {
        if (m_data == null)
        {
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
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
}