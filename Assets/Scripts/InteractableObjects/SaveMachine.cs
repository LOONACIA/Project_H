using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMachine : InteractableObject
{
    [SerializeField]
    private Transform m_respawnPosition;

    private void OnDrawGizmosSelected()
    {
        if (m_respawnPosition == null)
        {
            return;
        }
        
        Gizmos.color = Color.green;
        var currentTransform = transform;
        Gizmos.matrix =
            Matrix4x4.TRS(currentTransform.position, currentTransform.rotation, currentTransform.localScale) *
            (Matrix4x4.Translate(m_respawnPosition.localPosition) *
             Matrix4x4.Rotate(Quaternion.Euler(m_respawnPosition.rotation.eulerAngles)));
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }

    protected override void OnInteract(Actor actor)
    {
        GameManager.Character.SaveInformation(actor, m_respawnPosition);
        IsInteractable = false;
    }
}