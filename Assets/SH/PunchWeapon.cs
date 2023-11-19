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
        
    }

    protected override void OnHitMotion()
    {
        Vector3 dir = transform.TransformPoint(hitBox.Position) - transform.position;
        
        var detectedList = hitBox.DetectHitBox(transform)
                                 .Select(info => new WeaponAttackInfo(info,dir,dir))
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
