using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BossStageSpawner : MonoBehaviour, ISpawn
{
    [SerializeField]
    private WaveInfoList[] m_waveInfoList;

    [Header("Spawn Data")]
    [SerializeField, Tooltip("플레이어와 몬스터 소환 위치의 최소 거리")]
    private float m_minSpawnRadius = 10f;

    [SerializeField, Tooltip("플레이어와 몬스터 소환 위치의 최대 거리")]
    private float m_maxSpawnRadius = 20f;

    [SerializeField, Tooltip("소환 최대 시도 횟수")]
    private float m_maxSpawnAttemp = 10;

    [SerializeField, Tooltip("남은 몬스터 수가 minEncounterCount보다 작으면 다음 소환 준비")]
    private float m_minEncounterCount;    
    
    [SerializeField, Tooltip("남은 몬스터 수가 maxEncounterCount보다 크면 소환 하지 않음")]
    private float m_maxEncounterCount;

    [SerializeField, Tooltip("다음 소환까지의 지연 시간")]
    private float m_spawnDelay;

    [SerializeField, Tooltip("소환 후 일정 시간이 지나면 다시 소환")]
    private float m_spawnInterval;

    [SerializeField, Tooltip("최대 소환 횟수 적용 여부")]
    private bool m_useSpawnCountLimit;

    [SerializeField, Tooltip("최대 소환 횟수")]
    private int m_maxSpawnCount;

    [SerializeField, Tooltip("소환 위치에 해당 레이어의 오브젝트가 존재하면 소환하지 않음")]
    private LayerMask m_layerToNotSpawnOn;

    private int m_currentSpawnIndex;

    private Actor m_character;

    private bool m_active;

    private float m_lastSpawnTime;

    [Tooltip("현재까지 스폰한 횟수")]
    private int m_currentSpawnCount;

    private Coroutine m_spawnCoroutine;

    public ObservableCollection<Monster> Monsters { get; } = new();

    private void Start()
    {
        Monsters.CollectionChanged += OnMonsterCollectionChanged;
    }

    public void StartSpawn()
    {
        if (m_active) return;

        m_active = true;
        m_currentSpawnIndex = 0;
        Spawn();
        InvokeRepeating(nameof(Evaluate), 0, 0.5f);
    }

    public void EndSpawn()
    {
        if (!m_active) return;

        m_active = false;

        if (m_spawnCoroutine != null)
            StopCoroutine(m_spawnCoroutine);

        CancelInvoke(nameof(Evaluate));
    }

    private void OnMonsterCollectionChanged(object sender,
       System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var monster in e.OldItems.OfType<Monster>())
            {
                monster.Dying -= OnMonsterDying;
            }
        }

        if (e.NewItems != null)
        {
            foreach (var monster in e.NewItems.OfType<Monster>())
            {
                monster.Dying += OnMonsterDying;
            }
        }
    }

    private void OnMonsterDying(object sender, in AttackInfo info)
    {
        if (sender is Monster monster)
        {
            Monsters.Remove(monster);
        }

        Evaluate();
    }

    private void Evaluate()
    {
        // 이미 소환 대기 중이면 반환
        if (m_spawnCoroutine != null) return;

        // 최대 몬스터 수를 넘겼으면 반환
        if (Monsters.Count > m_maxEncounterCount) return;

        if (Monsters.Count < m_minEncounterCount 
            || Time.time - m_lastSpawnTime > m_spawnInterval)
        {
            m_spawnCoroutine = StartCoroutine(IE_WaitSpawnDelay());
        }
    }

    private void Spawn()
    {
        if (!m_active) return;

        if (m_spawnCoroutine != null)
            m_spawnCoroutine = null;

        if (m_currentSpawnIndex >= m_waveInfoList.Length)
            m_currentSpawnIndex = 0;

        if (m_useSpawnCountLimit
            && m_currentSpawnCount >= m_maxSpawnCount)
        {
            return;
        }
        
        // 마지막 스폰 시간 저장
        m_lastSpawnTime = Time.time;

        // 몬스터 스폰
        foreach (var waveInfo in m_waveInfoList[m_currentSpawnIndex].WaveInfos)
        {
            if (waveInfo.Monster is null)
            {
                continue;
            }

            int i = 0;
            while (i++ < waveInfo.Count)
            {
                Vector3 spawnPosition = GetSpawnPos();
                if (spawnPosition == Vector3.zero)
                    continue;

                var monster = Instantiate(waveInfo.Monster, spawnPosition, transform.rotation);
                if (monster.TryGetComponent<Monster>(out var component))
                {
                    Monsters.Add(component);
                }
            }
        }

        m_currentSpawnIndex++;
        m_currentSpawnCount++;
    }

    private Vector3 GetSpawnPos()
    {
        // 도넛 형태에서 좌표 반환
        m_character = GameManager.Character.Controller.Character;

        for (int i = 0; i < m_maxSpawnAttemp; i++)
        {
            Vector2 randomXZPos = Random.insideUnitCircle.normalized * Random.Range(m_minSpawnRadius, m_maxSpawnRadius);
            Vector3 spawnPos = new Vector3(randomXZPos.x, 0, randomXZPos.y);
            spawnPos.y = ConstVariables.MONSTER_SPAWN_HEIGHT;
            spawnPos += m_character.transform.position;

            var obstacleColliders = Physics.OverlapCapsule(spawnPos, spawnPos + new Vector3(0, ConstVariables.MONSTER_HEIGHT, 0), ConstVariables.MONSTER_RADIUS, m_layerToNotSpawnOn);
            if (obstacleColliders.Length == 0)
            {
                var groundCollier = Physics.Raycast(spawnPos, Vector3.down, out var hit, 50, LayerMask.GetMask("Ground"));
                if (groundCollier)
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

    [System.Serializable]
    private class WaveInfo
    {
        [field: SerializeField]
        public GameObject Monster { get; private set; }

        [field: SerializeField]
        public int Count { get; private set; }
    }

    [System.Serializable]
    private class WaveInfoList
    {
        [field: SerializeField]
        public WaveInfo[] WaveInfos { get; private set; }
    }
}
