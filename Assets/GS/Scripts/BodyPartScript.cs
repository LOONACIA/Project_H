using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BodyPartScript : MonoBehaviour
{
    public GameObject actor;

    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;
    private MeshRenderer m_meshRenderer;

    private bool m_isReplaced;

    private Coroutine m_coroutine;

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
    }

    public void ReplaceBodyPart()
    {
        if (m_isReplaced) return;

        m_isReplaced = true;
        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;
        m_meshRenderer.enabled = true;

        transform.parent = null;
        ExplodeBodyPart();

        if (m_coroutine == null)
            m_coroutine = StartCoroutine(DestroyBodyPart(5));
    }

    private void ExplodeBodyPart()
    {
        m_rigidbody.AddExplosionForce(15, transform.position, 5);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_collider.isTrigger = true;
            m_rigidbody.isKinematic = true;

            if (m_coroutine != null)
            { 
                StopCoroutine(m_coroutine);
                m_coroutine = StartCoroutine(DestroyBodyPart(3));
            }
        }
    }

    private IEnumerator DestroyBodyPart(float interver)
    { 
        yield return new WaitForSeconds(interver);
        Destroy(gameObject);
    }
}
