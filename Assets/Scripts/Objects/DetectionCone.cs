using LOONACIA.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
    }

    private void FixedUpdate()
    {
        foreach (var actor in m_recipients.Where(actor => IsInCone(actor.transform.position)))
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
        // Actor가 아니면
        if (!other.TryGetComponent<Actor>(out var actor))
        {
            // 무시
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
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
        var direction = (targetPosition - transform.position).normalized;
        return Vector3.Dot(transform.forward, direction) >= m_coneAngle;
    }
}
