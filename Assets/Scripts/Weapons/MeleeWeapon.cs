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

    [Header("각 공격에 대한 정보. attackData의 Length만큼 공격 수가 있다고 가정합니다.")]
    [SerializeField]
    private MeleeAttackData[] m_attackData;
    
    public int ComboCount => Animator.GetInteger(ComboAnimBehaviour.s_attackCountAnimationHash);

    [SerializeField] private TrailCaster m_trailCaster;

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
        m_trailCaster.StartCheck();
    }

    protected override void OnFollowThroughMotion()
    {
        IsAttacking = false;
        m_trailCaster.EndCheck();
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
            // var detectedObjects
            //     = attackHitBox.DetectHitBox(transform)
            //                   .Where(hit => hit.gameObject != Owner.gameObject);
            
            //트레일렌더러
            var detectedObjects
                 = m_trailCaster.PopBuffer()
                                .Select(detectedObject => detectedObject.transform.GetComponent<IHealth>())
                                .Where(health => health != null)
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
                Vector2 slashDir = m_attackData[ComboCount].slashDirection;
                
                AttackInfo info = new AttackInfo();
                info.damage = 5;
                info.attackDirection = transform.TransformDirection(new Vector3(slashDir.x, slashDir.y, 0f)).normalized;
                
                InvokeHitEvent(info, temporaryDetectedList);
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
        if (Animator != null)
        {
            if (ComboCount < m_attackData.Length)
            {
                Vector2 dir = m_attackData[ComboCount].slashDirection;
                //dir = dir.normalized * 10.0f;
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(
                    transform.TransformPoint(+dir.x,+dir.y + 1.0f,2.0f),
                    transform.TransformPoint(-dir.x,-dir.y + 1.0f,2.0f)
                );
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        attackHitBox.DrawGizmo(transform);
        
        Gizmos.matrix = Matrix4x4.identity;
        foreach (var d in m_attackData)
        {
            Vector2 dir = d.slashDirection;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                transform.TransformPoint(+dir.x,+dir.y + 1.0f,2.0f),
                transform.TransformPoint(-dir.x,-dir.y + 1.0f,2.0f)
                );
        }
    }

    #endregion

    #region AttackData

    [Serializable]
    private class MeleeAttackData
    {
        public MonsterAttackData data;
        public Vector2 slashDirection;
    }

    #endregion

}