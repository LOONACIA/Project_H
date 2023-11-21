using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterAttackData), menuName = "Data/" + nameof(MonsterAttackData))]
public class MonsterAttackData : ScriptableObject
{
    //플레이어 데이터
    [field:SerializeField] public AttackData Attack { get; private set; }
    [field:SerializeField] public AttackData Skill { get; private set; }
    [field:SerializeField] public AttackData BlockPush { get; private set; }
    
    //빙의 데이터
    [field:SerializeField] public AttackData PossessedAttack { get; private set; }
    [field:SerializeField] public AttackData PossessedSkill { get; private set; }
    [field:SerializeField] public AttackData PossessedBlockPush { get; private set; }

    [Serializable]
    public class AttackData
    {
        [SerializeField] private string m_name;
        
        [SerializeField]private int m_damage;
        [SerializeField]private float m_knockDownTime;
        [SerializeField]private float m_knockBackPower;

        public string Name => m_name;
        
        public int Damage => m_damage;
        public float KnockDownTime => m_knockDownTime;
        public float KnockBackPower => m_knockBackPower;
    }
    
}