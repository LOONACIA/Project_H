using System;
using UnityEngine;

public class FightZoneGate : MonoBehaviour
{
    [SerializeField]
    private WaveTrigger m_waveTrigger;

    private DissolveGate[] m_gates;

    private bool m_isUsed;

    private void Start()
    {
        m_waveTrigger.Triggered += OnWaveTriggered;
        m_waveTrigger.WaveEnd += Open;

        m_gates = GetComponentsInChildren<DissolveGate>();
    }

    private void OnWaveTriggered(object sender, EventArgs e)
    {
        if (m_isUsed)
        {
            return;
        }
        
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
