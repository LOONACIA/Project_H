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
[RequireComponent(typeof(MonsterStatus))]
public class MonsterAttack : MonoBehaviour
{
	private static readonly int s_attackAnimationKey = Animator.StringToHash("Attack");
	
	private static readonly int s_skillAnimationKey = Animator.StringToHash("Skill");
	
	[SerializeField]
	private MonsterAttackData m_data;

	private Monster m_actor;
	
	private MonsterStatus m_status;
	
	private Weapon[] m_weapons;

	private bool m_isHitBoxChecked;

	public MonsterAttackData Data => m_data;

	public bool CanAttack { get; set; } = true;
	
	public bool IsAttacking { get; protected set; }
	
	public event EventHandler AttackStart;
	
	public event EventHandler AttackFinish;

	private void Awake()
	{
		m_actor = GetComponent<Monster>();
		m_status = GetComponent<MonsterStatus>();
		m_weapons = GetComponentsInChildren<Weapon>(true);
	}

	private void Start()
	{
		m_status.Damage = m_data.Damage;
	}

	private void OnEnable()
	{
		foreach (var weapon in m_weapons)
		{
			RegisterWeaponEvents(weapon);
		}
	}

	private void OnDisable()
	{
		foreach (var weapon in m_weapons)
		{
			UnregisterWeaponEvents(weapon);
		}
	}

	public void Attack()
	{
		if (!CanAttack || IsAttacking)
		{
			return;
		}
		
		m_actor.Animator.SetTrigger(s_attackAnimationKey);
	}

	public void Skill()
	{
		if (!CanAttack || IsAttacking)
		{
			return;
		}
		
		m_actor.Animator.SetTrigger(s_skillAnimationKey);
	}

	private void HandleAttackHit(IEnumerable<IHealth> hitObjects)
	{
		int damage = m_status.Damage;
		HandleHitCore(hitObjects, damage);
	}

	private void HandleSkillHit(IEnumerable<IHealth> hitObjects)
	{
		int damage = m_data.SkillDamage;
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
			health.CurrentHp -= damage;
		}

		m_isHitBoxChecked = true;
	}
	
	private void OnAttackAnimationStart(object sender, EventArgs e)
	{
		m_isHitBoxChecked = false;
		IsAttacking = true;
		AttackStart?.Invoke(this, EventArgs.Empty);
	}
	
	private void OnAttackAnimationEnd(object sender, EventArgs e)
	{
		IsAttacking = false;
		AttackFinish?.Invoke(this, EventArgs.Empty);
	}

	private void RegisterWeaponEvents(Weapon weapon)
	{
		weapon.AttackStart += OnAttackAnimationStart;
		weapon.AttackFinish += OnAttackAnimationEnd;
		weapon.AttackHit += OnAttackHit;
		
		weapon.SkillStart += OnAttackAnimationStart;
		weapon.SkillFinish += OnAttackAnimationEnd;
		weapon.SkillHit += OnSkillHit;
	}

	private void UnregisterWeaponEvents(Weapon weapon)
	{
		weapon.AttackStart -= OnAttackAnimationStart;
		weapon.AttackFinish -= OnAttackAnimationEnd;
		weapon.AttackHit -= OnAttackHit;
		
		weapon.SkillStart -= OnAttackAnimationStart;
		weapon.SkillFinish -= OnAttackAnimationEnd;
		weapon.SkillHit -= OnSkillHit;
	}

	private void OnAttackHit(object sender, IEnumerable<IHealth> e)
	{
		var hits = e.Where(hit => hit.gameObject != gameObject);
		HandleAttackHit(hits);
	}
	
	private void OnSkillHit(object sender, IEnumerable<IHealth> e)
	{
		var hits = e.Where(hit => hit.gameObject != gameObject);
		HandleSkillHit(hits);
	}
	
	private void OnValidate()
	{
		if (m_data == null)
		{
			Debug.LogWarning($"{nameof(m_data)} is null");
		}
	}
}
