using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveGateScript : MonoBehaviour, IGate
{
    [SerializeField]
    private GameObject m_gate;

    [SerializeField]
    private Material m_dissolveMaterial;

    [SerializeField]
    private float m_progressTime;

    private Collider m_gateCollider;
    private Renderer m_gateRenderer;

    public IEnumerator Open()
    {
        if (m_gate == null) yield break;

        if (m_gateCollider != null)
        {
            m_gateCollider.isTrigger = true;
        }

        if (m_gateRenderer != null && m_dissolveMaterial != null)
        {
            m_gateRenderer.material = m_dissolveMaterial;

            yield return StartCoroutine(Dissolve());
        }

        Destroy(m_gate);
    }

    private void Start()
    {
        m_gateCollider = m_gate.GetComponent<Collider>();
        m_gateRenderer = m_gate.GetComponent<Renderer>();
    }

    private IEnumerator Dissolve()
    {
        float time = 0;

        while (time < m_progressTime)
        {
            time += Time.deltaTime;

            m_gateRenderer.material.SetFloat("_DissolveAmount", Mathf.Lerp(0, 1, time / m_progressTime));

            yield return null;
        }
    }
}
