using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterWeapon : Weapon
{

    protected override void Attack()
    {
        Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
    }

    protected override void OnLeadInMotion()
    {
        Debug.Log($"사격 모션 시작(선딜), State: {State}");
    }

    protected override void OnHitMotion()
    {
        Debug.Log($"사격, State: {State}");
    }

    protected override void OnFollowThroughMotion()
    {
        Debug.Log($"사격 모션 종료(후딜), State: {State}");
    }
}
