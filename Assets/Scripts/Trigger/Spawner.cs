using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    [FormerlySerializedAs("m_waveInfos")]
    [SerializeField]
    private WaveInfoList[] m_waveInfoList;

    private WaveTrigger m_waveTrigger;

    private int m_currentSpawnIndex;

    private Collider m_collider;

    private void Start()
    {
        m_waveTrigger = GetComponentInParent<WaveTrigger>();
        m_collider = GetComponent<Collider>();
    }

    public void Spawn()
    {
        if (m_currentSpawnIndex >= m_waveInfoList.Length)
        {
            m_currentSpawnIndex++;
            return;
        }

        if (m_waveInfoList[m_currentSpawnIndex].WaveInfos is not { Length: > 0 })
        {
            m_currentSpawnIndex++;
            return;
        }

        EnemySpawn();
    }

    private void EnemySpawn()
    {
        foreach (var waveInfo in m_waveInfoList[m_currentSpawnIndex].WaveInfos)
        {
            if (waveInfo.Monster is null)
            {
                continue;
            }

            int i = 0;
            while (i++ < waveInfo.Count)
            {
                Vector3 spawnPosition = GetRandomSpawnPos(m_collider);
                var monster = Instantiate(waveInfo.Monster, this.transform.position, transform.rotation);
                if (monster.TryGetComponent<Monster>(out var component))
                {
                    m_waveTrigger.Monsters.Add(component);
                }
            }
        }
        m_currentSpawnIndex++;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// GetRandomPointInCollider에서 전달받은 좌표 내 Layer로 충돌 검출 후 좌표 전달
    /// </summary>
    /// <param name="spawnableAreaCollider"></param>
    /// <returns></returns>
    private Vector3 GetRandomSpawnPos(Collider spawnableAreaCollider)
    {
        Vector3 spawnPosition = Vector3.zero;
        bool isSpawnPosValid = false;

        int attemptCount = 0;
        int maxAttempts = 200;

        int layerToNotSpawnOn = LayerMask.GetMask("Obstacle", "Wall", "Ground", "Monster");
        while (!isSpawnPosValid && attemptCount < maxAttempts)
        {
            float constHeight = 0.5f;
            spawnPosition = GetRandomPointInCollider(spawnableAreaCollider);
            Vector3 pos1 = new Vector3(spawnPosition.x, spawnPosition.y - constHeight, spawnPosition.z);
            Vector3 pos2 = new Vector3(spawnPosition.x, spawnPosition.y + constHeight, spawnPosition.z);
            Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, constHeight);

            bool isInvalidCollision = false;
            foreach (Collider collider in colliders)
            {
                if ((layerToNotSpawnOn & (1 << collider.gameObject.layer)) != 0)
                {
                    isInvalidCollision = true;
                    break;
                }
            }

            if (!isInvalidCollision)
            {
                isSpawnPosValid = true;
            }

            attemptCount++;
        }

        if (!isSpawnPosValid)
        {
            Debug.LogWarning("정상 스폰 위치 찾을 수 없음.");
        }

        return spawnPosition;
    }

    /// <summary>
    /// 콜라이더 범위 내 좌표 전달
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Vector3 GetRandomPointInCollider(Collider collider, float offset = 1f)
    {
        Bounds collBounds = collider.bounds;

        Vector3 minBounds = new(collBounds.min.x + offset, collBounds.min.y + offset, collBounds.min.z + offset);
        Vector3 maxBounds = new(collBounds.max.x - offset, collBounds.max.y - offset, collBounds.max.z - offset);

        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        float randomZ = Random.Range(minBounds.z, maxBounds.z);

        return new(randomX, randomY, randomZ);
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
