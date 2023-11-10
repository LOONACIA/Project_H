using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity;
using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
	[SerializeField]
	[ReadOnly]
	[Tooltip("Hp는 " + nameof(MonsterHealth) + "에서 관리됨")]
	private int m_hp;
	
	[SerializeField]
	[ReadOnly]
	private int m_damage;
	public int Hp
	{
		get => m_hp;
		set => m_hp = value;
	}

	public int Damage
	{
		get => m_damage;
		set => m_damage = value;
	}
}
