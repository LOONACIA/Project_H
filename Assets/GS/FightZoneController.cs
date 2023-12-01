using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FightZoneController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_gate;

    [SerializeField]
    private EnterZone m_enterZone;

    [SerializeField]
    private FightZone  m_fightZone;

    private DissolveGate[] m_gates;

    private bool m_isUsed;

    private void Start()
    {
        m_gates = m_gate.GetComponentsInChildren<DissolveGate>();

        if (m_enterZone == null)
            GetComponentInChildren<EnterZone>();

        if (m_fightZone == null)
            GetComponentInChildren<FightZone>();

        m_enterZone.OnEnter += Close;
        m_fightZone.OnClear += Open;
    }

    private void Close(object sender, EventArgs e)
    {
        if (m_isUsed == true) return;
        m_isUsed = true;

        foreach (var gate in m_gates)
        { 
            StartCoroutine(gate.Close());
        }

        m_fightZone.BeginFight();
    }

    private void Open(object sender, EventArgs e)
    {
        foreach (var gate in m_gates)
        {
            StartCoroutine(gate.Open());
        }
    }
}
