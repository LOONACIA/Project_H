using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterAttackData), menuName = "Data/" + nameof(MonsterAttackData))]
public class MonsterAttackData : ScriptableObject
{
	[SerializeField]
	private int m_damage;

	[SerializeField]
	private int m_possessedDamage;

	[SerializeField]
	private int m_skillDamage;

	public int Damage => m_damage;
	
	public int PossessedDamage => m_possessedDamage;

	public int SkillDamage => m_skillDamage;
}
