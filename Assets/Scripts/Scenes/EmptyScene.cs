using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;

public class EmptyScene : MonoBehaviour
{
	private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        var task = SceneManagerEx.LoadSceneAsync("TitleScene");
        task.allowSceneActivation = false;
        while (!task.isDone)
        {
            if (task.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);
                task.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
