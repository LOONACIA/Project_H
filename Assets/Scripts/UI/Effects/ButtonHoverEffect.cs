using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private AudioSource m_audioSource;
    
    private void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
        }
        
        m_audioSource = GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.ObjectUpdate);
        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
        }
        
        m_audioSource = GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.ObjectUpdate);
        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}
