using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
    private bool m_showHitBox = false;

    public Vector3 Position => m_position;

    public Vector3 HalfExtents => m_halfExtents;

    public Quaternion Rotation => m_rotation;

    public Color GizmoColor => m_gizmoColor;
    
    public IEnumerable<IHealth> DetectHitBox(Transform parent)
    {
        Quaternion rotation = Quaternion.Euler(parent.eulerAngles + this.Rotation.eulerAngles);

        return Physics.OverlapBox(parent.position + parent.TransformDirection(this.Position),
                                  this.HalfExtents, rotation, LayerMask.GetMask("Monster"))
                      .Select(detectedObject => detectedObject.GetComponent<IHealth>())
                      .Where(health => health != null);
    }

    public void DrawHitBoxGizmo(Transform parent)
    {
        if (!m_showHitBox) return;
        
        Gizmos.color = this.GizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(parent.position, parent.rotation, parent.localScale) *
                        (Matrix4x4.Translate(this.Position) *
                         Matrix4x4.Rotate(Quaternion.Euler(this.Rotation.eulerAngles)));
        Gizmos.DrawWireCube(Vector3.zero, this.HalfExtents * 2f);
    }
}