using LOONACIA.Unity.Coroutines;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FixedSpawner : MonoBehaviour, ISpawn
{
    [SerializeField, Tooltip("소환할 몬스터")]
    private GameObject m_monster;

    [SerializeField, Tooltip("첫 소환 지연 시간")]
    private float m_firstSpawnDelay;

    [SerializeField, Tooltip("반복해서 소환하는지 여부")]
    private bool m_useRepeat;

    [SerializeField, Tooltip("다음 소환까지의 지연 시간")]
    private float m_repeatSpawnDelay;

    [SerializeField, Tooltip("플레이어가 있는지 탐지하는 거리")]
    private float m_detectionRadius = 50;

    [SerializeField, Tooltip("소환 위치에 해당 레이어의 오브젝트가 존재하면 소환하지 않음")]
    private LayerMask m_layerToNotSpawnOn;

    [SerializeField, Tooltip("소환 최대 시도 횟수")]
    private float m_maxSpawnAttemp = 10;

    private int m_currentSpawnCount;

    private bool m_isActive;

    private bool m_isFirstSpawn;

    private BoxCollider m_collider;

    private CoroutineEx m_spawnCoroutine;

    private void Start()
    {
        TryGetComponent<BoxCollider>(out m_collider);
    }

    private void Evaluate()
    {
        if (!m_isActive) return;
        if (!CanDetect()) return;
        
        // 한 번도 소환한 적 없으면 바로 소환
        if (!m_isFirstSpawn) 
        {
            Spawn();    
        }

        // m_collider 안에 몬스터 있는지 확인 
        var monster = Physics.OverlapBox(m_collider.bounds.center, m_collider.bounds.extents, Quaternion.identity, LayerMask.GetMask("Monster"));

        // 없으면 소환
        if (monster.Length == 0 && m_spawnCoroutine == null)
        { 
            m_spawnCoroutine = CoroutineEx.Create(this, IE_WaitSpawnDelay());
        }
    }

    public void StartSpawn()
    {
        if (m_isActive) return;

        m_isActive = true;
    
        if (CanDetect())
            Invoke(nameof(Spawn), m_firstSpawnDelay);
        InvokeRepeating(nameof(Evaluate), m_firstSpawnDelay, 0.5f);
    }

    public void EndSpawn()
    {
        if (!m_isActive) return;

        m_isActive = false;

        if (m_spawnCoroutine != null)
            m_spawnCoroutine?.Abort();

        CancelInvoke(nameof(Evaluate));
    }

    private void Spawn()
    {
        if (!m_isActive) return;

        if (m_monster is null) return;

        if (!m_isFirstSpawn) 
            m_isFirstSpawn = true;

        if (!m_useRepeat && m_currentSpawnCount > 0)
        {
            EndSpawn();
            return;
        }

        m_spawnCoroutine = null;

        Vector3 spawnPosition = GetSpawnPos();
        if (spawnPosition == Vector3.zero)
            return;

        var monster = Instantiate(m_monster, spawnPosition, transform.rotation);

        m_currentSpawnCount++;
    }

    private Vector3 GetSpawnPos()
    {
        for (int i = 0; i < m_maxSpawnAttemp; i++)
        {
            var spawnPosXZ = m_collider.bounds.center;

            var groundCollier = Physics.Raycast(spawnPosXZ, Vector3.down, out var hit, 50, LayerMask.GetMask("Ground"));
            if (groundCollier)
            {
                var obstacleColliders = Physics.OverlapCapsule(spawnPosXZ, spawnPosXZ + new Vector3(0, ConstVariables.MONSTER_HEIGHT, 0), ConstVariables.MONSTER_RADIUS, m_layerToNotSpawnOn);
                if (obstacleColliders.Length == 0)
                {
                    return hit.point;
                }
            }
        }

        return Vector3.zero;
    }

    private bool CanDetect()
    {
        return Vector3.Distance(GameManager.Character.Controller.Character.transform.position, transform.position) < m_detectionRadius;
    }

    private IEnumerator IE_WaitSpawnDelay()
    {
        yield return new WaitForSeconds(m_repeatSpawnDelay);

        Spawn();
    }

}
