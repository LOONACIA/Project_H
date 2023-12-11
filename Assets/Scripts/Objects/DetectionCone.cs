using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Alarm))]
public class DetectionCone : MonoBehaviour
{
    private readonly List<Actor> m_targets = new();
    
    [SerializeField]
    [Tooltip("수신자 목록을 업데이트하는 간격")]
    private float m_updateInterval = 0.5f;

    private Alarm m_alarm;
    
    private Light m_light;
    
    private CoroutineEx m_updateCoroutine;
    
    private float m_coneAngle;
    
    private bool m_isEnabled;
    
    private void Awake()
    {
        m_alarm = GetComponent<Alarm>();
        m_light = GetComponent<Light>();
    }

    private void Start()
    {
        // Cone angle is half of spot angle
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
        
        // targets' capacity is maybe recipients' count + 1 (player)
        m_targets.Capacity = GameManager.Actor.GetMonsterCountInRadius(transform.position, m_alarm.AlertRange) + 1;
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
            actor.Dying += OnTargetDying;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
        {
            m_targets.Remove(actor);
            actor.Dying -= OnTargetDying;
        }
    }
    
    private void OnTargetDying(object sender, DamageInfo info)
    {
        if (sender is Actor actor)
        {
            m_targets.Remove(actor);
        }
    }

    private bool IsInCone(Vector3 targetPosition)
    {
        Transform coneTransform = transform;
        var direction = (targetPosition - coneTransform.position).normalized;
        return Vector3.Dot(coneTransform.forward, direction) >= m_coneAngle;
    }
    
    private IEnumerator CoDetect()
    {
        while (m_isEnabled)
        {
            // If actor is in cone
            foreach (var actor in m_targets.Where(actor => IsInCone(actor.transform.position)))
            {
                // And if actor is possessed
                if (actor.IsPossessed)
                {
                    // Show detection warning effect
                    GameManager.Effect.ShowDetectionWarningEffect();
                }
                
                // Trigger alarm
                m_alarm.Trigger(actor);
            }
            
            yield return new WaitForSeconds(m_updateInterval);
        }
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
