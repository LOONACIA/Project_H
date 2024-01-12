using LOONACIA.Unity.UI;
using Michsky.UI.Reach;
using UnityEngine;
using UnityEngine.UI;

public class UIChapterButton : UIBase
{
    [SerializeField]
    private Image m_thumbnail;
    
    private ButtonManager m_buttonManager;
    
    public void SetItem(ChapterInfo chapterInfo, System.Action onClick)
    {
        m_buttonManager.onClick.RemoveAllListeners();
        m_buttonManager.onClick.AddListener(() => onClick?.Invoke());
        m_buttonManager.buttonText = chapterInfo.DisplayName;
        m_buttonManager.isInteractable = chapterInfo.IsUnlocked;
        m_thumbnail.sprite = Resources.Load<Sprite>(chapterInfo.Thumbnail);
        m_buttonManager.UpdateUI();
    }
    
    protected override void Init()
    {
        m_buttonManager = GetComponent<ButtonManager>();
    }
}
