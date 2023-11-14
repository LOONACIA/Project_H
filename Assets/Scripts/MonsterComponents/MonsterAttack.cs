using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LOONACIA.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 1인칭, 3인칭 공격을 시전, 데미지 처리
 */
[RequireComponent(typeof (ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
    public static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");

    [SerializeField] private Weapon firstPersonAttack;
    [SerializeField] private Weapon thirdPersonAttack;
    [SerializeField]
    private MonsterAttackData m_data;

    private Monster m_actor;

    private ActorStatus m_status;

    public MonsterAttackData Data => m_data;

    public bool CanAttack { get; set; } = true;

    public bool IsAttacking { get; protected set; }

    public Weapon AttackWeapon => m_actor.IsPossessed ? firstPersonAttack : thirdPersonAttack;

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

    private void HandleHitEvent(IEnumerable<IHealth> hitObjects)
    {
        int damage = m_status.Damage;
        HandleHitCore(hitObjects, damage);
    }

    private void HandleHitCore(IEnumerable<IHealth> hitObjects, int damage)
    {
        foreach (var health in hitObjects)
        {
            // 빙의되지 않은 몬스터가 타겟이 아닌 대상을 공격하는 경우
            if (!m_actor.IsPossessed &&
                health.gameObject.TryGetComponent<Actor>(out var actor) && !m_actor.Targets.Contains(actor))
            {
                continue;
            }

            Debug.Log($"{health.gameObject.name} is hit by {gameObject.name}, damage: {damage}");
            health.TakeDamage(damage, m_actor);
        }

    }

    public void OnHitEvent(object sender, IEnumerable<IHealth> e)
    {
        var hits = e.Where(hit => hit.gameObject != gameObject);
        HandleHitEvent(hits);
    }

    private void OnValidate()
    {
        if (m_data == null)
        {
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
        }
    }
}