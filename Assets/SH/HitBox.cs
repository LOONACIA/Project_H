using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class HitBox
{
    [SerializeField]
    private Vector3 m_position;

    [SerializeField]
    private Vector3 m_halfExtents;

    [SerializeField]
    private Quaternion m_rotation;

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
        Quaternion rotation = Quaternion.Euler(parent.eulerAngles + Rotation.eulerAngles);

        return Physics.OverlapBox(parent.TransformPoint(Position),
                                  HalfExtents, parent.rotation * Rotation, LayerMask.GetMask("Monster"))
                      .Select(detectedObject => detectedObject.GetComponent<IHealth>())
                      .Where(health => health != null);
    }

    public void DrawGizmo(Transform parent)
    {
        if (!m_showGizmo) return;
        
        Gizmos.color = GizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(parent.TransformPoint(Position),
                                      parent.rotation * Rotation,
                                      Vector3.one);
        //Debug.Log($"pos: {parent.TransformPoint(Position)}, {parent.rotation * Rotation}");
        Gizmos.DrawWireCube(Vector3.zero, HalfExtents * 2f);

    }
    
    
}