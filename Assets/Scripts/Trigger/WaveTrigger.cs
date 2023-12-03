using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    [SerializeField]
    private int m_leftMonster;

    [SerializeField]
    private GameObject[] m_gameObjects;
    
    public event EventHandler WaveStart;

    public event EventHandler WaveEnd;

    public ObservableCollection<Monster> Monsters { get; } = new();

    private void Awake()
    {
        Monsters.CollectionChanged += OnMonsterCollectionChanged;
    }

    private void OnMonsterCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

    private void OnMonsterDying(object sender, System.EventArgs e)
    {
        var count = Monsters.Count(monster => !monster.IsPossessed);
        
        // 마지막 몬스터가 죽을 때, 이벤트 발생
        if (count - m_leftMonster <= 1)
        {
            WaveEnd?.Invoke(this, EventArgs.Empty);
        }

        if (sender is not Monster monster)
        {
            return;
        }

        Monsters.Remove(monster);
    }

    private void OnTriggerEnter(Collider other)
    {
        var character = FindObjectsOfType<Monster>()
            .SingleOrDefault(monster => monster.IsPossessed);
        if (other.name == character.name)
        {
            WaveStart?.Invoke(this, EventArgs.Empty);

            foreach (var go in m_gameObjects)
            {
                go.SetActive(true);
            }
            //Destroy(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
    
    public enum SpawnMode
    {
        Time,
        Count,
    }
        
    [field: SerializeField]
    public SpawnMode Mode { get; private set; }
}
