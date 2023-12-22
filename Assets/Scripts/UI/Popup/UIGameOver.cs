using LOONACIA.Unity.UI;
using System;
using UnityEngine.UI;

public class UIGameOver : UIPopup
{
    private enum Texts
    {
        TitleLabel
    }
    
    private enum Buttons
    {
        ContinueButton,
        MenuButton,
        ExitButton
    }

    private Text m_titleTextBox;
    
    private string m_text;

    private Action m_onContinue;
    
    private Action m_onMenu;
    
    private Action m_onExit;

    public void SetText(string text)
    {
        m_text = text;
        if (m_titleTextBox != null)
        {
            m_titleTextBox.text = text;
        }
    }

    public void SetButtonAction(Action restartAction, Action onMenuAction, Action exitAction)
    {
        m_onContinue = restartAction;
        m_onMenu = onMenuAction;
        m_onExit = exitAction;
    }

    protected override void Init()
    {
        base.Init();
        
        Bind<Text, Texts>();
        Bind<Button, Buttons>();
        
        Get<Button, Buttons>(Buttons.ContinueButton).onClick.AddListener(OnRestart);
        Get<Button, Buttons>(Buttons.MenuButton).onClick.AddListener(OnMenu);
        Get<Button, Buttons>(Buttons.ExitButton).onClick.AddListener(OnExit);
        
        m_titleTextBox = Get<Text, Texts>(Texts.TitleLabel);
        if (!string.IsNullOrEmpty(m_text))
        {
            m_titleTextBox.text = m_text;
        }
    }
    
    private void OnRestart()
    {
        m_onContinue?.Invoke();
    }
    
    private void OnMenu()
    {
        m_onMenu?.Invoke();
    }
    
    private void OnExit()
    {
        m_onExit?.Invoke();
    }
}
