using DG.Tweening;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private GameObject m_pressAnyKeyLabel;

    [SerializeField]
    private GameObject m_firstFocus;

    private CanvasGroup m_canvasGroup;

    private IDisposable m_inputHandle;

    private bool m_isStarted;

    private UISettings m_settings;

    private bool m_isSettingsToggled;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        m_canvasGroup = m_pressAnyKeyLabel.GetComponent<CanvasGroup>();

        //사운드
        GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.TitleSceneBGM);
    }

    private void Update()
    {
        if (m_isSettingsToggled && (m_settings == null || !m_settings.isActiveAndEnabled))
        {
            GameManager.Sound.OnInGame();
            m_isSettingsToggled = false;
        }
    }

    private void OnEnable()
    {
        m_inputHandle = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
        m_isStarted = false;
    }

    private void OnDisable()
    {
        m_inputHandle?.Dispose();
    }

    public void OnSettingsButtonClick()
    {
        if (m_settings == null || !m_settings.gameObject.activeSelf)
        {
            m_settings = ManagerRoot.UI.ShowPopupUI<UISettings>();
            m_isSettingsToggled = true;
        }
    }

    public void OnPlayButtonClick()
    {
        if (m_isStarted)
        {
            return;
        }

        m_inputHandle?.Dispose();
        var selector = ManagerRoot.UI.ShowPopupUI<UIChapterSelector>();
        selector.Clear();
        foreach (var chapterInfo in GameManager.Settings.GameData.ChapterInfos)
        {
            selector.AddChapter(chapterInfo, () => GameStart(chapterInfo));
        }
    }

    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void GameStart(ChapterInfo chapterInfo)
    {
        ManagerRoot.UI.ClearAllPopup();
        m_animator.Play("TitleMenu_Out");
        m_isStarted = true;
        GameManager.Notification.Initialize();
        CoroutineEx.Create(GameManager.Instance, WaitForLoadScene(chapterInfo.SceneName));
    }

    private IEnumerator WaitForLoadScene(SceneName sceneName)
    {
        yield return new WaitForSeconds(ReachUIInternalTools.GetAnimatorClipLength(m_animator, "TitleMenu_Out"));
        AsyncOperation task = SceneManagerEx.LoadSceneAsync(sceneName.ToString());
        if (task == null)
        {
            Debug.LogError($"Failed to load scene: {sceneName}");
            yield break;
        }

        task.allowSceneActivation = false;
        yield return new WaitForSeconds(3f);

        bool canPressAnyKey = false;
        Tween tween = null;
        while (!task.isDone)
        {
            if (task.progress >= 0.9f)
            {
                if (!canPressAnyKey)
                {
                    canPressAnyKey = true;
                    tween = m_canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.InCubic);
                }

                if (Input.anyKeyDown)
                {
                    tween?.Kill();
                    DOTween.Sequence()
                        .Append(m_canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InCubic))
                        .AppendCallback(() => task.allowSceneActivation = true);
                }
            }

            yield return null;
        }
    }

    private void OnAnyButtonPress(InputControl inputControl)
    {
        if (!inputControl.device.name.ToLower().Contains("mouse") && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(m_firstFocus);
        }
    }
}