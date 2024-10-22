using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMachine : InteractableObject
{
    private readonly Dictionary<Transform, Transform> m_targets = new();
    
    private readonly WaitForFixedUpdate m_waitForFixedUpdateCache = new();

    private AudioSource m_audioSource;

    public AudioClip[] m_audios;
    private enum AudioIndex {Move};

    [SerializeField]
    [Tooltip("이동할 오브젝트")]
    private Transform m_target;

    [SerializeField]
    [Tooltip("이동할 목표 위치")]
    private Vector3 m_offset;

    [SerializeField]
    [Tooltip("이동 속도")]
    private float m_speed = 1f;

    [SerializeField]
    [Tooltip("목표 위치에 도달했는지 판단하는 오차 범위")]
    private float m_epsilon = 0.01f;

    private Transform m_originalParent;

    private Vector3 m_destination;

    private CoroutineEx m_moveCoroutine;

    public event EventHandler ElevatorMoveEnd;

    private void Awake()
    {
        m_destination = m_target.transform.position + m_offset;
    }

    private void Start()
    {
        var collisionDetector = m_target.gameObject.GetOrAddComponent<CollisionDetector>();
        collisionDetector.CollisionDetected += OnCollisionDetected;
        TryGetComponent<AudioSource>(out m_audioSource);
    }

    private void OnEnable()
    {
        m_originalParent = transform.parent;
    }

    private void OnDisable()
    {
        m_moveCoroutine?.Abort();
    }

    private void OnCollisionDetected(object sender, CollisionEvent e)
    {
        var other = e.Collision;
        switch (e.EventType)
        {
            case CollisionEvent.Type.Enter:
                m_targets.TryAdd(other.transform, other.gameObject.transform.parent);
                other.transform.SetParent(e.Sender.transform);
                break;
            case CollisionEvent.Type.Exit:
                if (m_targets.TryGetValue(other.transform, out var parent))
                {
                    other.transform.SetParent(parent);
                    m_targets.Remove(other.transform);
                }

                break;
        }
    }

    protected override void OnInteract(Actor actor)
    {
        // Sets the parent of the transform.
        transform.SetParent(m_target.transform);
        m_moveCoroutine = CoroutineEx.Create(this, Move());
        IsInteractable = false;
    }

    private IEnumerator Move()
    {
        // 시작 사운드
        m_audioSource.clip = m_audios[(int)AudioIndex.Move];
        m_audioSource.Play();

        float sqrEpsilon = m_epsilon * m_epsilon;
        float sqrDistance = (m_destination - m_target.transform.position).sqrMagnitude;
        while (sqrDistance > sqrEpsilon)
        {
            m_target.transform.position = Vector3.MoveTowards(m_target.transform.position, m_destination,
                m_speed * Time.fixedDeltaTime);
            sqrDistance = (m_destination - m_target.transform.position).sqrMagnitude;
            yield return m_waitForFixedUpdateCache;

            ////사운드 체크
            //if(m_audioSource.isPlaying == false)
            //{
            //    m_audioSource.clip = m_audios[(int)AudioIndex.Move];
            //    m_audioSource.Play();
            //    m_audioSource.loop = true;
            //}
        }
        m_target.transform.position = m_destination;
        ElevatorMoveEnd.Invoke(this, null);
        transform.SetParent(m_originalParent);

        //사운드
        m_audioSource.Stop();
    }

    private void OnDrawGizmosSelected()
    {
        if (m_target == null)
        {
            return;
        }

        // Draw line from target to destination for each vertex
        Gizmos.color = Color.yellow;
        var mesh = m_target.GetComponent<MeshFilter>().sharedMesh;
        var targetVertices = mesh.vertices;

        foreach (var vertex in targetVertices)
        {
            var vertexWorldPosition = m_target.transform.TransformPoint(vertex);
            Gizmos.DrawLine(vertexWorldPosition, vertexWorldPosition + m_offset);
        }

        Gizmos.DrawLine(m_target.transform.position, m_destination);

        // Draw mesh at destination
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh(mesh, m_destination, Quaternion.identity, m_target.transform.lossyScale);
    }
}