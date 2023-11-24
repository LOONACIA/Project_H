using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private GameObject m_owner;

    private Collider m_collider;

    private Action<WeaponAttackInfo> m_onHit;
	
    private bool m_isStopped;

    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!m_isStopped && Rigidbody.velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(Rigidbody.velocity.normalized);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == m_owner)
        {
            return;
        }
		
        m_isStopped = true;
        Rigidbody.isKinematic = true;
        m_collider.isTrigger = true;

        if (other.gameObject.TryGetComponent<Actor>(out var actor))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            WeaponAttackInfo attackInfo = new(actor, direction, other.collider.ClosestPoint(transform.position));
            m_onHit(attackInfo);
        }
    }

    public void Init(GameObject owner, Action<WeaponAttackInfo> onHit)
    {
        m_owner = owner;
        m_onHit = onHit;
    }
}
