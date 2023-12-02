using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class DetectionCone : MonoBehaviour
{
    private readonly List<Actor> m_targets = new();
    
    [SerializeField]
    [Tooltip("수신자 목록을 업데이트하는 간격")]
    private float m_updateInterval = 0.5f;
    
    [SerializeField]
    [Tooltip("수신자를 감지할 수 있는 최대 거리")]
    private float m_alertRange = 10f;
    
    private CoroutineEx m_updateCoroutine;
    
    private float m_coneAngle;

    private Light m_light;
    
    private bool m_isEnabled;
    
    private void Awake()
    {
        m_light = GetComponent<Light>();
    }

    private void Start()
    {
        // Cone angle is half of spot angle
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
        
        // targets' capacity is maybe recipients' count + 1 (player)
        m_targets.Capacity = GameManager.Actor.GetMonsterCountInRadius(transform.position, m_alertRange) + 1;
        m_updateCoroutine = CoroutineEx.Create(this, CoDetect());
    }
    
    private void OnEnable()
    {
        m_isEnabled = true;
    }
    
    private void OnDisable()
    {
        m_isEnabled = false;
        m_updateCoroutine?.Abort();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If other is not an actor
        if (!other.TryGetComponent<Actor>(out var actor))
        {
            // return
            return;
        }

        if (!m_targets.Contains(actor))
        {
            m_targets.Add(actor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
        {
            m_targets.Remove(actor);
        }
    }

    private bool IsInCone(Vector3 targetPosition)
    {
#if UNITY_EDITOR
        // For debugging
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);  
#endif
        Transform coneTransform = transform;
        var direction = (targetPosition - coneTransform.position).normalized;
        return Vector3.Dot(coneTransform.forward, direction) >= m_coneAngle;
    }
    
    private IEnumerator CoDetect()
    {
        while (m_isEnabled)
        {
            // Get recipients in alert range
            using var recipients = GameManager.Actor.GetMonstersInRadius(transform.position, m_alertRange);
            
            // If actor is in cone
            foreach (var actor in m_targets.Where(actor => IsInCone(actor.transform.position)))
            {
                // And if actor is possessed
                if (actor.IsPossessed)
                {
                    GameManager.Effect.ShowDetectionWarningEffect();
                    // Add actor to recipients' targets
                    foreach (var recipient in recipients.Where(recipient => !recipient.Targets.Contains(actor)))
                    {
                        recipient.Targets.Add(actor);
                    }
                }
                // If actor is not possessed
                else
                {
                    // Remove actor from recipients' targets
                    foreach (var recipient in recipients)
                    {
                        recipient.Targets.Remove(actor);
                    }
                }
            }
            
            yield return new WaitForSeconds(m_updateInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_alertRange);
    }

    private void OnValidate()
    {
        if (!TryGetComponent<Collider>(out _))
        {
            Debug.LogWarning($"{name} has no collider.");
        }
        
        if (!TryGetComponent<Light>(out _))
        {
            Debug.LogWarning($"{name} has no light.");
        }
    }
}
