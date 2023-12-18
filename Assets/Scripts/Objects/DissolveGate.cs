using System.Collections;
using UnityEngine;

public class DissolveGate : MonoBehaviour, IGate
{
    private enum GateState
    {
        Close,
        Open,
    }

    [SerializeField]
    private GateState m_state;

    [SerializeField]
    private GameObject m_gate;

    private Material m_appearMaterial;

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

        gameObject.layer = LayerMask.NameToLayer("Default");

        if (m_gateCollider != null)
        {
            m_gateCollider.isTrigger = true;
        }

        if (m_gateRenderer != null && m_dissolveMaterial != null)
        {
            m_gateRenderer.material = m_dissolveMaterial;

            yield return StartCoroutine(Dissolve(true));
        }

    }

    public IEnumerator Close()
    {
        if (m_gate == null) yield break;

        if (m_state == GateState.Close) yield break;
            m_state = GateState.Close;

        gameObject.layer = LayerMask.NameToLayer("Wall");

        if (m_gateCollider != null)
        {
            m_gateCollider.isTrigger = false;
        }

        if (m_gateRenderer != null && m_appearMaterial != null)
        {
            m_gateRenderer.enabled = true;
            m_gateRenderer.material = m_appearMaterial;

            yield return StartCoroutine(Dissolve(false));
        }

    }

    private void Start()
    {
        m_gateCollider = m_gate.GetComponent<Collider>();
        m_gateRenderer = m_gate.GetComponent<Renderer>();

        m_appearMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.GATE_APPEAR_MATERIAL_PATH));
        m_dissolveMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.GATE_DISSOLVE_MATERIAL_PATH));

        if (m_state == GateState.Open)
        {
            m_gateCollider.isTrigger = true;
            m_gateRenderer.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        { 
            m_gateCollider.isTrigger = false;
            m_gateRenderer.enabled = true;
            gameObject.layer = LayerMask.NameToLayer("Wall");
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
