using System;
using UnityEngine;

public interface IInteractableObject
{
    Transform transform { get; }

    bool IsInteractable { get; }
    
    event EventHandler Interacted;
    
    void Interact(Actor actor, IProgress<float> onProgress, Action onComplete);
    
    void Abort();
}