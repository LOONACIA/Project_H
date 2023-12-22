using JetBrains.Annotations;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [Tooltip("없으면 자식에서 찾습니다.")]
    private ProgressBar loadingBar;

    private CanvasGroup canvasGroup;
    public GameObject pressAnyKeyText;

    private void Awake()
    {
        if (loadingBar == null)
        {
            loadingBar = GetComponentInChildren<ProgressBar>();
        }
        if (canvasGroup == null)
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        Cursor.visible = true;
        //Application.targetFrameRate = 60;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        canvasGroup.alpha = 0f;
    }

    public void OnStartGameClicked(string sceneName)
    {
        gameObject.SetActive(true);
        pressAnyKeyText.gameObject.SetActive(false);
        CoroutineEx.Create(GameManager.Instance, WaitForLoadScene(sceneName));
    }

    private IEnumerator WaitForLoadScene(string nextScene)
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * 2f;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);

        string oldScene = SceneManager.GetActiveScene().name;

        //2. 씬 로더를 DDOL
        //DontDestroyOnLoad(gameObject);

        //1. 다음 씬 로드
        AsyncOperation loadInfo = SceneManagerEx.LoadSceneAsync(nextScene);
        if (loadInfo == null)
        {
            yield break;
        }

        loadInfo.allowSceneActivation = false;

        if (loadingBar != null)
        {
            loadingBar.minValue = 0f;
            loadingBar.maxValue = 0.9f;
        }

        while (!loadInfo.isDone)
        {
            if (loadingBar != null)
            {
                loadingBar.SetValue(loadInfo.progress);
            }
            
            if (loadInfo.progress >= 0.9f)
            {
                pressAnyKeyText.gameObject.SetActive(true);
                if (Input.anyKeyDown)
                {
                    loadInfo.allowSceneActivation = true;
                }
            }
            
            yield return null;
        }

        loadingBar.SetValue(loadInfo.progress);
    }


    public void OnExitClicked()
    {
        Application.Quit();
    }
}