using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RenownedGames.ApexEditor.SerializedMember;

public class EnterZone : MonoBehaviour
{
    public event EventHandler OnEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster")
            && other.gameObject.TryGetComponent<Actor>(out var actor)
            && actor.IsPossessed)
        {
            OnEnter?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
