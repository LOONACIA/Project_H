using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HitBox
{
    [SerializeField]
    private Vector3 m_position = Vector3.zero;

    [SerializeField]
    private Vector3 m_halfExtents = Vector3.one;

    [SerializeField]
    private Quaternion m_rotation = Quaternion.identity;

    [SerializeField]
    private Color m_gizmoColor = Color.red;

    [SerializeField]
    private bool m_showGizmo = true;

    public Vector3 Position => m_position;

    public Vector3 HalfExtents => m_halfExtents;

    public Quaternion Rotation => m_rotation;

    public Color GizmoColor => m_gizmoColor;

    public IEnumerable<IHealth> DetectHitBox(Transform parent)
    {
        //Debug.Log($"forward: {parent.forward} rotation: {parent.rotation * Rotation}");
        return Physics.OverlapBox(parent.TransformPoint(Position),
                HalfExtents, parent.rotation * Rotation, LayerMask.GetMask("Monster", "Damageable"))
            .Select(detectedObject => detectedObject.GetComponent<IHealth>())
            .Where(health => health != null);
    }

    public void DrawGizmo(Transform parent)
    {
        if (!m_showGizmo)
        {
            return;
        }

        Gizmos.color = GizmoColor;
        Matrix4x4 parentMat = Matrix4x4.TRS(parent.position, parent.rotation, Vector3.one);
        Matrix4x4 hitBoxMat = Matrix4x4.TRS(Position, Rotation, Vector3.one);
        // Gizmos.matrix = Matrix4x4.TRS(parent.TransformPoint(Position),
        //                               parent.rotation,
        //                               Vector3.one);
        Gizmos.matrix = parentMat * hitBoxMat;
        //Debug.Log($"pos: {parent.TransformPoint(Position)}, {parent.rotation * Rotation}");
        Gizmos.DrawWireCube(Vector3.zero, HalfExtents * 2f);
    }
}