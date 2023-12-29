using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class UIController : MonoBehaviour
{
    private static readonly WaitForSecondsRealtime m_gameOverYieldInstructionCache = new(2f);
    
    private float m_timeScale;
    
    private UIModalDialogPresenter m_dialogPresenter;
    
    private bool m_isMenuShown;
    
    private bool m_isMenuRequested;
    
    private bool m_isModal;
    
    private CoroutineEx m_dialogCloseCoroutine;

    private void Start()
    {
        InitInput();
    }

    private void OnEnable()
    {
        GameManager.Instance.GameClear += OnGameClear;
        GameManager.Instance.GameOver += OnGameOver;
        GameManager.Instance.Pause += OnPause;
        GameManager.Instance.Resume += OnResume;
        GameManager.Notification.Activated += OnNotificationActivated;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameClear -= OnGameClear;
        GameManager.Instance.GameOver -= OnGameOver;
        GameManager.Instance.Pause -= OnPause;
        GameManager.Instance.Resume -= OnResume;
        GameManager.Notification.Activated -= OnNotificationActivated;
    }

    private void Pause()
    {
        // Modal 시 Game State가 Pause로 변경되므로, Modal 관련 로직이 우선되어야 함
        // if (m_isModal && m_dialogCloseCoroutine?.IsRunning is not true)
        // {
        //     m_dialogPresenter.Confirm();
        //     return;
        // }

        if (ManagerRoot.UI.ClosePopupUI())
        {
            return;
        }
        
        // if (GameManager.Instance.IsPaused)
        // {
        //     GameManager.Instance.SetResume();
        //     return;
        // }

        if (GameManager.Instance.IsGameOver)
        {
            return;
        }
        
        m_isMenuRequested = true;
        GameManager.Instance.SetPause();
    }
    
    private void OnGameClear(object sender, EventArgs e)
    {
        GameManager.UI.ShowMenuUI(MenuInfoBag.GameClearText, MenuInfoBag.Menu, MenuInfoBag.Credits, MenuInfoBag.Exit);
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        StartCoroutine(ShowUI());
        
        IEnumerator ShowUI()
        {
            yield return m_gameOverYieldInstructionCache;
            GameManager.UI.ShowMenuUI(MenuInfoBag.GameOverText, MenuInfoBag.Restart, MenuInfoBag.Menu, MenuInfoBag.Settings, MenuInfoBag.Exit);
        }
    }

    private void OnPause(object sender, EventArgs e)
    {
        m_inputActions.Character.Disable();
        if (m_isMenuRequested)
        {
            GameManager.UI.ShowMenuUI(MenuInfoBag.PausedText, MenuInfoBag.Continue, MenuInfoBag.Menu, MenuInfoBag.Settings, MenuInfoBag.Exit);
            m_isMenuShown = true;
            m_isMenuRequested = false;
        }
    }
    
    private void OnResume(object sender, EventArgs e)
    {
        if (m_isMenuShown)
        {
            GameManager.UI.HideMenuUI();
            m_isMenuShown = false;
        }
        
        m_inputActions.Character.Enable();
    }

    private void OnNotificationActivated(object sender, Notification e)
    {
        if (e is ModalDialog dialog)
        {
            ShowModalDialog(dialog);
        }
    }

    private void ShowModalDialog(ModalDialog dialog)
    {
        GameManager.Instance.SetPause();

        m_dialogPresenter = ManagerRoot.UI.ShowPopupUI<UIModalDialogPresenter>();
        m_dialogPresenter.SetDialog(dialog, OnConfirm);
        m_isModal = true;

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
                GameManager.Instance.SetResume();
                m_isModal = false;
            }
            else
            {
                ShowModalDialog(dialog.RelatedDialog);
            }
        }
    }

    private static class MenuInfoBag
    {
        public static readonly MenuInfo Restart = new("Restart from checkpoint", () => SceneManagerEx.LoadScene(SceneManager.GetActiveScene().name));
    
        public static readonly MenuInfo Continue = new("Return to game", () => GameManager.Instance.SetResume());
    
        public static readonly MenuInfo Menu = new("Exit to main menu", () => SceneManagerEx.LoadScene("TitleScene"));
        
        public static readonly MenuInfo Settings = new("Settings", () => ManagerRoot.UI.ShowPopupUI<UISettings>());
        
        public static readonly MenuInfo Credits = new("Credits", () => ManagerRoot.UI.ShowPopupUI<UICredits>());
    
        public static readonly MenuInfo Exit =
#if UNITY_EDITOR
            new("Exit the game", () => UnityEditor.EditorApplication.isPlaying = false);
#else
            new("Exit the game", Application.Quit);
#endif
        public static readonly string GameOverText = "Game Over";
        
        public static readonly string PausedText = "Paused";
        
        public static readonly string GameClearText = "Game Clear";
    }
}