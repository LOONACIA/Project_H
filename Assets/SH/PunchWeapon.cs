using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PunchWeapon : Weapon
{
    public HitBox hitBox;
    public MonsterAttackData punchData;
    
    protected override void Attack()
    {
        Animator.SetTrigger("Attack");
    }

    protected override void OnHitMotion()
    {
        var detectedList = hitBox.DetectHitBox(transform)
                                 .Select(info => new AttackInfo(punchData, hitBox.Position, Owner, info));

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
