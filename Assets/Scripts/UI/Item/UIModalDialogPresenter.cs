using Michsky.UI.Reach;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    protected override void OnEnable()
    {
        base.OnEnable();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(m_confirmButton.gameObject);
    }

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
        Close();
    }

    public override void Close()
    {
        m_modalWindow.CloseWindow();
        OnConfirm();
        base.Close();
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