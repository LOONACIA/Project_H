using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LOONACIA.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 공격을 시작하고, Weapon으로부터 전달받은 IHealth 객체에 피해를 가하는 함수
 * 판정은 Weapon 클래스에서 이루어짐
 */
[RequireComponent(typeof(ActorStatus))]
public class MonsterAttack : MonoBehaviour
{
	private static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");
	
	
	[SerializeField]
	private MonsterAttackData m_data;

	private Monster m_actor;
	
	private ActorStatus m_status;

	private bool m_isHitBoxChecked;

	public MonsterAttackData Data => m_data;

	public bool CanAttack { get; set; } = true;
	
	public bool IsAttacking { get; protected set; }

    public event EventHandler<Monster> attackEventHandler;

    private void Awake()
	{
		m_actor = GetComponent<Monster>();
		m_status = GetComponent<ActorStatus>();
    }

	private void Start()
	{
		m_status.Damage = m_data.Damage;
	}

	private void OnEnable()
	{
        //Weapon의 Hit판정이 일어났을 때에 대한 이벤트를 등록합니다.
        //RegisterWeaponEvents(attackWeapon);
    }

	private void OnDisable()
	{
        //UnregisterWeaponEvents(attackWeapon);
	}

	public void Attack()
	{
		if (!CanAttack || IsAttacking)
		{
			return;
		}
		
		//m_actor.Animator.SetTrigger(s_attackAnimationKey);
        attackEventHandler?.Invoke(this, m_actor);
	}

	private void HandleHitEvent(IEnumerable<IHealth> hitObjects)
	{
		int damage = m_status.Damage;
		HandleHitCore(hitObjects, damage);
	}
	
	private void HandleHitCore(IEnumerable<IHealth> hitObjects, int damage)
	{
		//이전 프레임에서 공격 판정이 처리되었다면 return
		if (m_isHitBoxChecked)
		{
			return;
		}

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
            m_isHitBoxChecked = true;
		}

	}

	private void RegisterWeaponEvents(Weapon weapon)
	{
		//weapon.HitEventHandler += OnHitEvent;
	}

	private void UnregisterWeaponEvents(Weapon weapon)
	{
		//weapon.HitEventHandler -= OnHitEvent;
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
