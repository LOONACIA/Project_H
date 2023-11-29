using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    protected float m_requiredTime;
    
    private CoroutineEx m_interactCoroutine;
    
    public bool IsInteractable { get; protected set; } = true;
    
    public void Interact(Actor actor, IProgress<float> progress, Action onComplete)
    {
        m_interactCoroutine = CoroutineEx.Create(this, CoWaitTime(actor, progress, onComplete));
    }

    public void Abort()
    {
        if (m_interactCoroutine?.IsRunning is true)
        {
            m_interactCoroutine.Abort();
        }
    }
    
    protected abstract void OnInteract(Actor actor);

    protected virtual IEnumerator CoWaitTime(Actor actor, IProgress<float> progress, Action onComplete)
    {
        float time = 0f;
        float seconds = m_requiredTime;
        
        while (time < seconds)
        {
            time += Time.deltaTime;
            progress?.Report(time / seconds);
            yield return null;
        }
        progress?.Report(seconds);
        
        OnInteract(actor);
        onComplete?.Invoke();
    }
}