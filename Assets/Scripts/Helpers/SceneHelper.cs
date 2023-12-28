using LOONACIA.Unity.Managers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper
{
    public static async Task LazyLoadAsync(string sceneName, float delay = 1.5f)
    {
        ManagerRoot.UI.Clear(false);
        var dim = ManagerRoot.UI.ShowSceneUI<UIDim>();
        dim.FadeIn(delay);

        await Task.Delay((int)(delay * 1000));
        
        AsyncOperation task = SceneManagerEx.LoadSceneAsync(sceneName);
        task.allowSceneActivation = false;
        while (task.progress < 0.9f)
        {
            await Task.Delay(1);
        }

        SceneManager.sceneLoaded += FadeOutDim;
        
        task.allowSceneActivation = true;
        return;

        async void FadeOutDim(Scene arg0, LoadSceneMode arg1)
        {
            var dim = ManagerRoot.UI.ShowSceneUI<UIDim>();
            dim.GetComponentInChildren<CanvasGroup>().alpha = 1f;
            dim.FadeOut(delay);
            
            await Task.Delay((int)(delay * 1000));
            SceneManager.sceneLoaded -= FadeOutDim;
        }
    }
}
