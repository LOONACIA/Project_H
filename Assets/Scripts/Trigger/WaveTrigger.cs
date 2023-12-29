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

    [SerializeField]
    private bool m_usePosseionClear = true;

    private PossessionProcessor m_processor;

    private Spawner[] m_spawners;

    private int m_currentWaveIndex;

    private float m_lastSpawnTime;

    private CoroutineEx m_evaluationCoroutine;

    private bool m_isTriggered;

    private WaitForSeconds m_waitForSecondsCache = new(0.1f);

    private Collider m_collider;

    public event EventHandler Triggered;

    public event EventHandler WaveStart;

    public event EventHandler WaveEnd;

    public ObservableCollection<Monster> Monsters { get; } = new();

    private void Awake()
    {
        m_processor = FindObjectOfType<PossessionProcessor>();
        m_spawners = GetComponentsInChildren<Spawner>();
    }

    private void Start()
    {
        Monsters.CollectionChanged += OnMonsterCollectionChanged;

        TryGetComponent<Collider>(out m_collider);
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
            yield return m_waitForSecondsCache;
            Evaluate();
        }
    }

    private void Evaluate()
    {
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

        foreach (var spawner in m_spawners)
        {
            spawner.Spawn();
        }

        m_lastSpawnTime = Time.time;
    }

    private void StopSpawn()
    {
        m_evaluationCoroutine?.Abort();
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

        if (!Monsters.Any(monster => !monster.IsPossessed))
        {
            WaveEnd?.Invoke(this, EventArgs.Empty);
        }
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
        if (actor.IsPossessed)
        {
            m_isTriggered = true;
            AddInitialMonsters();
            Triggered?.Invoke(this, EventArgs.Empty);
            StartCoroutine(CoWaitSpawn());
        }
    }

    private void AddInitialMonsters()
    {
        if (m_collider == null)
            return;

        var monsterColliders = Physics.OverlapBox(m_collider.bounds.center, m_collider.bounds.size / 2, Quaternion.identity, LayerMask.NameToLayer("Montser"));
        foreach (var monsterCollider in monsterColliders)
        {
            if (monsterCollider.TryGetComponent<Monster>(out var monster) && !monster.IsPossessed)
            {
                Monsters.Add(monster);
            }
        }
    }

    private IEnumerator CoWaitSpawn()
    {
        if (m_usePosseionClear)
            m_processor.ClearTarget();

        yield return new WaitForSeconds(m_waitTime);
        StartSpawn();
    }
}