using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleFrameHitBoxWeapon : Weapon
{
    public HitBox hitBox;
    public Vector3 knockBackDirection = Vector3.forward;
    
    protected override void Attack()
    {
        
    }

    protected override void OnHitMotion()
    {
        Vector3 dir = transform.TransformDirection(knockBackDirection);
        
        var detectedList = hitBox.DetectHitBox(transform)
                                 .Select(info => new WeaponAttackInfo(info,dir.normalized))
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
