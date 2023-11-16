using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BodyPartScript : MonoBehaviour
{
    public ActorHealth actorHealth;

    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;
    private MeshRenderer m_meshRenderer;

    private bool m_isReplaced;

    private void Start()
    {
        if (!TryGetComponent<Rigidbody>(out m_rigidbody))
        { 
            m_rigidbody = gameObject.AddComponent<Rigidbody>();
            m_rigidbody.isKinematic = true; 
        }

        if (!TryGetComponent<BoxCollider>(out m_collider))
        {
            m_collider = gameObject.AddComponent<BoxCollider>();
            m_collider.enabled = false;
        }

        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.enabled = false;

        if (!actorHealth)
        {
            actorHealth.Dying -= ReplaceBodyPart;
            actorHealth.Dying += ReplaceBodyPart;
        }
    }

    private void ReplaceBodyPart(object sender, EventArgs e)
    {
        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;
        m_meshRenderer.enabled = true;
        transform.parent = null;

        m_rigidbody.AddExplosionForce(15, transform.position, 5);
    }


    // temp
    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha2) && !m_isReplaced)
        {
            m_isReplaced = true;

            m_rigidbody.isKinematic = false;
            m_collider.enabled = true;
            m_meshRenderer.enabled = true;
            transform.parent = null;

            m_rigidbody.AddExplosionForce(1, transform.position, 1);
        }
    }
}
