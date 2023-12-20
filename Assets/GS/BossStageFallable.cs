using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BossStageFallable : MonoBehaviour, IActivate
{
    private bool m_isActive;

    private Collider m_collider;

    private Rigidbody m_rigidBody;

    private void Start()
    {
        TryGetComponent<Collider>(out m_collider); 
        TryGetComponent<Rigidbody>(out m_rigidBody); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isActive && other.gameObject.TryGetComponent<Laser>(out _))
        {
            FallDown();
        }
    }

    public void Activate()
    {
        if (m_isActive) return;

        m_isActive = true;
    }
    public void Deactivate()
    {
        if (!m_isActive) return;

        m_isActive = false;
    }

    private void FallDown()
    {
        Deactivate();
        m_rigidBody.isKinematic = false;
        m_collider.isTrigger = true;
    }
}
