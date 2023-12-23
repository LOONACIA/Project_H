using DG.Tweening;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using Michsky.UI.Reach;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestPresenter : UIBase
{
    private readonly WaitForSeconds m_waitForSecondsCache = new(0.5f);
    
    [SerializeField]
    private AnimationCurve m_curve;

    [SerializeField]
    private QuestItem m_questItem;

    private Quest m_quest;

    private LayoutElement m_layoutElement;

    public void SetQuest(Quest quest)
    {
        m_quest = quest;
        SetQuestItem();
    }

    public void Complete()
    {
        m_questItem.MinimizeQuest();
        StartCoroutine(CoWaitForDisable());
    }

    protected override void Init()
    {
        m_layoutElement = GetComponent<LayoutElement>();
        m_questItem.minimizeAfter = 0;
        m_questItem.defaultState = QuestItem.DefaultState.Expanded;
        m_questItem.gameObject.SetActive(false);
        var rectTransform = m_questItem.GetComponent<RectTransform>();
        StartCoroutine(FitSize());
        return;

        IEnumerator FitSize()
        {
            yield return new WaitUntil(() => rectTransform.sizeDelta.y != 0);
            Vector2 sizeDelta = rectTransform.sizeDelta;
            m_layoutElement.preferredWidth = sizeDelta.x;
            m_layoutElement.preferredHeight = sizeDelta.y;
        }
    }

    private void SetQuestItem()
    {
        m_questItem.questText = m_quest.Content;
        m_questItem.ExpandQuest();

        var point = Camera.main.ViewportToScreenPoint(new(0.5f, 0.4f, 0f));
        var rectTransform = m_questItem.GetComponent<RectTransform>();
        var myTransform = transform as RectTransform;
        
        StartCoroutine(CoWait());
        return;

        IEnumerator CoWait()
        {
            yield return new WaitUntil(() => rectTransform.sizeDelta.x != 0);
            
            var from = point - Vector3.right * (m_layoutElement.preferredWidth / 2f);
            from.y = -(from.y + myTransform.anchoredPosition.y);
            var to = new Vector3(from.x, 0, from.z);
            
            rectTransform.anchoredPosition = from;
            var animator = m_questItem.GetComponent<Animator>();
            yield return new WaitUntil(() => !animator.enabled);
            yield return new WaitForSeconds(0.25f);
            rectTransform.DOAnchorPos(to, 0.25f).SetEase(m_curve);
            yield return m_waitForSecondsCache;
            rectTransform.DOAnchorPos(Vector3.zero, 0.25f).SetEase(m_curve);
        }
    }

    private IEnumerator CoWaitForDisable()
    {
        while (gameObject.activeSelf)
        {
            yield return m_waitForSecondsCache;
        }

        ManagerRoot.Resource.Release(gameObject);
    }
}