using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FightZoneGate : MonoBehaviour
{
    [SerializeField]
    private WaveTrigger m_waveTrigger;

    private DissolveGate[] m_gates;

    private bool m_isUsed;

    private void Start()
    {
        m_waveTrigger.WaveStart += Close;
        m_waveTrigger.WaveEnd += Open;

        m_gates = GetComponentsInChildren<DissolveGate>();
    }

    private void Close(object sender, EventArgs e)
    {
        if (m_isUsed == true) return;
        m_isUsed = true;

        foreach (var gate in m_gates)
        { 
            StartCoroutine(gate.Close());
        }
    }

    private void Open(object sender, EventArgs e)
    {
        foreach (var gate in m_gates)
        {
            StartCoroutine(gate.Open());
        }
    }
}
