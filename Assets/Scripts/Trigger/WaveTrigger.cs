using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    private enum SpawnMode
    {
        Time,
        Count
    }

    [SerializeField]
    private SpawnMode m_spawnMode;

    [SerializeField]
    [Tooltip("진입 후 첫 웨이브까지 대기 시간")]
    private float m_waitTime;

    [SerializeField]
    [Tooltip("SpawnMode가 Time일 때, 웨이브 간격")]
    private float m_waveInterval;

    [SerializeField]
    [Tooltip("SpawnMode가 Count일 때, 남은 몬스터의 수가 이 값보다 작거나 같으면 다음 웨이브로 넘어감.")]
    private float m_encounterCount;

    [SerializeField]
    [Tooltip("스폰 횟수. 하위 객체 Spawner의 WaveInfoList 길이에 따라 무시될 수 있음.")]
    private int m_waveCount;

    private Spawner[] m_spawners;

    private int m_currentWaveIndex;

    private float m_lastSpawnTime;

    private CoroutineEx m_evaluationCoroutine;

    private bool m_isOnSpawn;

    private bool m_isTriggered;

    public event EventHandler WaveStart;

    public event EventHandler WaveEnd;

    public ObservableCollection<Monster> Monsters { get; } = new();

    private void Awake()
    {
        m_spawners = GetComponentsInChildren<Spawner>();
    }

    private void Start()
    {
        Monsters.CollectionChanged += OnMonsterCollectionChanged;
    }

    private void OnEnable()
    {
        m_isTriggered = false;
    }

    private void OnDisable()
    {
        m_evaluationCoroutine?.Abort();
    }

    private void StartSpawn()
    {
        if (m_spawnMode == SpawnMode.Time)
        {
            m_evaluationCoroutine = CoroutineEx.Create(this, CoEvaluate());
        }

        WaveStart?.Invoke(this, EventArgs.Empty);
        Spawn();
    }

    private IEnumerator CoEvaluate()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(0.1f);
            Evaluate();
        }
    }

    private void Evaluate()
    {
        // If already on spawn
        if (m_isOnSpawn)
        {
            // Ignore
            return;
        }
        
        switch (m_spawnMode)
        {
            case SpawnMode.Time:
                if (Time.time - m_lastSpawnTime >= m_waveInterval)
                {
                    Spawn();
                }

                break;
            case SpawnMode.Count:
                if (Monsters.Count(monster => !monster.IsPossessed) <= m_encounterCount)
                {
                    Spawn();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Spawn()
    {
        if (m_currentWaveIndex++ >= m_waveCount)
        {
            StopSpawn();
            return;
        }
        
        m_isOnSpawn = true;

        foreach (var spawner in m_spawners)
        {
            spawner.Spawn();
        }

        m_lastSpawnTime = Time.time;
        m_isOnSpawn = false;
    }

    private void StopSpawn()
    {
        WaveEnd?.Invoke(this, EventArgs.Empty);
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

    private void OnMonsterDying(object sender, EventArgs e)
    {
        if (sender is Monster monster)
        {
            Monsters.Remove(monster);
        }

        Evaluate();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If already triggered
        if (m_isTriggered)
        {
            // Ignore
            return;
        }
        
        // If other is not an actor or not possessed
        if (!other.TryGetComponent<Actor>(out var actor))
        {
            // Ignore
            return;
        }

        // If actor is monster and not in the list
        if (actor is Monster monster && !Monsters.Contains(monster))
        {
            // Add monster to the list
            Monsters.Add(monster);
        }

        // Trigger spawn
        m_isTriggered = true;
        StartCoroutine(CoWaitSpawn());
    }
    
    private IEnumerator CoWaitSpawn()
    {
        yield return new WaitForSeconds(m_waitTime);
        StartSpawn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}