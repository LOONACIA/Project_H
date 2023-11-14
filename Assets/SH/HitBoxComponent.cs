using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitBoxComponent : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_halfExtents = new Vector3(1f, 1f, 1f);

    [SerializeField]
    private Color m_gizmoColor = Color.red;

    [SerializeField]
    private bool m_showHitBox = false;

    public Vector3 HalfExtents => m_halfExtents;

    public Color GizmoColor => m_gizmoColor;

    //TODO: 템플릿화 가능한지 확인 후 템플릿화
    public IEnumerable<IHealth> DetectHitBox()
    {
        return Physics.OverlapBox(transform.position,
                                  Vector3.Scale(HalfExtents, transform.localScale),
                                  transform.rotation,
                                  LayerMask.GetMask("Monster"))
                      .Select(detectedObject => detectedObject.GetComponent<IHealth>())
                      .Where(health => health != null);
    }

    public void OnDrawGizmos()
    {
        if (!m_showHitBox) return;

        Gizmos.color = this.GizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, this.HalfExtents * 2f);
    }
}