using LOONACIA.Unity.Coroutines;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Spawner: MonoBehaviour
{
    public GameObject spawnEnemy;

    public int enemyCount;

    [SerializeField]
    private Collider m_spawnPos;

    [SerializeField]
    private float m_spawnDelay;

    private int m_spawnedEnemy = 0;

    private CoroutineEx m_enemySpawnCoroutine;

    private WaveTrigger m_waveTrigger;

    // Start is called before the first frame update
    private void OnEnable()
    {
        m_spawnPos = this.transform.GetComponent<Collider>();
        Monster monster = GetComponent<Monster>();
        m_enemySpawnCoroutine = CoroutineEx.Create(this, EnemySpawn(m_spawnPos));
        m_waveTrigger = GetComponentInParent<WaveTrigger>();
        if (m_spawnedEnemy == enemyCount)
        {
            if (m_enemySpawnCoroutine?.IsRunning is true)
            {
                m_enemySpawnCoroutine.Abort();
                m_enemySpawnCoroutine = null;
                this.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 전달 받은 최종 좌표로 프리팹 인스턴스화
    /// </summary>
    /// <param name="spawnableAreaCollider"></param>
    /// <returns></returns>
    IEnumerator EnemySpawn(Collider spawnableAreaCollider)
    {
        yield return new WaitForSeconds(m_spawnDelay);
        var character = FindObjectsOfType<Monster>()
            .SingleOrDefault(monster => monster.IsPossessed);

        while (m_spawnedEnemy < enemyCount)
        {
            Vector3 spawnPosition = GetRandomSpawnPos(spawnableAreaCollider);
            var go = Instantiate(spawnEnemy, spawnPosition, Quaternion.identity);
            if (character != null && go.TryGetComponent<Monster>(out var monster))
            {
                monster.Targets.Add(character);
                m_waveTrigger.Monsters.Add(monster);
            }

            yield return new WaitForSeconds(0.1f);
            m_spawnedEnemy++;
        }
    }

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

        int layerToNotSpawnOn = LayerMask.GetMask("Obstacle", "Wall","Ground","Monster");
        while (!isSpawnPosValid && attemptCount < maxAttempts)
        {
            float constHeight = 0.5f;
            spawnPosition = GetRandomPointInCollider(spawnableAreaCollider);
            Vector3 pos1 = new Vector3(spawnPosition.x, spawnPosition.y - constHeight, spawnPosition.z);
            Vector3 pos2 = new Vector3(spawnPosition.x, spawnPosition.y + constHeight, spawnPosition.z);
            Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, constHeight);

            bool isInvalidCollision = false;
            foreach(Collider collider in colliders)
            {
                if((layerToNotSpawnOn & ( 1<< collider.gameObject.layer)) != 0)
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

        Vector3 minBounds = new Vector3(collBounds.min.x + offset, collBounds.min.y + offset, collBounds.min.z + offset);
        Vector3 maxBounds = new Vector3(collBounds.max.x - offset, collBounds.max.y - offset, collBounds.max.z - offset);

        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        float randomZ = Random.Range(minBounds.z, maxBounds.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
