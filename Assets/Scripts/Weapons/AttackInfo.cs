using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackInfo
{
   public int damage;

   public Vector3 attackDirection;

   public IEnumerable<IHealth> hitObjects;

   public AttackInfo(int damage, Vector3 attackDirection, IEnumerable<IHealth> hitObjects)
   {
       this.damage = damage;
       this.attackDirection = attackDirection;
       this.hitObjects = hitObjects;
   }
}
