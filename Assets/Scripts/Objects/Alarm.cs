using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 특정 범위 내 Actor의 해킹 여부를 감지하여 주변 Monster의 타겟 정보를 업데이트하는 컴포넌트
/// </summary>
public class Alarm : MonoBehaviour
{
    [SerializeField]
    [Tooltip("수신자를 감지하는 원점")]
    private Transform m_origin;

    [field: SerializeField]
    [Tooltip("수신자를 감지할 수 있는 최대 거리")]
    public float AlertRange { get; private set; } = 10f;

    private void Start()
    {
        if (m_origin == null)
        {
            m_origin = transform;
        }
    }

    public void Trigger(Actor target)
    {
        // Get recipients in alert range
        using var recipients = GameManager.Actor.GetMonstersInRadius(m_origin.position, AlertRange);

        // If target is possessed
        if (target.IsPossessed)
        {
            // Add actor to recipients' targets
            foreach (var recipient in recipients.Where(recipient => !recipient.Targets.Contains(target)))
            {
                recipient.Targets.Add(target);
            }
        }
        // Otherwise
        else
        {
            // Try remove actor from recipients' targets
            foreach (var recipient in recipients)
            {
                recipient.Targets.Remove(target);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (m_origin == null)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_origin.position, AlertRange);
    }

    private void OnValidate()
    {
        if (m_origin == null)
        {
            Debug.LogWarning($"{name} has no origin.", gameObject);
        }
    }
}
