using LOONACIA.Unity.Managers;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private GameObject m_owner;

    private Collider m_collider;

    private Action<WeaponAttackInfo> m_onHit;
	
    private bool m_isStopped;

    private Vector3 m_initialPosition;

    private float m_range;
    
    private LayerMask m_aimLayers;

    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        EnableComponents();
    }

    private void Update()
    {
        if (!m_isStopped && Rigidbody.velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(Rigidbody.velocity.normalized);
            
            if (Vector3.Distance(m_initialPosition, transform.position) > m_range)
            {
                DisableComponents();
                ManagerRoot.Resource.Release(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == m_owner)
        {
            return;
        }
        
        int bit = 1 << other.gameObject.layer;
        if ((m_aimLayers.value & bit) == 0)
        {
            return;
        }
		
        DisableComponents();
        transform.SetParent(other.transform);

        if (other.gameObject.TryGetComponent<Actor>(out var actor))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            WeaponAttackInfo attackInfo = new(actor, direction, other.ClosestPoint(transform.position));
            m_onHit(attackInfo);
        }
    }

    public void Init(GameObject owner, float range, LayerMask aimLayers, Action<WeaponAttackInfo> onHit)
    {
        m_owner = owner;
        m_initialPosition = transform.position;
        m_range = range;
        m_aimLayers = aimLayers;
        m_onHit = onHit;
    }
    
    private void EnableComponents()
    {
        m_isStopped = false;
        Rigidbody.isKinematic = false;
        m_collider.enabled = true;
    }
    
    private void DisableComponents()
    {
        m_isStopped = true;
        Rigidbody.isKinematic = true;
        m_collider.enabled = false;
    }
}
