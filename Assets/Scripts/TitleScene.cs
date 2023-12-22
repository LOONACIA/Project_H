using JetBrains.Annotations;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [Tooltip("없으면 자식에서 찾습니다.")]
    public ProgressBar loadingBar;

    private void OnEnable()
    {
        if (loadingBar == null)
        {
            loadingBar = GetComponentInChildren<ProgressBar>();
        }
    }

    public void OnStartGameClicked(string sceneName)
    {
        CoroutineEx.Create(GameManager.Instance, WaitForLoadScene(sceneName));
    }

    private IEnumerator WaitForLoadScene(string nextScene)
    {
        yield return new WaitForSeconds(0.5f);

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
            loadingBar.maxValue = 1f;
        }

        while (!loadInfo.isDone)
        {
            if (loadingBar != null)
            {
                loadingBar.SetValue(loadInfo.progress);
            }
            
            if (loadInfo.progress >= 0.9f)
            {
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