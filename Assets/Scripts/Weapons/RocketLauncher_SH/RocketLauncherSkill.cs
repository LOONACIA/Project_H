using UnityEngine;

/*
 * 23.12.03: 기존 스킬 테스트용으로 제작한 로켓 런쳐 스킬입니다. 
 * 연결된 프리팹이 있을 수 있어 프리프로덕션 제출 이후 확인 후 삭제 예정입니다. 
 */

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

