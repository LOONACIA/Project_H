using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearMachine : InteractableObject
{
    protected override void OnInteract(Actor actor)
    {
        if (actor.IsPossessed)
        {
            GameManager.Instance.SetGameClear();
        }
    }
}
