using DG.Tweening;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    private string m_gameSceneName;
    
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

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        m_canvasGroup = m_pressAnyKeyLabel.GetComponent<CanvasGroup>();
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
        }
    }

    public void OnPlayButtonClick()
    {
        if (m_isStarted)
        {
            return;
        }
        
        m_inputHandle?.Dispose();
        
        m_animator.Play("TitleMenu_Out");
        m_isStarted = true;
        CoroutineEx.Create(GameManager.Instance, WaitForLoadScene(m_gameSceneName));
    }
    
    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator WaitForLoadScene(string sceneName)
    {
        yield return new WaitForSeconds(ReachUIInternalTools.GetAnimatorClipLength(m_animator, "TitleMenu_Out"));
        AsyncOperation task = SceneManagerEx.LoadSceneAsync(sceneName);
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