using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * 공격 범위 및 대상을 판정하는 클래스
 * 실제 대미지 처리는 MonsterAttack 클래스에서 이루어짐
 */
public class MeleeWeapon : Weapon
{
    [FormerlySerializedAs("m_hitBoxes")]
    [SerializeField]
    private List<HitBox> m_attackHitBoxes;

    private int m_attackHitBoxIndex = 0;

    #region AnimationEvent

    protected override void OnAnimationStart(object sender, EventArgs e)
    {
        m_attackHitBoxIndex = 0;
    }
    
    protected override void OnAnimationEvent(object sender, EventArgs e)
    {
        var hitBox = m_attackHitBoxes[m_attackHitBoxIndex++ % m_attackHitBoxes.Count];
        if (hitBox == null)
        {
            Debug.LogError($"Attack is interrupted because hit box is null. {name}");
            return;
        }

        var detectedObjects = hitBox.DetectHitBox(transform);
        //Debug.Log($"Col: {m_attackHitBoxIndex}{detectedObjects.Count()}");
        InvokeAttackHit(detectedObjects);
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