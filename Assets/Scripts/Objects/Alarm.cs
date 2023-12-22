using LOONACIA.Unity.Collections;
using System;
using System.Buffers;
using UnityEngine;

/// <summary>
/// 특정 범위 내 Actor의 해킹 여부를 감지하여 주변 Monster의 타겟 정보를 업데이트하는 컴포넌트
/// </summary>
public class Alarm : MonoBehaviour
{
    // Monster Layer
    private static readonly int s_recipientLayers = 1 << 7;

    [SerializeField]
    [Tooltip("수신자를 감지하는 원점")]
    private Transform m_origin;

    [SerializeField]
    [Tooltip("캐릭터 변경 시, 새로운 캐릭터를 추적할 때까지의 지연 여부")]
    private bool m_useDelay;

    [SerializeField]
    [Tooltip("캐릭터 변경 시, 새로운 캐릭터를 추적할 때까지의 지연 시간")]
    private float m_updateTargetDelay = 3;

    private BoxCollider m_collider;

    private bool m_hasBoxCollider;

    private Actor m_lastAlarmCharacter;

    private Actor m_currentAlarmCharacter;

    private float m_lastAlarmTime;

    [field: SerializeField]
    [Tooltip("원점에 BoxCollider가 있을 경우 BoxCollider를 사용하여 수신자를 감지합니다.")]
    public bool UseBoxCollider { get; private set; }

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

        m_hasBoxCollider = false;
        if (UseBoxCollider && m_origin.TryGetComponent<BoxCollider>(out var origin))
        {
            m_collider = origin;
            m_hasBoxCollider = true;
        }
    }

    public void Trigger(Actor target)
    {
        if (target.IsPossessed)
        {
            m_currentAlarmCharacter = target;
        }

        if (m_lastAlarmCharacter == m_currentAlarmCharacter && m_updateTargetDelay > Time.time - m_lastAlarmTime)
        {
            return;
        }

        if (target.IsPossessed && target != m_lastAlarmCharacter)
        {
            m_lastAlarmCharacter = target;
            m_lastAlarmTime = Time.time;
            return;
        }

        PooledList<Monster> recipients;

        if (m_hasBoxCollider)
        {
            Collider[] colliders = ArrayPool<Collider>.Shared.Rent(GameManager.Actor.GetMonsterCount());

            int recipientLayers = RecipientLayers > 0 ? RecipientLayers : s_recipientLayers;

            Vector3 halfExtents = Vector3.Scale(m_collider.size / 2f, m_origin.lossyScale);
            int length = Physics.OverlapBoxNonAlloc(m_origin.position + m_collider.center, halfExtents,
                colliders, m_origin.rotation, recipientLayers);

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
        if (UseBoxCollider)
        {
            if (m_collider == null)
            {
                m_collider = m_origin.GetComponent<BoxCollider>();
            }

            Gizmos.matrix = Matrix4x4.TRS(m_origin.position, m_origin.rotation, m_origin.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, m_collider.size);
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