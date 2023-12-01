using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateState
{ 
    Close,
    Open,
}

public class DissolveGate : MonoBehaviour, IGate
{
    [SerializeField]
    private GateState m_state;

    [SerializeField]
    private GameObject m_gate;

    [SerializeField]
    private Material m_originMaterial;

    [SerializeField]
    private Material m_dissolveMaterial;

    [SerializeField]
    private float m_progressTime;

    private Collider m_gateCollider;
    
    private Renderer m_gateRenderer;


    public IEnumerator Open()
    {
        if (m_gate == null) yield break;

        if (m_state == GateState.Open) yield break;
            m_state = GateState.Open;

        if (m_gateCollider != null)
        {
            m_gateCollider.isTrigger = true;
        }

        if (m_gateRenderer != null && m_dissolveMaterial != null)
        {
            m_gateRenderer.material = m_dissolveMaterial;

            yield return StartCoroutine(Dissolve(true));
        }

        Destroy(m_gate);
    }

    public IEnumerator Close()
    {
        if (m_gate == null) yield break;

        if (m_state == GateState.Close) yield break;
            m_state = GateState.Close;

        if (m_gateCollider != null)
        {
            m_gateCollider.isTrigger = false;
        }

        if (m_gateRenderer != null && m_originMaterial != null)
        {
            m_gateRenderer.enabled = true;
            m_gateRenderer.material = m_originMaterial;

            yield return StartCoroutine(Dissolve(false));
        }
    }

    private void Start()
    {
        m_gateCollider = m_gate.GetComponent<Collider>();
        m_gateRenderer = m_gate.GetComponent<Renderer>();

        if (m_state == GateState.Open)
        {
            m_gateCollider.isTrigger = true;
            m_gateRenderer.enabled = false;
        }
        else
        { 
            m_gateCollider.isTrigger = false;
            m_gateRenderer.enabled = true;
        }
    }

    private IEnumerator Dissolve(bool value)
    {
        float time = 0;

        while (time < m_progressTime)
        {
            time += Time.deltaTime;

            float ratio;
            if (value == true)
                ratio = Mathf.Lerp(0, 1, time / m_progressTime);
            else
                ratio = Mathf.Lerp(1, 0, time / m_progressTime);

            m_gateRenderer.material.SetFloat("_DissolveAmount", ratio);

            yield return null;
        }
    }
}
