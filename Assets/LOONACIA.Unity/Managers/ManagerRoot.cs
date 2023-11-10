using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOONACIA.Unity.Managers
{
	public class ManagerRoot : MonoBehaviour
	{
		private static Lazy<ManagerRoot> s_lazyInstance = new(CreateInstance);

		private static bool s_isApplicationQuitting;

		private readonly InputManager _input = new();

		private readonly PoolManager _pool = new();

		private readonly ResourceManager _resource = new();

		private readonly UIManager _ui = new();

		public static ManagerRoot Instance => s_lazyInstance.Value;

		public static InputManager Input => Instance._input;

		public static PoolManager Pool => Instance._pool;

		public static ResourceManager Resource => Instance._resource;

		public static UIManager UI => Instance._ui;
		
		public static void Clear(bool destroyAssociatedObject = false)
		{
			UI.Clear(destroyAssociatedObject);
			Pool.Clear(destroyAssociatedObject);
		}

		private static ManagerRoot CreateInstance()
		{
			if (s_isApplicationQuitting)
			{
				return null;
			}
			
			if (FindObjectOfType<ManagerRoot>() is not { } instance)
			{
				GameObject go = new("@Managers");
				instance = go.AddComponent<ManagerRoot>();
			}
			
			DontDestroyOnLoad(instance);
			SceneManagerEx.SceneChanging += instance.OnSceneChanging;
			
			instance.Init();

			return instance;
		}

		private void Init()
		{
			_input.Init();
			_pool.Init();
			_ui.Init();
		}

		private void OnSceneChanging(Scene obj)
		{
			Clear(false);
		}

		private void OnDestroy()
		{
			s_isApplicationQuitting = true;
		}

		private void OnApplicationQuit()
		{
			s_isApplicationQuitting = true;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				s_isApplicationQuitting = true;
			}
		}
	}
}