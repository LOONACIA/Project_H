using LOONACIA.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectionCone : MonoBehaviour
{
    private readonly List<Actor> m_targets = new();
    
    [SerializeField]
    private List<Monster> m_recipients;
    
    private float m_coneAngle;

    private Light m_light;
    
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
        m_targets.Capacity = m_recipients.Count + 1;
    }

    private void FixedUpdate()
    {
        foreach (var actor in m_targets.Where(actor => IsInCone(actor.transform.position)))
        {
            if (actor.IsPossessed)
            {
                foreach (var recipient in m_recipients)
                {
                    recipient.Targets.Remove(actor);
                }
            }
            else
            {
                foreach (var recipient in m_recipients.Where(recipient => !recipient.Targets.Contains(actor)))
                {
                    recipient.Targets.Add(actor);
                }
            }
        }
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
        Transform @transform = this.transform;
        var direction = (targetPosition - @transform.position).normalized;
        return Vector3.Dot(@transform.forward, direction) >= m_coneAngle;
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
