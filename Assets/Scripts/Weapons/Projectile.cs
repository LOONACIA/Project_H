using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject m_owner;
    
    private Rigidbody m_rigidbody;

    private Collider m_collider;
	
    private bool m_isStopped;
	
    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!m_isStopped && m_rigidbody.velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(m_rigidbody.velocity.normalized);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == m_owner)
        {
            return;
        }
		
        m_isStopped = true;
        m_rigidbody.isKinematic = true;
        m_collider.isTrigger = true;
    }

    public void Init(GameObject owner, Action<AttackInfo> onHit)
    {
        m_owner = owner;
    }
}
