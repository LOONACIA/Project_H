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
        RestartButton,
        ExitButton
    }

    private Text m_titleTextBox;
    
    private string m_text;

    private Action m_onRestart;
    
    private Action m_onExit;

    public void SetText(string text)
    {
        m_text = text;
        if (m_titleTextBox != null)
        {
            m_titleTextBox.text = text;
        }
    }

    public void SetButtonAction(Action restartAction, Action exitAction)
    {
        m_onRestart = restartAction;
        m_onExit = exitAction;
    }

    protected override void Init()
    {
        base.Init();
        
        Bind<Text, Texts>();
        Bind<Button, Buttons>();
        
        Get<Button, Buttons>(Buttons.RestartButton).onClick.AddListener(OnRestart);
        Get<Button, Buttons>(Buttons.ExitButton).onClick.AddListener(OnExit);
        
        m_titleTextBox = Get<Text, Texts>(Texts.TitleLabel);
        if (!string.IsNullOrEmpty(m_text))
        {
            m_titleTextBox.text = m_text;
        }
    }
    
    private void OnRestart()
    {
        m_onRestart?.Invoke();
    }
    
    private void OnExit()
    {
        m_onExit?.Invoke();
    }
}
