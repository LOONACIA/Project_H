using LOONACIA.Unity.Console;
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
    
    private CoroutineEx m_dialogCloseCoroutine;
    
    private DebugController m_debugController;

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

        m_debugController = FindObjectOfType<DebugController>();
        if (m_debugController != null)
        {
            m_debugController.Toggled += OnDebugToggled;
        }
        
        EnableInput();
    }

    private void OnDisable()
    {
        GameManager.Instance.GameClear -= OnGameClear;
        GameManager.Instance.GameOver -= OnGameOver;
        GameManager.Instance.Pause -= OnPause;
        GameManager.Instance.Resume -= OnResume;
        GameManager.Notification.Activated -= OnNotificationActivated;
        
        if (m_debugController != null)
        {
            m_debugController.Toggled -= OnDebugToggled;
        }
        
        DisableInput();
    }

    private void Pause()
    {
        if (ManagerRoot.UI.ClosePopupUI())
        {
            return;
        }

        if (GameManager.Instance.IsGameOver)
        {
            return;
        }
        
        m_isMenuRequested = true;
        GameManager.Instance.SetPause();
    }
    
    private void OnGameClear(object sender, EventArgs e)
    {
        //24.01.12: GameClear 시 UI 생성 대신 EndingScene으로 넘어가는 것으로 변경
        //GameManager.UI.ShowMenuUI(MenuInfoBag.GameClearText, MenuInfoBag.Menu, MenuInfoBag.Credits, MenuInfoBag.Exit);
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
            GameManager.UI.ShowMenuUI(MenuInfoBag.PausedText, MenuInfoBag.Continue, MenuInfoBag.Restart, MenuInfoBag.Menu, MenuInfoBag.Settings, MenuInfoBag.Exit);
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
            }
            else
            {
                ShowModalDialog(dialog.RelatedDialog);
            }
        }
    }
    
    private void OnDebugToggled(object sender, bool isToggled)
    {
        if (isToggled)
        {
            GameManager.Instance.SetPause();
        }
        else
        {
            GameManager.Instance.SetResume();
        }
    }

    private static class MenuInfoBag
    {
        public static readonly MenuInfo Restart = new("restart_checkpoint_button", () => SceneManagerEx.LoadScene(SceneManager.GetActiveScene().name));
    
        public static readonly MenuInfo Continue = new("return_game_button", () => GameManager.Instance.SetResume());
    
        public static readonly MenuInfo Menu = new("exit_menu_button", () =>
        {
            GameManager.Sound.ResetBGMTime();
            SceneManagerEx.LoadScene("TitleScene");
        });
        
        public static readonly MenuInfo Settings = new("settings_button", () => ManagerRoot.UI.ShowPopupUI<UISettings>());
        
        public static readonly MenuInfo Credits = new("credits_button", () => ManagerRoot.UI.ShowPopupUI<UICredits>());
    
        public static readonly MenuInfo Exit =
#if UNITY_EDITOR
            new("exit_game_button", () => UnityEditor.EditorApplication.isPlaying = false);
#else
            new("exit_game_button", Application.Quit);
#endif
        public static readonly string GameOverText = "Game Over";
        
        public static readonly string PausedText = "Paused";
        
        public static readonly string GameClearText = "Game Clear";
    }
}