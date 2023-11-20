using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockPushWeapon : Weapon
{
    public HitBox hitBox;
    public Transform hitTransform;
    
    protected override void Attack()
    {
        
    }

    protected override void OnHitMotion()
    {
        Vector3 dir = transform.TransformDirection(new Vector3(0f, 0f, 1f)).normalized;
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
