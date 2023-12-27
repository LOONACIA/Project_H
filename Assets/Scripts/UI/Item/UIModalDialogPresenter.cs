using LOONACIA.Unity.UI;
using Michsky.UI.Reach;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UIPopup = LOONACIA.Unity.UI.UIPopup;

public class UIModalDialogPresenter : UIPopup
{
    [SerializeField]
    private ModalWindowManager m_modalWindow;
    
    [SerializeField]
    private VideoPlayer m_videoPlayer;
    
    [SerializeField]
    private TextMeshProUGUI m_text;
    
    [SerializeField]
    private ButtonManager m_confirmButton;
    
    private Action m_onConfirm;

    public bool IsOpen => m_modalWindow.isActiveAndEnabled;
    
    public void SetDialog(ModalDialog dialog, Action onConfirm = null)
    {
        m_text.text = dialog.Content;
        if (dialog.Video != null)
        {
            m_videoPlayer.clip = dialog.Video;
        }

        m_onConfirm = onConfirm;
        m_modalWindow.OpenWindow();
        GameManager.Sound.OffInGame();
        m_videoPlayer.Play();
    }

    public void Confirm()
    {
        m_modalWindow.CloseWindow();
        m_onConfirm?.Invoke();
    }
    
    protected override void Init()
    {
        base.Init();
        m_confirmButton.onClick.AddListener(OnConfirm);
    }

    private void OnConfirm()
    {
        m_onConfirm?.Invoke();
        GameManager.Sound.OnInGame();
    }
}