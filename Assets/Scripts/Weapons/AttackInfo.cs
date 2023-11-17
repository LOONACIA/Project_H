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
  

   public AttackInfo(int damage, Vector3 attackDirection, Monster attacker, IHealth hitObject)
   {
       this.damage = damage;
       this.attackDirection = attackDirection;
       this.attacker = attacker;
       this.hitObject = hitObject;
   }
}
