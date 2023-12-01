using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMachine : InteractableObject
{
    [SerializeField]
    private GameObject m_gate;
    
    private IGate m_gateScript;

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        if (m_gate != null && m_gateScript != null)
        {
            StartCoroutine(m_gateScript.Open());
        }
    }

    private void Start()
    {
        m_gateScript = m_gate.GetComponent<IGate>();
    }
}
