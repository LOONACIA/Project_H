using LOONACIA.Unity.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : UIPopup
{
    private enum Buttons
    {
        RestartButton,
        ExitButton
    }

    private Action m_onRestart;
    
    private Action m_onExit;

    public void SetButtonAction(Action restartAction, Action exitAction)
    {
        m_onRestart = restartAction;
        m_onExit = exitAction;
    }

    protected override void Init()
    {
        base.Init();
        
        Bind<Button, Buttons>();
        Get<Button, Buttons>(Buttons.RestartButton).onClick.AddListener(OnRestart);
        Get<Button, Buttons>(Buttons.ExitButton).onClick.AddListener(OnExit);
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
