using DG.Tweening;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class UIController : MonoBehaviour
{
    private static readonly WaitForSecondsRealtime m_gameOverYieldInstructionCache = new(2f);
    
    private bool m_isDialogShown;
    
    private UIModalDialogPresenter m_dialogPresenter;
    
    private float m_timeScale;
    
    private CoroutineEx m_dialogCloseCoroutine;
    
    private bool m_isMenuShown;

    private void Start()
    {
        InitInput();
    }

    private void OnEnable()
    {
        GameManager.Instance.GameOver += OnGameOver;
        GameManager.Instance.Pause += OnPause;
        GameManager.Instance.Resume += OnResume;
        GameManager.Notification.Activated += OnNotificationActivated;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameOver -= OnGameOver;
        GameManager.Instance.Pause -= OnPause;
        GameManager.Instance.Resume -= OnResume;
        GameManager.Notification.Activated -= OnNotificationActivated;
    }

    private void Pause()
    {
        if (m_isDialogShown && m_dialogCloseCoroutine?.IsRunning is not true)
        {
            m_dialogPresenter.Confirm();
            return;
        }

        if (GameManager.Instance.IsGameOver)
        {
            return;
        }

        m_inputActions.Character.Disable();
        GameManager.Instance.SetPause();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnGameOver(object sender, EventArgs e)
    {
        StartCoroutine(ShowUI());
        
        IEnumerator ShowUI()
        {
            yield return m_gameOverYieldInstructionCache;
            GameManager.UI.ShowMenuUI(MenuInfoBag.Restart, MenuInfoBag.Menu, MenuInfoBag.Exit, MenuInfoBag.GameOverText);
        }
    }

    private void OnPause(object sender, EventArgs e)
    {
        GameManager.UI.ShowMenuUI(MenuInfoBag.Continue, MenuInfoBag.Menu, MenuInfoBag.Exit, MenuInfoBag.PausedText);
        m_isMenuShown = true;
    }
    
    private void OnResume(object sender, EventArgs e)
    {
        m_inputActions.Character.Enable();
        m_isMenuShown = false;
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
            Cursor.lockState = CursorLockMode.Confined;
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

            yield return new WaitUntil(() => !m_isMenuShown);

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

    private static class MenuInfoBag
    {
        public static readonly MenuInfo Restart = new("Restart from checkpoint", () => SceneManagerEx.LoadScene(SceneManager.GetActiveScene().name));
    
        public static readonly MenuInfo Continue = new("Resume", () => GameManager.Instance.SetResume());
    
        public static readonly MenuInfo Menu = new("Exit to main menu", () => SceneManagerEx.LoadScene("TitleScene"));
    
        public static readonly MenuInfo Exit =
#if UNITY_EDITOR
            new("Exit the game", () => UnityEditor.EditorApplication.isPlaying = false);
#else
        new("Exit the game", Application.Quit);
#endif
        public static readonly string GameOverText = "Game Over";
        
        public static readonly string PausedText = "Paused";
    }
}