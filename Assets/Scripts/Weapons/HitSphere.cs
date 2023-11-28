using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HitSphere
{
    [SerializeField]
    private Vector3 m_position = Vector3.zero;

    [SerializeField]
    private float m_radius = 1f;

    [SerializeField]
    private Color m_gizmoColor = Color.red;

    [SerializeField]
    private bool m_showGizmo = true;

    public Vector3 Position => m_position;
    
    public float Radius => m_radius;

    public Color GizmoColor => m_gizmoColor;
    
    public IEnumerable<Actor> DetectHitSphere(Transform parent)
    {
        return Physics.OverlapSphere(parent.TransformPoint(Position),
                                     Radius,
                                     LayerMask.GetMask("Monster"))
                      .Select(detectedObject => detectedObject.GetComponent<Actor>())
                      .Where(health => health != null);
    }

    public void DrawGizmo(Transform parent)
    {
        if (!m_showGizmo) return;
        
        Gizmos.color = GizmoColor;
        Matrix4x4 parentMat = Matrix4x4.TRS(parent.position, parent.rotation, Vector3.one);
        Matrix4x4 hitBoxMat = Matrix4x4.TRS(Position, Quaternion.identity, Vector3.one);
        Gizmos.matrix = parentMat * hitBoxMat;
        Gizmos.DrawWireSphere(Vector3.zero, Radius);
    }
}
