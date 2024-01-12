using DG.Tweening;
using LOONACIA.Unity.UI;
using UnityEngine;

public class UIDim : UIScene
{
    [SerializeField]
    private CanvasGroup m_canvasGroup;

    private Tween m_fadeTween;

    private void OnDisable()
    {
        m_fadeTween?.Kill();
    }
    
    /// <summary>
    /// Fade in the dim.
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration)
    {
        m_fadeTween?.Kill();
        m_fadeTween = m_canvasGroup.DOFade(1f, 0.5f);
    }
    
    /// <summary>
    /// Fade out the dim.
    /// </summary>
    /// <param name="duration"></param>
    public void FadeOut(float duration)
    {
        m_fadeTween?.Kill();
        m_fadeTween = m_canvasGroup.DOFade(0f, 0.5f);
    }
}
