using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    public Vector3 direction = Vector3.forward;
    public float speed = 3.0f;
    public GameObject explosionVfx;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_rigidbody.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")
            ||other.gameObject.layer==LayerMask.NameToLayer("Monster"))
        {
            GameObject v = Instantiate(explosionVfx, transform.position, transform.rotation);
            v.transform.localScale *= 5f;
            v.SetActive(true);
            Destroy(v, 2f);
            Destroy(gameObject);
        }
    }
    
    
}
