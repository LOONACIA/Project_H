using System;
using UnityEngine;
using static InteractableObject;

public interface IInteractableObject
{
    Transform transform { get; }

    bool IsInteractable { get; }
    
    event EventHandler<Transform> Interacted;
    
    void Interact(Actor actor, IProgress<float> onProgress, Action onComplete);
    
    void Abort();
}