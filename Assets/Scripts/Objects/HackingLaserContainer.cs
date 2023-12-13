using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class HackingLaserContainer : MonoBehaviour
{
    [Header("Move")]
    [SerializeField]
    private bool m_isOneWay = true;

    [SerializeField]
    private Vector3 m_startPos;

    [SerializeField]
    private Vector3 m_endPos;

    [SerializeField]
    private float m_moveSpeed = 5f;

    [Header("Rotate")]
    [SerializeField]
    private bool m_canRotate;

    [SerializeField]
    private Vector3 m_rotationVec;

    [SerializeField]
    private float m_rotationSpeed;

    private void Start()
    {
        m_startPos += transform.position;
        m_endPos += transform.position;
        transform.position = m_startPos;
    }

    private void Update()
    {
        Move();

        if (m_canRotate)
            Rotate();
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, m_endPos) < 0.1)
        {
            if (m_isOneWay)
            { 
                transform.position = m_startPos;
            }
            else
            {
                m_endPos = m_startPos;
                m_startPos = transform.position;
            }
        } 

        transform.position = Vector3.MoveTowards(transform.position, m_endPos, m_moveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        transform.Rotate(m_rotationVec * m_rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(m_startPos, Vector3.one);
        Gizmos.DrawWireCube(m_endPos, Vector3.one);
        Gizmos.DrawLine(m_startPos, m_endPos);
    }
}

