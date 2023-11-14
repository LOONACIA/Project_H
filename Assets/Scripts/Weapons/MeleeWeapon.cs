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

    [SerializeField]
    private HitBoxList[] comboHitBox;

    private int m_hitEventIndex = 0;
    private bool m_isHitBoxChecked = false;

    //콤보 공격 관련 변수
    private bool m_canNextAttack = false;
    private int m_comboCount;
    [SerializeField] private int m_maxCombo;
    
    protected override void Attack()
    {
        //공격 중인데 다음 공격 딜레이가 오지 않았다면 return;
        if (IsAttacking && !m_canNextAttack) return;
        
        //콤보 카운팅, 콤보에 따라 애니메이션이 달라지므로 히트박스도 달라져야함
        if (IsAttacking)
        {
            m_comboCount += 1;
            if (m_comboCount >= comboHitBox.Length) m_comboCount = 0;
        }
        else
        {
            m_comboCount = 0;
        }
        
        Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
        //TODO: 1인칭일 경우 카메라 쉐이킹
        
        //공격 관련 변수 초기화
        IsAttacking = true;
        m_isHitBoxChecked = false;
        m_hitEventIndex = 0;
        
        //TODO: 무기별로 콤보 대기 타이밍이 다름. 현재는 애니메이션 기준이지만, 후에 시간 기준으로 변경할지 논의 필요.
        m_canNextAttack = false;
    }

    #region AnimationEvent
    
    protected override void OnAnimationEvent(object sender, EventArgs e)
    {
        //MeleeWeapon은 hit 판정 프레임 중 한번이라도 적에게 닿았다면 바로 취소합니다.
        if (m_isHitBoxChecked) return;
        
        //var hitBox = m_attackHitBoxes[m_hitEventIndex++ % m_attackHitBoxes.Count];
        var hitBox = comboHitBox[m_comboCount][m_hitEventIndex++ % comboHitBox.Length];
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
        foreach (var hitboxList in comboHitBox)
        {
            if (hitboxList.showGizmo)
            {
                foreach (var hitbox in hitboxList.hitBoxes)
                {
                    hitbox.DrawGizmo(transform);
                }
            }
        }
    }

    #endregion

    [Serializable]
    private class HitBoxList
    {
        public bool showGizmo = true;
        public HitBox[] hitBoxes;
        public HitBox this[int index] => hitBoxes[index];
    }    
    
}