using System;
using UnityEngine;

public class HackingLaserContainer : MonoBehaviour
{
    [Header("Move")]
    [SerializeField, Tooltip("레이저가 이동하는지 여부")]
    private bool m_canMove;

    [SerializeField, Tooltip("편도로 가는지, 왕복하는지 여부")]
    private bool m_isOneWay = true;

    [SerializeField, Tooltip("HackingLaserContainer의 출발 위치")]
    private Vector3 m_startPos;

    [SerializeField, Tooltip("HackingLaserContainer의 도착 위치")]
    private Vector3 m_endPos;

    [SerializeField, Tooltip("HackingLaserContainer의 이동 속도")]
    private float m_moveSpeed = 5f;

    [Header("Rotate")]
    [SerializeField, Tooltip("레이저가 회전하는지 여부")]
    private bool m_canRotate;

    [SerializeField, Tooltip("레이저가 회전하는 방향")]
    private Vector3 m_rotationVec;

    [SerializeField, Tooltip("레이저가 회전하는 속도")]
    private float m_rotationSpeed;

    [Header("Consoles")]
    [SerializeField, Tooltip("동기화되는 콘솔 배열")]
    private SynchronizedConsole[] m_consoles;

    private void Start()
    {
        m_startPos += transform.position;
        m_endPos += transform.position;
        transform.position = m_startPos;

        foreach (var console in m_consoles)
        { 
            console.Register();
        }
    }

    private void Update()
    {
        if (m_canMove)
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
        transform.Rotate(m_rotationVec * (m_rotationSpeed * Time.deltaTime));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(m_startPos, Vector3.one);
        Gizmos.DrawWireCube(m_endPos, Vector3.one);
        Gizmos.DrawLine(m_startPos, m_endPos);
    }

    [Serializable]
    private class SynchronizedConsole
    {
        public HackingConsole[] hackingConsoles;

        public void Register()
        {
            foreach (var console in hackingConsoles)
            {
                console.OnHacking += Hacking;
            }
        }
        
        private void Hacking(object sender, EventArgs e)
        {
            foreach (var console in hackingConsoles)
            {
                console.Hacking();
            }
        }
    }
}
