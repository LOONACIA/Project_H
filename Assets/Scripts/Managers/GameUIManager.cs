using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager
{
    private List<UIScene> m_sceneUIs = new();
    
    private UICrosshair m_crosshair;

    private UIDamageIndicator m_damageIndicator;
    
    private UIProgressRing m_progressRing;
    
    private UIMessageDialog m_dialog;

    private UIObjects m_objects;

    private int m_dialogVersion;
    
    private CoroutineEx m_showDialogCoroutine;

    private UISensedWarning m_warningUI;
    
    private UIMenu m_menuUI;

    public void Init()
    {
        m_dialogVersion = 0;
    }

    public void CloseSceneUI()
    {
        ManagerRoot.UI.ClearAllPopup();
        
        for (int index = m_sceneUIs.Count - 1; index >= 0; --index)
        {
            var ui = m_sceneUIs[index];
            m_sceneUIs.RemoveAt(index);
            ManagerRoot.Resource.Release(ui.gameObject);
        }
        
        if (m_crosshair is not null)
        {
            ManagerRoot.Resource.Release(m_crosshair.gameObject);
            m_crosshair = null;
        }
        
        if (m_damageIndicator is not null)
        {
            ManagerRoot.Resource.Release(m_damageIndicator.gameObject);
            m_damageIndicator = null;
        }

        if (m_objects != null)
        {
            ManagerRoot.Resource.Release(m_objects.gameObject);
            m_objects = null;
        }
    }

    public void Clear()
    {
        m_crosshair = null;
        m_damageIndicator = null;
    }
    
    public void ShowMenuUI(Action onResume, Action onMenu, Action onExit, string text = "Game Over")
    {
        if (m_menuUI != null && m_menuUI.gameObject.activeSelf)
        {
            m_menuUI.Close();
            return;
        }
        
        List<Transform> children = new();
        m_menuUI = ManagerRoot.UI.ShowPopupUI<UIMenu>();
        m_menuUI.SetButtonAction(Resume, onMenu, onExit);
        m_menuUI.SetText(text);
        
        foreach (Transform child in ManagerRoot.UI.Root.transform)
        {
            if (child != m_menuUI.transform && child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                children.Add(child);
            }
        }

        return;

        void Resume()
        {
            foreach (Transform child in children)
            {
                child.gameObject.SetActive(true);
            }
            ManagerRoot.UI.ClosePopupUI(m_menuUI);
            onResume?.Invoke();
        }
    }
    
    public void ShowGameOverUI(Action onRestart, Action onMenu, Action onExit, string text = "Game Over")
    {
        CloseSceneUI();
        
        ShowMenuUI(onRestart, onMenu, onExit, text);
    }

    public void ShowHpIndicator(PlayerController player)
    {
        var ui = ManagerRoot.UI.ShowSceneUI<UIHpIndicator>();
        ui.SetPlayer(player);
        m_sceneUIs.Add(ui);
    }

    public void ShowObjects()
    {
        m_objects = ManagerRoot.UI.ShowSceneUI<UIObjects>();
    }
    
    public void UpdateObject(string _text)
    {
        if (m_objects == null)
        {
            return;
        }
        
        m_objects.UpdateObjectText(_text);
    }

    public void ShowShurikenIndicator(PossessionProcessor processor)
    {
        var ui = ManagerRoot.UI.ShowSceneUI<UIShuriken>();
        //ui.GetComponent<Canvas>().sortingOrder = -2;
        ui.SetPossessionProcessor(processor);
        m_sceneUIs.Add(ui);
    }

    public void ShowSkillIndicator(PlayerController player)
    { 
        var ui = ManagerRoot.UI.ShowSceneUI<UISkill>();
        //ui.GetComponent<Canvas>().sortingOrder = -2;
        ui.SetActorStatus(player);
        m_sceneUIs.Add(ui);
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
        return -1;

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
        return -1;

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