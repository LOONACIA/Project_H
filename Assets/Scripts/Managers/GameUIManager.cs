using LOONACIA.Unity.Managers;
using System;
using UnityEngine;

public class GameUIManager
{
    private UICrosshair m_crosshair;

    private UIShieldIndicator m_shieldIndicator;

    private UIShuriken m_shuriken;

    private UIDamageIndicator m_damageIndicator;

    public void Init()
    {
    }
    
    public void ShowGameOverUI(Action onRestart, Action onExit)
    {
        var ui = ManagerRoot.UI.ShowPopupUI<UIGameOver>();
        ui.SetButtonAction(onRestart, onExit);
    }

    public void ShowHpIndicator(PlayerController player)
    {
        var ui = ManagerRoot.UI.ShowSceneUI<UIHpIndicator>();
        ui.SetPlayer(player);
    }

    public void GenerateShieldIndicator(PlayerController player)
    {
        m_shieldIndicator = ManagerRoot.UI.ShowSceneUI<UIShieldIndicator>();
        m_shieldIndicator.SetPlayer(player);

        m_shieldIndicator.HideIndicator();
    }

    public void ShowShurikenIndicator(PossessionProcessor processor)
    {
        if (m_shuriken is not null)
        {
            // Do nothing
            return;
        }

        m_shuriken = ManagerRoot.UI.ShowSceneUI<UIShuriken>();
        m_shuriken.GetComponent<Canvas>().sortingOrder = -2;
        m_shuriken.SetPossessionProcessor(processor);
    }

    public void ShowDamageIndicator()
    {
        if (m_damageIndicator is not null)
        {
            // Do nothing
            return;
        }

        m_damageIndicator = ManagerRoot.UI.ShowSceneUI<UIDamageIndicator>();
    }

    public void ShowCrosshair()
    {
        // If crosshair is already shown
        if (m_crosshair is not null)
        {
            // Do nothing
            return;
        }

        m_crosshair = ManagerRoot.UI.ShowSceneUI<UICrosshair>();
    }

    public void HideCrosshair()
    {
        // If crosshair is not shown
        if (m_crosshair is null)
        {
            // Do nothing
            return;
        }
        
        ManagerRoot.Resource.Release(m_crosshair.gameObject);
        m_crosshair = null;
    }
}