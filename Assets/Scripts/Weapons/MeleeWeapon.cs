using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Melee Weapon: 기본적인 공격 클래스입니다.
 * 4연타가 존재하며, 각 공격 타격 중 실제 hit가 발생한다면 공격이 interrupt됩니다.
 */
public class MeleeWeapon : Weapon
{
    [FormerlySerializedAs("m_hitBoxes")]
    [SerializeField]
    private List<HitBox> m_attackHitBoxes;

    private int m_hitEventIndex = 0;
    private bool m_isHitBoxChecked = false;
    
    protected override void Attack()
    {
        Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
        //TODO: 1인칭일 경우 카메라 쉐이킹
        
        //공격 관련 변수 초기화
        IsAttacking = true;
        m_isHitBoxChecked = false;
        m_hitEventIndex = 0;
    }

    #region AnimationEvent
    
    protected override void OnAnimationEvent(object sender, EventArgs e)
    {
        //MeleeWeapon은 hit 판정 프레임 중 한번이라도 적에게 닿았다면 바로 취소합니다.
        if (m_isHitBoxChecked) return;
        
        var hitBox = m_attackHitBoxes[m_hitEventIndex++ % m_attackHitBoxes.Count];
        if (hitBox == null)
        {
            Debug.LogError($"Attack is interrupted because hit box is null. {name}");
            return;
        }

        //내 몬스터와 다른 대상만 가져옴
        var detectedObjects 
            = hitBox.DetectHitBox(transform)
                    .Where(hit=>hit.gameObject!=Owner.gameObject);

        //오브젝트가 하나라도 있다면?
        if (detectedObjects.Any())
        {
            InvokeHitEvent(detectedObjects);
            m_isHitBoxChecked = true;
        }
    }

    protected override void OnAnimationEnd(object sender, EventArgs e)
    {
        IsAttacking = false;
    }

    #endregion
    
    #region HitBox

    private void OnDrawGizmosSelected()
    {
        foreach (var hitbox in m_attackHitBoxes)
        {
            hitbox.DrawGizmo(transform);
        }
    }

    #endregion
}