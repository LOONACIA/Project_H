using LOONACIA.Unity.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorManager
{
    private readonly List<Actor> m_monsters = new();
    
    public void Clear()
    {
        m_monsters.Clear();
    }
    
    public void AddActor(Actor monster)
    {
        if (m_monsters.Contains(monster))
        {
            return;
        }
        
        m_monsters.Add(monster);
    }
    
    public void RemoveActor(Actor monster)
    {
        m_monsters.Remove(monster);
    }
    
    public int GetMonsterCount()
    {
        return m_monsters.Count;
    }

    public int GetMonsterCountInRadius(Vector3 center, float radius)
    {
        float sqrRadius = radius * radius;
        return m_monsters.Count(monster => (center - monster.transform.position).sqrMagnitude <= sqrRadius);
    }
    
    public PooledList<Monster> GetMonstersInRadius(Vector3 center, float radius)
    {
        int capacity = GetMonsterCountInRadius(center, radius);
        var monsters = new PooledList<Monster>(capacity);
        float sqrRadius = radius * radius;
        foreach (var monster in m_monsters.OfType<Monster>().Where(monster => (center - monster.transform.position).sqrMagnitude <= sqrRadius))
        {
            monsters.Add(monster);
        }

        return monsters;
    }
}
