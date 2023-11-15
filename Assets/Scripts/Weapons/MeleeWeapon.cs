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
    private HitBox attackHitBox;

    [SerializeField]
    private GameObject swordWeapon;

    private int m_hitEventIndex = 0;
    private bool m_isHitBoxChecked = false;

    //콤보 공격 관련 변수
    private bool m_canNextAttack = false;

    protected override void Attack()
    {
        //TODO: 이번 공격의 End보다 다음 공격의 Start가 먼저 호출될 수 있음.
        //공격 중인데 다음 공격 딜레이가 오지 않았다면 return;
        //if (IsAttacking && !m_canNextAttack) return;

        Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
    }

    #region AnimationEvent

    private Dictionary<long, GameObject> attackedEnemyList = new();
    private List<IHealth> temporaryDetectedList = new();


    protected override void OnHitMotion()
    {
        //TODO: 1인칭일 경우 카메라 쉐이킹

        //공격 관련 변수 초기화
        IsAttacking = true;
        //m_isHitBoxChecked = false;
        m_hitEventIndex = 0;

        //TODO: 무기별로 콤보 대기 타이밍이 다름. 현재는 애니메이션 기준이지만, 후에 시간 기준으로 변경할지 논의 필요.
        m_canNextAttack = false;

        attackedEnemyList.Clear();
        Animator.SetBool(MonsterAttack.s_targetCheckAnimationKey, false);
    }

    protected override void OnFollowThroughMotion()
    {
        IsAttacking = false;
    }

    #endregion

    #region UnityEvent

    private void Update()
    {
        if (State == AttackState.HIT)
        {
            //MeleeWeapon은 hit 판정 프레임 중 한번이라도 적에게 닿았다면 바로 취소합니다.
            //if (m_isHitBoxChecked) return;

            //내 몬스터와 다른 대상만 가져옴
            var detectedObjects
                = attackHitBox.DetectHitBox(transform)
                              .Where(hit => hit.gameObject != Owner.gameObject);

            foreach (var detected in detectedObjects)
            {
                if (!attackedEnemyList.TryGetValue(detected.gameObject.GetInstanceID(), out var t))
                {
                    temporaryDetectedList.Add(detected);
                    attackedEnemyList.Add(detected.gameObject.GetInstanceID(), detected.gameObject);
                }
            }

            //오브젝트가 하나라도 있다면?
            if (temporaryDetectedList.Any())
            {
                InvokeHitEvent(temporaryDetectedList);
                //m_isHitBoxChecked = true;
                Animator.SetBool(MonsterAttack.s_targetCheckAnimationKey, true);
            }
            //다음 공격 가능
            m_canNextAttack = true;
            temporaryDetectedList.Clear();
        }
    }

    #endregion

    #region HitBox

    private void OnDrawGizmosSelected()
    {
        attackHitBox.DrawGizmo(transform);
    }

    #endregion

}