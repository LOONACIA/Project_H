using LOONACIA.Unity;
using RenownedGames.ApexEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BodyPartScript : MonoBehaviour
{
    //public bool isParent = false;

    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;
    //private MeshRenderer m_meshRenderer;

    // 몬스터 사망 시 파편이 날라감
    private float m_explosionForce = 15;
    private float m_explosionRadius = 5;

    // 몬스터 파편이 사라지는 시간을 관리하는 코루틴
    private Coroutine m_coroutine;

    // 몬스터 파편이 사라지는 시간
    private float m_initInterval = 5f;
    private float m_onGroundInterval = 3f;

    private void Start()
    {
        m_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
        m_rigidbody.isKinematic = true;

        m_collider = gameObject.GetOrAddComponent<BoxCollider>();
        m_collider.enabled = false;

        gameObject.SetActive(false);

        //m_meshRenderer = GetComponent<MeshRenderer>();

        //if( m_meshRenderer == null)
        //{
        //    isParent = true;
        //}
        //else
        //{
        //    m_meshRenderer.enabled = false;
        //}
    }

    public void ReplaceBodyPart(DamageInfo info)
    {
        if (m_rigidbody == null ||  m_collider == null) return;

        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;
        //_meshRenderer.enabled = true;

        transform.parent = null;
        ExplodeBodyPart(info.AttackDirection);

        if (m_coroutine == null)
            m_coroutine = StartCoroutine(DestroyBodyPart(m_initInterval));
    }

    private void ExplodeBodyPart(Vector3 attackDirection)
    {
        //m_rigidbody.AddExplosionForce(m_explosionForce, explosionPosition, m_explosionRadius);
        m_rigidbody.AddForce(attackDirection*m_explosionForce,ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //m_collider.isTrigger = true;
            //m_rigidbody.isKinematic = true;

            if (m_coroutine != null)
            { 
                StopCoroutine(m_coroutine);
                m_coroutine = StartCoroutine(DestroyBodyPart(m_onGroundInterval));
            }
        }
    }

    private IEnumerator DestroyBodyPart(float interver)
    { 
        yield return new WaitForSeconds(interver);
        Destroy(gameObject);
    }
}
