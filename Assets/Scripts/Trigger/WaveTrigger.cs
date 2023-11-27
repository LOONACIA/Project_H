using LOONACIA.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public readonly ObservableCollection<Monster> Monsters = new();
    [SerializeField]
    private int m_leftMonster;
    [SerializeField]
    private GameObject[] m_gameObjects;
    private CharacterController playerCharacter;

    private void Awake()
    {
        Monsters.CollectionChanged += OnMonsterCollectionChanged;
        playerCharacter = GetComponent<CharacterController>();
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
        // 스폰된 몬스터들 전부 죽음(플레이어 빼고)
        var count = Monsters.Count(monster => !monster.IsPossessed);
        if (count <= m_leftMonster)
        {

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
            foreach (var go in m_gameObjects)
            {
                go.SetActive(true);
            }
            Destroy(this);
        }
    }
}
