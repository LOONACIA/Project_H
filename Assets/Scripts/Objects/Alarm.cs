using LOONACIA.Unity.Collections;
using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 범위 내 Actor의 해킹 여부를 감지하여 주변 Monster의 타겟 정보를 업데이트하는 컴포넌트
/// </summary>
public class Alarm : MonoBehaviour
{
    // Monster Layer
    private static int s_recipientLayers = 1 << 7;
    
    [SerializeField]
    [Tooltip("수신자를 감지하는 원점")]
    private Transform m_origin;

    private Collider m_collider;

    [field: SerializeField]
    [Tooltip("원점에 Collider가 있을 경우 Collider를 사용하여 수신자를 감지합니다.")]
    public bool UseCollider { get; private set; }
    
    [field: SerializeField]
    [Tooltip("수신자를 감지할 수 있는 레이어. UseCollider가 true일 경우에만 적용됩니다.")]
    public LayerMask RecipientLayers { get; private set; }

    [field: SerializeField]
    [Tooltip("수신자를 감지할 수 있는 최대 거리")]
    public float AlertRange { get; private set; } = 10f;

    private void Start()
    {
        if (m_origin == null)
        {
            m_origin = transform;
        }

        if (UseCollider && m_origin.TryGetComponent<Collider>(out var origin))
        {
            m_collider = origin;
        }
    }

    public void Trigger(Actor target)
    {
        PooledList<Monster> recipients;

        if (UseCollider)
        {
            Collider[] colliders = ArrayPool<Collider>.Shared.Rent(GameManager.Actor.GetMonsterCount());

            int recipientLayers = RecipientLayers > 0 ? RecipientLayers : s_recipientLayers;
            Bounds bounds = m_collider.bounds;
            
            int length = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, colliders,  m_origin.transform.rotation,
                recipientLayers);

            recipients = new(length);
            foreach (Collider recipient in colliders.AsSpan(0, length))
            {
                if (recipient.TryGetComponent<Monster>(out var monster))
                {
                    recipients.Add(monster);
                }
            }

            ArrayPool<Collider>.Shared.Return(colliders);
        }
        else
        {
            // Get recipients in alert range
            recipients = GameManager.Actor.GetMonstersInRadius(m_origin.position, AlertRange);
        }

        AlarmTarget(recipients.AsSpan(), target);
        recipients.Dispose();
    }

    private static void AlarmTarget(Span<Monster> recipients, Actor target)
    {
        // If target is possessed
        if (target.IsPossessed)
        {
            foreach (var recipient in recipients)
            {
                // Clear existing targets
                recipient.Targets.Clear();

                // Add actor to recipients' targets
                if (!recipient.Targets.Contains(target))
                {
                    recipient.Targets.Add(target);
                }
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
        if (UseCollider)
        {
            m_collider ??= m_origin.GetComponent<Collider>();
            Gizmos.DrawWireCube(m_origin.position, m_collider.bounds.size);
        }
        else
        {
            Gizmos.DrawWireSphere(m_origin.position, AlertRange);
        }
    }

    private void OnValidate()
    {
        if (m_origin == null)
        {
            Debug.LogWarning($"{name} has no alarm origin. Origin will be set to self.", gameObject);
        }
    }
}