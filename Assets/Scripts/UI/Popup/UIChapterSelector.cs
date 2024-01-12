using LOONACIA.Unity.Managers;
using System;
using UnityEngine;
using UIPopup = LOONACIA.Unity.UI.UIPopup;

public class UIChapterSelector : UIPopup
{
    [SerializeField]
    private Transform m_chapterParent;

    [SerializeField]
    private GameObject m_chapterPrefab;

    public void Clear()
    {
        for (var index = m_chapterParent.childCount - 1; index >= 0; index--)
        {
            ManagerRoot.Resource.Release(m_chapterParent.GetChild(index).gameObject);
        }
    }

    public void AddChapter(ChapterInfo chapterInfo, Action onClick)
    {
        var chapter = ManagerRoot.Resource.Instantiate(m_chapterPrefab);
        chapter.transform.SetParent(m_chapterParent, false);
        chapter.GetComponent<UIChapterButton>().SetItem(chapterInfo, onClick);
    }
}