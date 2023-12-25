using DG.Tweening.Core.Easing;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
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

    private NavMeshObstacle m_obstacle;

    public IEnumerator Open()
    {
        yield return null;

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

        m_obstacle.enabled = false;
    }

    public IEnumerator Close()
    {
        yield return null;

        if (m_gate == null) yield break;

        if (m_state == GateState.Close)
        {
            yield break;
        }
        
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

        m_obstacle.enabled = true;
    }

    private void Start()
    {
        m_gateCollider = m_gate.GetComponent<Collider>();
        m_gateRenderer = m_gate.GetComponent<Renderer>();
        TryGetComponent<NavMeshObstacle>(out m_obstacle);

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
        yield return null;

        float time = 0;

        //사운드 재생
        GetComponent<AudioSource>().Stop();
        GameManager.Sound.PlayClipAt(GameManager.Sound.SFXObjectData.GateOpen, transform.position);

        while (time < m_progressTime)
        {
            time += Time.deltaTime;

            (float from, float to) = value ? (0, 1) : (1, 0);
            float ratio = Mathf.Lerp(from, to, time / m_progressTime);

            m_gateRenderer.material.SetFloat("_DissolveAmount", ratio);

            yield return null;
        }
    }
}
