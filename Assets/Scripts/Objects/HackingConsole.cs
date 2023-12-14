using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingConsole : HackingObject
{
    public EventHandler Interacted;

    public override void Interact()
    {
        Interacted.Invoke(this, EventArgs.Empty);
    }
}
