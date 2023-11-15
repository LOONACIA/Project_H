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

    [Header("About Slash Direction")]
    [SerializeField]
    private Vector2[] m_attackDirections;
    private int m_attackCount = 0;

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

        attackedEnemyList.Clear();
        Animator.SetBool(MonsterAttack.s_targetCheckAnimationKey, false);
        
        //공격 카운트 +1
        m_attackCount += 1;
        if (m_attackCount >= m_attackDirections.Length) m_attackCount = 0;
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
                AttackInfo info = new AttackInfo();
                info.damage = 5;
                
                info.attackDirection = transform.TransformDirection(
                    new Vector3(m_attackDirections[m_attackCount].x,
                        m_attackDirections[m_attackCount].y,
                        0f)).normalized;
                Debug.Log($"{info.attackDirection}");
                InvokeHitEvent(info, temporaryDetectedList);
                //m_isHitBoxChecked = true;
                Animator.SetBool(MonsterAttack.s_targetCheckAnimationKey, true);
                
            }
            //다음 공격 가능
            temporaryDetectedList.Clear();
        }
    }

    #endregion

    #region HitBox

    private void OnDrawGizmos()
    {
        if (m_attackCount < m_attackDirections.Length)
        {
            Vector2 dir = m_attackDirections[m_attackCount];
            //dir = dir.normalized * 10.0f;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                transform.TransformPoint(+dir.x,+dir.y,2.0f),
                transform.TransformPoint(-dir.x,-dir.y,2.0f)
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        attackHitBox.DrawGizmo(transform);
        
        Gizmos.matrix = Matrix4x4.identity;
        foreach (var dir in m_attackDirections)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                transform.TransformPoint(+dir.x,+dir.y,2.0f),
                transform.TransformPoint(-dir.x,-dir.y,2.0f)
                );
        }
    }

    #endregion

}