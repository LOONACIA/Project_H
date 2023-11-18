using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PunchWeapon : Weapon
{
    public HitBox hitBox;
    
    protected override void Attack()
    {
        Animator.SetTrigger("Attack");
    }

    protected override void OnHitMotion()
    {
        var detectedList = hitBox.DetectHitBox(transform)
                                 .Where(health => health.gameObject!=Owner.gameObject)
                                 .Select(info => new AttackInfo(WeaponData, hitBox.Position, Owner, info,(transform.position - Owner.transform.position).normalized))
                                 ;

        if (detectedList.Any())
        {
            InvokeHitEvent(detectedList);
        }
    }

    public void OnDrawGizmos()
    {
        hitBox.DrawGizmo(transform);
    }
}
