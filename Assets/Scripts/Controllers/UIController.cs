using DG.Tweening;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using UnityEngine;

public partial class UIController : MonoBehaviour
{
    private bool m_isDialogShown;
    
    private UIModalDialogPresenter m_dialogPresenter;
    
    private float m_timeScale;
    
    private CoroutineEx m_dialogCloseCoroutine;

    private void Start()
    {
        InitInput();
    }

    private void OnEnable()
    {
        GameManager.Notification.Activated += OnNotificationActivated;
    }

    private void OnDisable()
    {
        GameManager.Notification.Activated -= OnNotificationActivated;
    }

    private void Pause()
    {
        if (m_isDialogShown && m_dialogCloseCoroutine?.IsRunning is not true)
        {
            m_dialogPresenter.Confirm();
            return;
        }

        m_inputActions.Character.Disable();
        GameManager.Instance.Pause(m_inputActions.Character.Enable);
    }

    private void OnNotificationActivated(object sender, Notification e)
    {
        if (e is ModalDialog dialog)
        {
            ShowModalDialog(dialog);
        }
    }

    private void ShowModalDialog(ModalDialog dialog, bool isInitial = true)
    {
        if (isInitial)
        {
            m_timeScale = Time.timeScale;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_inputActions.Character.Disable();
        }

        m_dialogPresenter = ManagerRoot.UI.ShowPopupUI<UIModalDialogPresenter>();
        m_dialogPresenter.SetDialog(dialog, OnConfirm);
        m_isDialogShown = true;

        void OnConfirm()
        {
            m_dialogCloseCoroutine = CoroutineEx.Create(this, CoWaitForAnimation());
        }
        
        IEnumerator CoWaitForAnimation()
        {
            yield return new WaitUntil(() => !m_dialogPresenter.IsOpen);
            ManagerRoot.UI.ClosePopupUI(m_dialogPresenter);

            if (dialog.RelatedDialog == null)
            {
                m_inputActions.Character.Enable();
                Time.timeScale = m_timeScale;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_isDialogShown = false;
            }
            else
            {
                ShowModalDialog(dialog.RelatedDialog, false);
            }
        }
    }
}