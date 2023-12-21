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
        CoroutineEx.Create(this, WaitForLoadScene(sceneName));
    }    

    public IEnumerator WaitForLoadScene(string nextScene)
    {
        yield return new WaitForSeconds(0.5f);

        string oldScene = SceneManager.GetActiveScene().name;

        //2. 씬 로더를 DDOL
        DontDestroyOnLoad(this.gameObject);

        //1. 다음 씬 로드
        AsyncOperation loadInfo = SceneManagerEx.LoadSceneAsync(nextScene);
        if (loadInfo == null)
        {
            yield break;
        }

        //loadInfo.allowSceneActivation = false;

        if (loadingBar != null)
        {
            loadingBar.minValue = 0f;
            loadingBar.maxValue = 1f;
        }

        while (true)
        {
            if(loadingBar!=null)
            {
                loadingBar.SetValue(loadInfo.progress);
            }
            if (loadInfo.isDone)
            {
                break;
            }
            yield return null;
        }
        //loadInfo.allowSceneActivation = true;


        //3. 기존 씬 언로드
        //loadInfo = SceneManager.UnloadSceneAsync(oldScene);
        //while (true)
        //{
        //    if (loadingBar != null)
        //    {
        //        loadingBar.SetValue(0.45f + loadInfo.progress * 0.5f);
        //    }
        //    if (loadInfo.progress >= 0.9f)
        //    {
        //        break;
        //    }
        //    yield return null;
        //}

        //Loader 삭제
        yield return new WaitForSeconds(1.0f);
        //Destroy(this.gameObject);
    }


    public void OnExitClicked()
    {
        Application.Quit();
    }
}
