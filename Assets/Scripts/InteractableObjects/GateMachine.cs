using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alarm))]
public class GateMachine : InteractableObject, IHealth
{
    [SerializeField]
    private GameObject[] m_gate;

    [SerializeField]
    private bool m_alarmWhenOpen = true;

    [SerializeField]
    private bool m_useShield;

    [SerializeField]
    private Shield m_shield;

    private List<IGate> m_gateList = new();
    
    private Alarm m_alarm;

    public event RefEventHandler<AttackInfo> Damaged { add { } remove { } }
    public event RefEventHandler<AttackInfo> Dying { add { } remove { } }
    public event EventHandler Died { add { } remove { } }

    public int CurrentHp => 1;

    public int MaxHp => 1;

    private void Start()
    {
        foreach (var gate in m_gate)
        {
            if (gate.TryGetComponent<IGate>(out var gateScript))
                m_gateList.Add(gateScript);
        }

        m_alarm = GetComponentInChildren<Alarm>();

        if (m_useShield == true)
        { 
            m_shield = GetComponentInChildren<Shield>();

            if (m_shield != null) 
            {
                IsInteractable = false;
            }
        }
    }

    public void Kill()
    {
    }

    public void TakeDamage(in AttackInfo damageInfo)
    {
        if (!m_useShield) return;
        if (m_shield == null || !m_shield.IsValid) return;
        if (damageInfo.Attacker == null) return;

        if (damageInfo.Attacker.TryGetComponent<Actor>(out var actor) && actor.IsPossessed)
        {
            m_shield?.TakeDamage(damageInfo);

            if (!m_shield.IsValid)
            {
                IsInteractable = true;
            }
        }
    }

    protected override void OnInteractStart(Actor actor)
    {
        if (m_alarmWhenOpen)
        {
            m_alarm.Trigger(actor);
        }
    }

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        foreach (var gate in m_gateList) 
        {
            StartCoroutine(gate.Open());
        }
    }
}
