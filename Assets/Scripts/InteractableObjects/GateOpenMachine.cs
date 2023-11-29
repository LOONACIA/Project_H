using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpenMachine : InteractableObject
{
    [SerializeField]
    private GateScript m_gateScript;

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        if (m_gateScript != null)
        {
            StartCoroutine(m_gateScript.Open());
        }
    }
}
