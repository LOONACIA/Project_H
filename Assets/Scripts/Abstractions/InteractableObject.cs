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

    /// <summary>
    /// 상호작용을 중단합니다.
    /// </summary>
    public void Abort()
    {
        if (m_interactCoroutine?.IsRunning is true)
        {
            m_interactCoroutine.Abort();
        }
    }
    
    /// <summary>
    /// 상호작용 시작 시의 동작을 정의합니다.
    /// </summary>
    /// <param name="actor"></param>
    protected virtual void OnInteractStart(Actor actor)
    {
        // 상호작용 시작 시 처리할 내용이 있을 경우, 이 메서드를 오버라이드하여 구현
    }
    
    /// <summary>
    /// 상호작용 완료 시의 동작을 정의합니다.
    /// </summary>
    /// <param name="actor">상호작용을 시도한 Actor</param>
    protected abstract void OnInteract(Actor actor);

    private IEnumerator CoWaitTime(Actor actor, IProgress<float> progress, Action onComplete)
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