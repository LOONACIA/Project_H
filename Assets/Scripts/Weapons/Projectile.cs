using LOONACIA.Unity.Managers;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private GameObject m_owner;

    private Collider m_collider;

    private Action<AttackInfo> m_onHit;

    private bool m_isStopped;

    private Vector3 m_initialPosition;

    private float m_range;

    private LayerMask m_aimLayers;
    
    private int m_damage;

    public event EventHandler OnEnableComponents;
    public event EventHandler OnDisableComponents;

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
        
        if (other.gameObject.TryGetComponent<IHealth>(out var health))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 direction = (hitPoint - m_initialPosition).normalized;
            AttackInfo attackInfo = new(m_owner, health, m_damage, hitPoint, direction);
            m_onHit(attackInfo);
            
            if (health.CurrentHp <= 0)
            {
                return;
            }
        }
        
        DisableComponents();
        ManagerRoot.Resource.Release(gameObject);
    }

    public void Init(GameObject owner, int damage, float range, LayerMask aimLayers, Action<AttackInfo> onHit)
    {
        m_owner = owner;
        m_damage = damage;
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
        OnEnableComponents?.Invoke(this, EventArgs.Empty);
    }

    private void DisableComponents()
    {
        m_isStopped = true;
        Rigidbody.isKinematic = true;
        m_collider.enabled = false;
        OnDisableComponents?.Invoke(this, EventArgs.Empty);
    }
}