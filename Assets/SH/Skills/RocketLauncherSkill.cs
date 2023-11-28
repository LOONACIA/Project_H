using UnityEngine;

public class RocketLauncherSkill: Weapon
{
    public RocketProjectile rocketProjectile;

    public Vector3 spawnPosition;
    public bool isShowingSpawnPositionGizmo;
    
    protected override void Attack()
    {
        //TODO: 애니메이션 Key 수정하기
        //애니메이션 실행
        //Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
    }
    
    protected override void OnHitMotion()
    {
        RocketProjectile rp = Instantiate(rocketProjectile, transform.TransformPoint(spawnPosition), transform.rotation);
        rp.direction = transform.forward;
        rp.shooter = this;
    }

    private void OnDrawGizmos()
    {
        if (isShowingSpawnPositionGizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            Gizmos.DrawWireSphere(spawnPosition, 0.3f);
        }
    }
}

