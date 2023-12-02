using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GateMachine : InteractableObject
{
    [SerializeField]
    private GameObject m_gate;

    [SerializeField, Range(0, 100)]
    private float m_alarmRadius = 20;

    private List<Monster> m_monsters = new();

    private IGate m_gateScript;

    private void Start()
    {
        m_gateScript = m_gate.GetComponent<IGate>();
    }

    public override void Interact(Actor actor, IProgress<float> progress, Action onComplete)
    {
        base.Interact(actor, progress, onComplete);

        Alarm();
    }

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        if (m_gate != null && m_gateScript != null)
        {
            StartCoroutine(m_gateScript.Open());
        }
    }

    private void Alarm()
    {
        FindMonsters();

        var character = m_monsters.SingleOrDefault(monster => monster.IsPossessed);

        foreach (var monster in m_monsters)
        {
            if (monster == character)
                continue;

            monster.Targets.Add(character);
        }
    }

    /// <summary>
    /// 범위 내에 있는 몬스터를 리스트에 저장
    /// </summary>
    private void FindMonsters()
    {
        var monsterColliders = Physics.OverlapSphere(transform.position, m_alarmRadius, LayerMask.GetMask("Monster"));

        m_monsters.Clear();

        foreach (var monsterCollider in monsterColliders) 
        {
            if (monsterCollider.gameObject.TryGetComponent<Monster>(out var monster))
            {
                m_monsters.Add(monster);
            }
        }
    }
}
