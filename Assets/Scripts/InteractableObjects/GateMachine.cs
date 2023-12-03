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

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        if (m_gate != null && m_gateScript != null)
        {
            StartCoroutine(m_gateScript.Open());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_alarmRadius);
    }
}
