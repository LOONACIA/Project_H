using DG.Tweening;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System.Collections;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    private string m_gameSceneName;
    
    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private GameObject m_pressAnyKeyLabel;
    
    private CanvasGroup m_canvasGroup;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        m_canvasGroup = m_pressAnyKeyLabel.GetComponent<CanvasGroup>();
    }

    public void OnPlayButtonClick()
    {
        m_animator.Play("TitleMenu_Out");
        CoroutineEx.Create(GameManager.Instance, WaitForLoadScene(m_gameSceneName));
    }
    
    public void OnExitButtonClick()
    {
        Application.Quit();
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
                    //m_pressAnyKeyLabel.gameObject.SetActive(true);
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
}