using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;

public class LogoScene : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void LoadNextScene()
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
    #endregion

    #region PrivateMethod
    #endregion
}
