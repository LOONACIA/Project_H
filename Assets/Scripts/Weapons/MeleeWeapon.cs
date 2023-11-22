using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Melee Weapon: 기본적인 공격 클래스입니다.
 */
public class MeleeWeapon : Weapon
{
    [SerializeField]
    private TrailCaster m_trailCaster;

    private List<WeaponAttackInfo> m_attackInfoBuffer = new();

    protected override void Attack()
    {
        //TODO: 이번 공격의 End보다 다음 공격의 Start가 먼저 호출될 수 있음.
        //공격 중인데 다음 공격 딜레이가 오지 않았다면 return;
        //if (IsAttacking && !m_canNextAttack) return;
    }

    #region AnimationEvent

    protected override void OnHitMotion()
    {
        //TODO: 1인칭일 경우 카메라 쉐이킹
        m_trailCaster.StartCheck();
    }

    protected override void OnFollowThroughMotion()
    {
        m_trailCaster.EndCheck();
    }

    #endregion

    #region UnityEvent

    private void Update()
    {
        if (State == AttackState.HIT)
        {
            //검의 TrailCaster로 충돌체크
            IEnumerable<RaycastHit> detectedRayCast = m_trailCaster.PopBuffer();

            //공격한 오브젝트 버퍼 초기화
            m_attackInfoBuffer.Clear();

            //AttackInfo 제작해줌
            foreach (var hit in detectedRayCast)
            {
                Actor hitObject = hit.transform.GetComponent<Actor>();

                //체력이 없는 오브젝트거나, 본인이 타겟된 경우는 체크하지 않음.
                if (hitObject != null)
                {
                    m_attackInfoBuffer.Add(new WeaponAttackInfo(hitObject, hit.normal,hit.point));
                }
            }

            //11.17: trailCaster에서 중복처리를 하므로 여기선 하지 않아도 됨

            //공격한 오브젝트가 존재한다면, 공격 정보를 MonsterAttack으로 넘겨줌
            if (m_attackInfoBuffer.Any())
            {
                InvokeHitEvent(m_attackInfoBuffer);
            }
        }
    }

    #endregion
}