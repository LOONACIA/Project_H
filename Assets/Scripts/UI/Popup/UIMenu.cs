using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UIPopup = LOONACIA.Unity.UI.UIPopup;

public class UIMenu : UIPopup
{
    private enum Texts
    {
        Title
    }
    
    private enum Buttons
    {
        FirstButton,
        SecondButton,
        ThirdButton
    }
    
    private WaitForSeconds m_waitForSecondsCache;

    private TextMeshProUGUI m_titleTextBox;
    
    private string m_title;
    
    private ButtonManager m_firstButton;
    
    private ButtonManager m_secondButton;
    
    private ButtonManager m_thirdButton;

    private Action m_firstButtonAction;
    
    private Action m_secondButtonAction;
    
    private Action m_thirdButtonAction;

    private CoroutineEx m_coroutine;

    private Animator m_animator;

    private void OnEnable()
    {
        m_coroutine = CoroutineEx.Create(this, CoDisableAnimator());
        GameManager.Sound.OffInGame();
    }

    private void OnDisable()
    {
        m_coroutine?.Abort();
        m_firstButton.transform.localScale = m_secondButton.transform.localScale = m_thirdButton.transform.localScale = Vector3.one;
        GameManager.Sound.OnInGame();
    }

    public void SetTitle(string text)
    {
        m_title = text;
        if (m_titleTextBox != null)
        {
            m_titleTextBox.text = text;
        }
    }
    
    public void SetButtonContent(MenuInfo firstButton, MenuInfo secondButton, MenuInfo thirdButton)
    {
        m_firstButton.buttonText = firstButton.Text;
        m_secondButton.buttonText = secondButton.Text;
        m_thirdButton.buttonText = thirdButton.Text;
        
        m_firstButtonAction = firstButton.OnClick;
        m_secondButtonAction = secondButton.OnClick;
        m_thirdButtonAction = thirdButton.OnClick;
        
        m_firstButton.UpdateUI();
        m_secondButton.UpdateUI();
        m_thirdButton.UpdateUI();
    }
    
    public override void Close()
    {
        m_firstButtonAction?.Invoke();
        base.Close();
    }

    protected override void Init()
    {
        // Menu should be top of the UI stack. Don't call base.Init().
        //base.Init();

        m_animator = GetComponent<Animator>();
        
        Bind<TextMeshProUGUI, Texts>();
        Bind<ButtonManager, Buttons>();

        m_firstButton = Get<ButtonManager, Buttons>(Buttons.FirstButton);
        m_secondButton = Get<ButtonManager, Buttons>(Buttons.SecondButton);
        m_thirdButton = Get<ButtonManager, Buttons>(Buttons.ThirdButton);
        
        m_firstButton.onClick.AddListener(OnFirstButtonClick);
        m_secondButton.onClick.AddListener(OnSecondButtonClick);
        m_thirdButton.onClick.AddListener(OnThirdButtonClick);
        
        m_titleTextBox = Get<TextMeshProUGUI, Texts>(Texts.Title);
        if (!string.IsNullOrEmpty(m_title))
        {
            m_titleTextBox.text = m_title;
        }
    }
    
    private void OnFirstButtonClick()
    {
        m_firstButtonAction?.Invoke();
    }
    
    private void OnSecondButtonClick()
    {
        m_secondButtonAction?.Invoke();
    }
    
    private void OnThirdButtonClick()
    {
        m_thirdButtonAction?.Invoke();
    }
    
    private IEnumerator CoDisableAnimator()
    {
        yield return m_waitForSecondsCache ??= new(ReachUIInternalTools.GetAnimatorClipLength(m_animator, "Menu_In"));
        m_animator.enabled = false;
    }
}
