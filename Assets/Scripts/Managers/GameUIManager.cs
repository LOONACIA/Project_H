using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager
{
    private UICrosshair m_crosshair;

    private UIShieldIndicator m_shieldIndicator;

    private UIShuriken m_shuriken;

    private UISkill m_skill;

    private UIDamageIndicator m_damageIndicator;
    
    private UIProgressRing m_progressRing;
    
    private UIMessageDialog m_dialog;

    private UIObjects m_objects;

    private int m_dialogVersion;
    
    private CoroutineEx m_showDialogCoroutine;

    private UISensedWarning m_warningUI;

    public void Init()
    {
        m_dialogVersion = 0;
    }

    public void CloseAll()
    {
        ManagerRoot.UI.ClearAllPopup();
        if (m_crosshair is not null)
        {
            ManagerRoot.Resource.Release(m_crosshair.gameObject);
            m_crosshair = null;
        }
        
        if (m_shuriken is not null)
        {
            ManagerRoot.Resource.Release(m_shuriken.gameObject);
            m_shuriken = null;
        }
        
        if (m_skill is not null)
        {
            ManagerRoot.Resource.Release(m_skill.gameObject);
            m_skill = null;
        }
        
        if (m_damageIndicator is not null)
        {
            ManagerRoot.Resource.Release(m_damageIndicator.gameObject);
            m_damageIndicator = null;
        }
    }

    public void Clear()
    {
        m_crosshair = null;
        m_shieldIndicator = null;
        m_shuriken = null;
        m_skill = null;
        m_damageIndicator = null;
    }
    
    public void ShowGameOverUI(Action onRestart, Action onExit, string text = "Game Over")
    {
        CloseAll();
        
        var ui = ManagerRoot.UI.ShowPopupUI<UIGameOver>();
        ui.SetButtonAction(onRestart, onExit);
        ui.SetText(text);
    }

    public void ShowHpIndicator(PlayerController player)
    {
        var ui = ManagerRoot.UI.ShowSceneUI<UIHpIndicator>();
        ui.SetPlayer(player);
    }

    public void ShowObjects()
    {
        m_objects = ManagerRoot.UI.ShowSceneUI<UIObjects>();
    }
    
    public void UpdateObject(string _text)
    {
        m_objects.UpdateObjectText(_text);
    }

    public void ShowShieldIndicator(PlayerController player)
    {
        m_shieldIndicator = ManagerRoot.UI.ShowSceneUI<UIShieldIndicator>();
        m_shieldIndicator.SetPlayer(player);

        m_shieldIndicator.HideIndicator();
    }

    public void ShowShurikenIndicator(PossessionProcessor processor)
    {
        m_shuriken = ManagerRoot.UI.ShowSceneUI<UIShuriken>();
        m_shuriken.GetComponent<Canvas>().sortingOrder = -2;
        m_shuriken.SetPossessionProcessor(processor);
    }

    public void ShowSkillIndicator(PlayerController player)
    { 
        m_skill = ManagerRoot.UI.ShowSceneUI<UISkill>();
        m_skill.GetComponent<Canvas>().sortingOrder = -2;
        m_skill.SetActorStatus(player);
    }

    public void ShowDamageIndicator()
    {
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
    
    public IProgress<float> ShowProgressRing(UIProgressRing.TextDisplayMode mode = UIProgressRing.TextDisplayMode.None, string text = null)
    {
        // If progress ring is already shown, hide it
        HideProgressRing();
        
        m_progressRing = ManagerRoot.UI.ShowPopupUI<UIProgressRing>();
        m_progressRing.DisplayMode = mode;
        m_progressRing.SetText(text);
        Progress<float> progress = new(value =>
        {
            if (m_progressRing is not null)
                m_progressRing.UpdateProgress(value);
        });

        return progress;
    }

    public void HideProgressRing()
    {
        if (m_progressRing is not null)
        {
            ManagerRoot.UI.ClosePopupUI(m_progressRing);
            m_progressRing = null;
        }
    }
    
    public int ShowDialog(string text)
    {
        if (m_dialog == null)
        {
            m_dialog = ManagerRoot.UI.ShowPopupUI<UIMessageDialog>();            
        }
        
        m_dialog.SetText(text);
        m_dialog.gameObject.SetActive(true);
        return ++m_dialogVersion;
    }

    public int ShowDialog(MessageDialogInfo[] texts, float interval = 1f)
    {
        if (m_dialog == null)
        {
            m_dialog = ManagerRoot.UI.ShowPopupUI<UIMessageDialog>();
        }
        m_showDialogCoroutine?.Abort();
        m_dialog.Abort();

        int index = 0;
        MessageDialogInfo dialogInfo = texts[index++];
        m_dialog.SetDialogInfo(dialogInfo, () => dialogInfo.Callback?.Invoke());
        //m_dialog.SetText(dialogInfo.Message, () => dialogInfo.Callback?.Invoke());
        m_dialog.gameObject.SetActive(true);
        m_showDialogCoroutine = CoroutineEx.Create(m_dialog, CoShowDialog(texts, interval, index));
        return ++m_dialogVersion;

        IEnumerator CoShowDialog(IReadOnlyList<MessageDialogInfo> infoList, float innerInterval, int innerIndex)
        {
            while (innerIndex < infoList.Count)
            {
                while (m_dialog.IsTyping)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(innerInterval);

                var info = infoList[innerIndex++];
                m_dialog.SetDialogInfo(info, () => info.Callback?.Invoke());
                //m_dialog.SetText(info.Message, () => info.Callback?.Invoke());
            }
        }
    }

    public void HideDialog(int version)
    {
        if (m_dialog == null)
        {
            return;
        }

        if (version != m_dialogVersion)
        {
            return;
        }
        
        m_dialog.gameObject.SetActive(false);
    }

    public void ShowWarning(float duration, float interval)
    {
        if (m_warningUI == null)
        {
            m_warningUI = ManagerRoot.UI.ShowPopupUI<UISensedWarning>();
        }
        
        m_warningUI.Show(duration, interval);
    }
}