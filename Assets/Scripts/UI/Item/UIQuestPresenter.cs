using DG.Tweening;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using Michsky.UI.Reach;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestPresenter : UIBase
{
    private readonly WaitForSeconds m_waitForSecondsCache = new(0.5f);
    
    [SerializeField]
    private AnimationCurve m_curve;

    [SerializeField]
    private QuestItem m_questItem;

    [SerializeField]
    private TextMeshProUGUI m_alarmText;
    
    [SerializeField]
    private UIManagerImage m_indicatorManager;
    
    [SerializeField]
    private UIManagerImage m_filledManager;

    [SerializeField]
    private UIManagerText m_textManager;

    [SerializeField]
    private float m_subQuestXOffset = 20;

    private CanvasScaler m_canvasScaler;

    private Quest m_quest;

    private LayoutElement m_layoutElement;

    private RectTransform m_questItemRectTransform;

    private void OnEnable()
    {
        GameManager.Localization.LanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        GameManager.Localization.LanguageChanged -= OnLanguageChanged;
    }

    public void SetQuest(Quest quest)
    {
        m_quest = quest;
        SetQuestItem();
    }

    public void Complete()
    {
        m_layoutElement.ignoreLayout = true;
        m_questItem.MinimizeQuest();
        StartCoroutine(CoWaitForDisable());
    }

    protected override void Init()
    {
        if (m_canvasScaler == null)
        {
            m_canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        m_layoutElement = GetComponent<LayoutElement>();
        m_questItem.minimizeAfter = 0;
        m_questItem.defaultState = QuestItem.DefaultState.Expanded;
        m_questItem.gameObject.SetActive(false);
        m_questItemRectTransform = m_questItem.GetComponent<RectTransform>();
        StartCoroutine(FitSize());
    }
    
    private IEnumerator FitSize()
    {
        yield return new WaitUntil(() => m_questItemRectTransform.sizeDelta.y != 0);
        Vector2 sizeDelta = m_questItemRectTransform.sizeDelta;
        m_layoutElement.preferredWidth = sizeDelta.x;
        m_layoutElement.preferredHeight = sizeDelta.y;
    }

    private void SetQuestItem()
    {
        StartCoroutine(FitSize());
        if (m_canvasScaler == null)
        {
            m_canvasScaler = GetComponentInParent<CanvasScaler>();
        }
        
        float reactiveXScale = m_canvasScaler.referenceResolution.x / Screen.width;

        float xOffset = 0;
        
        if (m_quest.IsMainQuest)
        {
            m_indicatorManager.colorType = UIManagerImage.ColorType.Accent;
            m_filledManager.colorType = UIManagerImage.ColorType.Accent;
            m_textManager.colorType = UIManagerText.ColorType.Accent;
        }
        else
        {
            m_indicatorManager.colorType = UIManagerImage.ColorType.Negative;
            m_filledManager.colorType = UIManagerImage.ColorType.Negative;
            m_textManager.colorType = UIManagerText.ColorType.Primary;
            xOffset = m_subQuestXOffset / reactiveXScale;
        }
        
        m_questItem.questText = GameManager.Localization.Get(m_quest.Content);
        m_questItem.UpdateUI();
        m_questItem.ExpandQuest();
        GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.ObjectUpdate);

        var point = Camera.main.ViewportToScreenPoint(new(0.5f, 0.4f, 0f));
        
        var myTransform = transform.GetComponent<RectTransform>();
        
        StartCoroutine(CoWait());
        return;

        IEnumerator CoWait()
        {
            yield return new WaitUntil(() => m_questItemRectTransform.sizeDelta.x != 0);
            Vector3 final = new(xOffset, 0, 0);

            var anchoredPosition = myTransform.anchoredPosition;
            Vector3 from = point - Vector3.right * (anchoredPosition.x + m_layoutElement.preferredWidth / reactiveXScale / 2f);
            from.y = -(from.y + anchoredPosition.y);
            Vector3 to = from.GetFlatVector() + final;
            
            m_questItemRectTransform.anchoredPosition = from;
            Animator animator = m_questItem.Animator;
            yield return new WaitUntil(() => !animator.enabled);
            Color color = m_alarmText.color;
            color.a = 0;
            
            DOTween.Sequence()
                .Join(m_alarmText.DOColor(color, 0.5f).SetEase(Ease.InCubic))
                .Append(m_questItemRectTransform.DOAnchorPos(to, 0.25f).SetEase(m_curve))
                .AppendInterval(0.25f)
                .Append(m_questItemRectTransform.DOAnchorPos(final, 0.25f).SetEase(m_curve));
        }
    }
    
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        m_questItem.questText = GameManager.Localization.Get(m_quest.Content);
        m_questItem.UpdateUI();
        StartCoroutine(FitSize());
    }

    private IEnumerator CoWaitForDisable()
    {
        GameObject child = m_questItem.gameObject;
        while (child.activeSelf)
        {
            yield return m_waitForSecondsCache;
        }

        m_layoutElement.ignoreLayout = false;

        ManagerRoot.Resource.Release(gameObject);
    }
}