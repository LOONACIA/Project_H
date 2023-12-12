using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class HackingLaserController : MonoBehaviour
{
    [SerializeField]
    private Laser[] m_lasers;

    [SerializeField]
    private HackingConsole m_hackingConsole;

    [SerializeField]
    private float m_hackingCoolTime = 3f;

    [SerializeField]
    private Vector3 m_startPos;

    [SerializeField]
    private Vector3 m_endPos;

    [SerializeField]
    private float m_moveSpeed = 5f;

    private bool m_isHacking;

    private void Start()
    {
        if (m_hackingConsole == null)
            m_hackingConsole = GetComponentInChildren<HackingConsole>();

        if (m_lasers == null || m_lasers.Length == 0)
            m_lasers = GetComponentsInChildren<Laser>(true);

        m_hackingConsole.Interacted += Hacking;

        m_startPos += transform.position;
        m_endPos += transform.position;
        transform.position = m_startPos;
    }

    private void Update()
    {
        Move();
    }

    private void Hacking(object sender, EventArgs e)
    {
        if (m_isHacking) return;

        m_isHacking = true;

        foreach (var laser in m_lasers)
        {
            laser.Hacking();
        }

        Invoke(nameof(Recovery), m_hackingCoolTime);
    }

    private void Recovery()
    {
        m_isHacking = false;

        foreach (var laser in m_lasers)
        {
            laser.Recovery();
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, m_endPos) < 0.1)
            transform.position = m_startPos;

        transform.position = Vector3.MoveTowards(transform.position, m_endPos, m_moveSpeed * Time.deltaTime);
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
