using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOONACIA.Unity.Managers
{
    public static class SceneManagerEx
    {
        public static Action<string> SceneChanging;

        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneChanging?.Invoke(sceneName);
            
            SceneManager.LoadScene(sceneName, mode);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneChanging?.Invoke(sceneName);
            
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }
    }
}