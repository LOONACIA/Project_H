using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackInfo
{
   public int damage;

   //공격이 들어온 방향
   public Vector3 attackDirection;

   //공격자
   public Monster attacker;
   
   //피격자
   public IHealth hitObject;

   public AttackInfo()
   {
       this.damage = 0;
       this.attackDirection = new Vector3();
       this.attacker = null;
       this.hitObject = null;
   }
  
   public AttackInfo(int damage, Vector3 attackDirection, Monster attacker, IHealth hitObject)
   {
       this.damage = damage;
       this.attackDirection = attackDirection;
       this.attacker = attacker;
       this.hitObject = hitObject;
   }

   /// <summary>
   /// MonsterAttackData를 받아 AttackInfo에 대응하는 값에 대입합니다.
   /// </summary>
   public AttackInfo(MonsterAttackData attackData, Vector3 attackDirection, Monster attacker, IHealth hitObject)
   {
       //MonsterAttackData
       this.damage = attackData.Damage;
       
       //공격 자체에 대한 정보
       this.attackDirection = attackDirection;
       this.attacker = attacker;
       this.hitObject = hitObject;
   }
}
