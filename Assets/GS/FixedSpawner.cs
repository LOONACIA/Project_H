using LOONACIA.Unity.Coroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FixedSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("소환할 몬스터")]
    private GameObject m_monster;

    [SerializeField, Tooltip("다음 소환까지의 지연 시간")]
    private float m_spawnDelay;

    [SerializeField, Tooltip("소환 위치에 해당 레이어의 오브젝트가 존재하면 소환하지 않음")]
    private LayerMask m_layerToNotSpawnOn;

    [SerializeField, Tooltip("소환 최대 시도 횟수")]
    private float m_maxSpawnAttemp = 10;

    private bool m_active;

    private BoxCollider m_collider;

    private CoroutineEx m_spawnCoroutine;

    private void Start()
    {
        TryGetComponent<BoxCollider>(out m_collider);
    }

    private void Evaluate()
    {
        // m_collider 안에 몬스터 있는지 확인 
        var target = Physics.OverlapBox(m_collider.bounds.center, m_collider.bounds.extents, Quaternion.identity, LayerMask.GetMask("Monster"));

        // 없으면 소환
        if (target.Length == 0)
            m_spawnCoroutine = CoroutineEx.Create(this, IE_WaitSpawnDelay()); 
    }

    [ContextMenu(nameof(StartSpawn))]
    public void StartSpawn()
    {
        if (m_active) return;

        m_active = true;
        Spawn();
        InvokeRepeating(nameof(Evaluate), 0, 0.5f);
    }

    public void EndSpawn()
    {
        if (!m_active) return;

        m_active = false;

        if (m_spawnCoroutine != null)
            m_spawnCoroutine?.Abort();

        CancelInvoke(nameof(Evaluate));
    }

    private void Spawn()
    {
        if (!m_active) return;

        if (m_monster is null) return;

        Vector3 spawnPosition = GetSpawnPos();
        if (spawnPosition == Vector3.zero)
            return;

        var monster = Instantiate(m_monster, spawnPosition, transform.rotation);

        m_spawnDelay++;
    }

    private Vector3 GetSpawnPos()
    {
        float constHeight = 2f;
        float constRadius = 0.5f;

        for (int i = 0; i < m_maxSpawnAttemp; i++)
        {
            var spawnPosXZ = m_collider.bounds.center;

            var groundCollier = Physics.Raycast(spawnPosXZ, Vector3.down, out var hit, 50, LayerMask.GetMask("Ground"));
            if (groundCollier)
            {
                var obstacleColliders = Physics.OverlapCapsule(spawnPosXZ, spawnPosXZ + new Vector3(0, constHeight, 0), constRadius, m_layerToNotSpawnOn);
                if (obstacleColliders.Length == 0)
                {
                    return hit.point;
                }
            }
        }

        return Vector3.zero;
    }

    private IEnumerator IE_WaitSpawnDelay()
    {
        yield return new WaitForSeconds(m_spawnDelay);

        Spawn();
    }
}
