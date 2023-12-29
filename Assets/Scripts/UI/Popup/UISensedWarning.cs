using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.UI;
using System.Collections;
using UnityEngine;

public class UISensedWarning : UIBase
{
    private Canvas m_canvas;
    
    private CoroutineEx m_showCoroutine;
    
    protected override void Init()
    {
        m_canvas = GetComponentInChildren<Canvas>();
    }
    
    public void Show(float duration, float interval)
    {
        if (m_showCoroutine?.IsRunning is true)
        {
            return;
        }
        
        if (!isActiveAndEnabled)
        {
            return;
        }

        m_showCoroutine = CoroutineEx.Create(this, CoShowUI(duration, interval));
    }

    private IEnumerator CoShowUI(float duration, float interval)
    {
        bool enabled = false;
        float timer = 0;
        while (timer < duration)
        {
            m_canvas.enabled = enabled = !enabled;
            timer += interval;
            yield return new WaitForSeconds(interval);
        }

        m_canvas.enabled = false;
    }
}
