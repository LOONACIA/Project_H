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

    [SerializeField]
    private List<HitBox> m_skillHitBoxes;

    private int m_attackHitBoxIndex = 0;

    private int m_skillHitBoxIndex = 0;

    protected override void OnAttackAnimationStart()
    {
        base.OnAttackAnimationStart();
        m_attackHitBoxIndex = 0;
    }

    protected override void OnSkillAnimationStart()
    {
        base.OnSkillAnimationStart();
        m_skillHitBoxIndex = 0;
    }

    protected virtual void OnAttackAnimationEvent()
    {
        var hitBox = m_attackHitBoxes[m_attackHitBoxIndex++ % m_attackHitBoxes.Count];
        if (hitBox == null)
        {
            Debug.LogError($"Attack is interrupted because hit box is null. {name}");
            return;
        }

        var detectedObjects = DetectHitBox(hitBox);
        OnAttackHit(detectedObjects);
    }

    protected virtual void OnSkillAnimationEvent()
    {
        var hitBox = m_skillHitBoxes[m_skillHitBoxIndex++ % m_skillHitBoxes.Count];
        if (hitBox == null)
        {
            Debug.LogError($"Skill is interrupted because hit box is null. {name}");
            return;
        }

        var detectedObjects = DetectHitBox(hitBox);
        OnSkillHit(detectedObjects);
    }

    private IEnumerable<IHealth> DetectHitBox(HitBox hitBox)
    {
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles + hitBox.Rotation.eulerAngles);

        return Physics.OverlapBox(transform.position + transform.TransformDirection(hitBox.Position),
                hitBox.HalfExtents, rotation, LayerMask.GetMask("Monster"))
            .Select(detectedObject => detectedObject.GetComponent<IHealth>())
            .Where(health => health != null);
    }

    private void OnDrawGizmosSelected()
    {
        DrawHitBoxGizmos(m_attackHitBoxes);
        DrawHitBoxGizmos(m_skillHitBoxes);
    }

    private void DrawHitBoxGizmos(IEnumerable<HitBox> hitBoxes)
    {
        foreach (var hitBox in hitBoxes)
        {
            Gizmos.color = hitBox.GizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale) *
                            (Matrix4x4.Translate(hitBox.Position) *
                             Matrix4x4.Rotate(Quaternion.Euler(hitBox.Rotation.eulerAngles)));
            Gizmos.DrawWireCube(Vector3.zero, hitBox.HalfExtents * 2f);
        }
    }

    [Serializable]
    private class HitBox
    {
        [SerializeField]
        private Vector3 m_position;

        [SerializeField]
        private Vector3 m_halfExtents;

        [SerializeField]
        private Quaternion m_rotation;

        [SerializeField]
        private Color m_gizmoColor = Color.red;

        public Vector3 Position => m_position;

        public Vector3 HalfExtents => m_halfExtents;

        public Quaternion Rotation => m_rotation;

        public Color GizmoColor => m_gizmoColor;
    }
}