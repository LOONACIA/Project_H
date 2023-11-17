using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherSkill: Weapon
{
    public MonsterAttackData data;
    public RocketProjectile rocketProjectile;

    public Vector3 spawnPosition;
    
    protected override void Attack()
    {
        //TODO: 애니메이션 Key 수정하기
        //애니메이션 실행
        Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
    }
    
    protected override void OnHitMotion()
    {
        RocketProjectile rp = Instantiate(rocketProjectile, transform.position, transform.rotation);
        rp.direction = transform.forward;
        rp.owner = Owner.gameObject;
        rp.shooter = this;
    }
}

