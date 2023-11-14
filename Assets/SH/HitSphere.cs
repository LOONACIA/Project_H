using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitSphere : MonoBehaviour
{
    [Header("Sphere는 Scale 적용이 불가능합니다.")]
    [SerializeField]
    private float m_radius = 1f;

    [SerializeField]
    private Color m_gizmoColor = Color.red;

    [SerializeField]
    private bool m_showHitBox = false;

    public float Radius => m_radius;

    public Color GizmoColor => m_gizmoColor;

    //TODO: 템플릿화 가능한지 확인 후 템플릿화
    public IEnumerable<IHealth> DetectHitSphere()
    {
        return Physics.OverlapSphere(transform.position, Radius, LayerMask.GetMask("Monster"))
                      .Select(detectedObject => detectedObject.GetComponent<IHealth>())
                      .Where(health => health != null);
    }

    public void OnDrawGizmos()
    {
        if (!m_showHitBox) return;

        Gizmos.color = this.GizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireSphere(Vector3.zero, Radius);
    }
}