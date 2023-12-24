using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOONACIA.Unity.Managers
{
    public static class SceneManagerEx
    {
        public static Action<Scene> SceneChanging;

        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneChanging?.Invoke(currentScene);
            
            SceneManager.LoadScene(sceneName, mode);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneChanging?.Invoke(currentScene);

            return SceneManager.LoadSceneAsync(sceneName, mode);
        }
    }
}