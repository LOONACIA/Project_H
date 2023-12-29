using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UIPopup = LOONACIA.Unity.UI.UIPopup;

public class UIMenu : UIPopup, IEventSystemHandler
{
    private enum Texts
    {
        Title
    }
    
    private readonly List<ButtonManager> m_buttons = new();
    
    [SerializeField]
    private GameObject m_buttonPrefab;
    
    [SerializeField]
    private Transform m_buttonParent;
    
    [SerializeField]
    private CanvasGroup m_buttonsCanvasGroup;
    
    private WaitForSeconds m_waitForSecondsCache;

    private TextMeshProUGUI m_titleTextBox;
    
    private string m_title;

    private CoroutineEx m_coroutine;

    private Animator m_animator;

    private void OnEnable()
    {
        m_coroutine = CoroutineEx.Create(this, CoDisableAnimator());
        GameManager.Sound.OffInGame();
        m_buttonsCanvasGroup.interactable = true;
    }

    private void OnDisable()
    {
        m_coroutine?.Abort();
    }

    private void Update()
    {
        bool isTop = ManagerRoot.UI.GetTopPopupUI() == this;
        if (m_buttonsCanvasGroup.interactable != isTop)
        {
            m_buttonsCanvasGroup.interactable = isTop;
            if (isTop)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(m_buttons[0].gameObject);
            }
        }
    }

    public void SetTitle(string text)
    {
        m_title = text;
        if (m_titleTextBox != null)
        {
            m_titleTextBox.text = text;
        }
    }
    
    public void SetButtonContent(params MenuInfo[] menuInfos)
    {
        Clear();
        for (int index = 0; index < menuInfos.Length; index++)
        {
            int cursor = index;
            var button = ManagerRoot.Resource.Instantiate(m_buttonPrefab, m_buttonParent).GetComponent<ButtonManager>();
            button.buttonText = menuInfos[index].Text;
            button.onClick.AddListener(() => OnButtonClick(menuInfos[cursor].OnClick));
            button.UpdateUI();
            m_buttons.Add(button);
        }
        
        for (int index = 0; index < menuInfos.Length; index++)
        {
            var button = m_buttons[index];
            button.GetComponent<Button>().navigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = m_buttons[Mathf.Max(0, index - 1)].GetComponent<Button>(),
                selectOnDown = m_buttons[Mathf.Min(m_buttons.Count - 1, index + 1)].GetComponent<Button>()
            };
        }
        
        EventSystem.current.SetSelectedGameObject(null);
        if (m_buttons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(m_buttons[0].gameObject);
        }
    }

    private void OnButtonClick(Action action)
    {
        GameManager.Sound.OnInGame();
        action?.Invoke();
    }
    
    public override void Close()
    {
        // 종료 시 첫 번째 버튼의 동작을 실행
        if (m_buttons.Count > 0)
        {
            m_buttons[0].onClick.Invoke();
        }
        base.Close();
    }

    protected override void Init()
    {
        base.Init();

        m_animator = GetComponent<Animator>();
        
        Bind<TextMeshProUGUI, Texts>();
        
        m_titleTextBox = Get<TextMeshProUGUI, Texts>(Texts.Title);
        if (!string.IsNullOrEmpty(m_title))
        {
            m_titleTextBox.text = m_title;
        }
    }

    private void Clear()
    {
        foreach (var button in m_buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        
        m_buttons.Clear();
        int childCount = m_buttonParent.childCount;
        while (childCount-- > 0)
        {
            ManagerRoot.Resource.Release(m_buttonParent.GetChild(0).gameObject);
        }
    }
    
    private IEnumerator CoDisableAnimator()
    {
        yield return m_waitForSecondsCache ??= new(ReachUIInternalTools.GetAnimatorClipLength(m_animator, "Menu_In"));
        m_animator.enabled = false;
    }
}
