using System.Collections.Generic;
using UnityEngine;

/*
 * Melee Weapon: 기본적인 공격 클래스입니다.
 */
public class MeleeWeapon : Weapon
{
    private readonly List<AttackInfo> m_attackInfoBuffer = new();
    
    [SerializeField]
    private TrailCaster m_trailCaster;
    
    private void Update()
    {
        if (State == WeaponState.Attack)
        {
            TrailCast();
        }
    }

    protected override void OnAttackState()
    {
        //TODO: 1인칭일 경우 카메라 쉐이킹
        m_trailCaster.StartCheck();
    }

    protected override void OnRecoveryState()
    {
        m_trailCaster.EndCheck();
    }

    protected override void OnIdleState()
    {
        //애니메이터 단에서 Recovery를 거치지 않고 Idle로 들어가는 상황이 발생할 수 있어 Idle에도 EndCheck을 실행합니다.
        m_trailCaster.EndCheck();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void TrailCast()
    {
        //검의 TrailCaster로 충돌체크
        IEnumerable<TrailCaster.HitInfo> detectedRayCast = m_trailCaster.PopBuffer();

        //공격한 오브젝트 버퍼 초기화
        m_attackInfoBuffer.Clear();

        //AttackInfo 제작해줌
        foreach (var hitInfo in detectedRayCast)
        {
            //체력이 없는 오브젝트거나, 본인이 타겟된 경우는 체크하지 않음.
            if (hitInfo.Hit.transform.TryGetComponent<IHealth>(out var hitObject))
            {
                m_attackInfoBuffer.Add(new(Owner.gameObject, hitObject, Damage, hitInfo.Hit.point, hitInfo.AttackDirection));
            }
        }
            
        //공격한 오브젝트가 존재한다면, 공격 정보를 MonsterAttack으로 넘겨줌
        Hit(m_attackInfoBuffer);
    }
}