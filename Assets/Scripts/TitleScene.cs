using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
	public void OnStartGameClicked()
    {
        AsyncOperation loadInfo = SceneManagerEx.LoadSceneAsync("MainBuild");

        CoroutineEx.Create(this, WaitForLoadScene(loadInfo));
    }

    public IEnumerator WaitForLoadScene(AsyncOperation loadInfo)
    {
        if (loadInfo == null)
        {
            yield break;
        }

        while (true)
        {
            Debug.Log($"로드 중:{loadInfo.progress}");
            if (loadInfo.isDone)
            {
                break;
            }
            yield return null;
        }
        Debug.Log("아무 키나 누르면 넘어갑니다.");
        while (true)
        {
            if (loadInfo.isDone && Input.anyKeyDown)
            {
                loadInfo.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
