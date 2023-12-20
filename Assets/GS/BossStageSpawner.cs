using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BossStageSpawner : MonoBehaviour
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

    [SerializeField, Tooltip("남은 몬스터 수가 encounterCount보다 작으면 다음 소환 준비 ")]
    private float m_encounterCount;

    [SerializeField, Tooltip("소환과 소환 사이의 간격")]
    private float m_spawnInterval;

    [SerializeField, Tooltip("소환 위치에 해당 레이어의 오브젝트가 존재하면 소환하지 않음")]
    private LayerMask m_layerToNotSpawnOn;

    private int m_currentSpawnIndex;

    private Actor m_character;

    private bool m_active;

    private float m_lastSpawnTime;

    private ObservableCollection<Monster> Monsters { get; } = new();

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
    }

    //public void EndSpawn()
    //{
    //    if (!m_active) return;

    //    m_active = false;
    //}

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
        // 남은 몬스터 수 확인, 일정 수 이하면 m_spawnInterval 시간 뒤에 소환
        if (Monsters.Count < m_encounterCount)
        { 
            Invoke(nameof(Spawn), m_spawnInterval);    
        }
    }

    private void Spawn()
    {
        if (!m_active) return;

        if (m_currentSpawnIndex >= m_waveInfoList.Length)
            m_currentSpawnIndex = 0;

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
    }

    private Vector3 GetSpawnPos()
    {
        // 도넛 형태에서 좌표 반환
        m_character = GameManager.Character.Controller.Character;

        // temp
        float spawnHeight = 5;
        float constHeight = 2f;
        float constRadius = 0.5f;

        for (int i = 0; i < m_maxSpawnAttemp; i++)
        {
            Vector2 randomXZPos = Random.insideUnitCircle * Random.Range(m_minSpawnRadius, m_maxSpawnRadius);
            Vector3 spawnPos = new Vector3(randomXZPos.x, 0, randomXZPos.y);
            spawnPos.y = Random.Range(-spawnHeight, spawnHeight);
            spawnPos += m_character.transform.position;

            var obstacleColliders = Physics.OverlapCapsule(spawnPos, spawnPos + new Vector3(0, constHeight, 0), constRadius, m_layerToNotSpawnOn);
            if (obstacleColliders.Length == 0)
            {
                // temp
               var temp = Physics.RaycastAll(spawnPos, Vector3.down, spawnHeight);
                Debug.DrawRay(spawnPos, spawnPos + Vector3.down, Color.red);
               var groundCollier = Physics.Raycast(spawnPos, Vector3.down, spawnHeight, LayerMask.GetMask("Ground"));
                if (groundCollier)
                {
                    Debug.Log(Vector3.Distance(spawnPos, m_character.transform.position));
                    return spawnPos;
                }
            }
        }

        return Vector3.zero;
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
