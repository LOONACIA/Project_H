using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
	public static class GameObjectExtension
	{
		public static T GetOrAddComponent<T>(this GameObject gameObject)
			where T : Component
		{
			if (!gameObject.TryGetComponent(out T component))
			{
				component = gameObject.AddComponent<T>();
			}

			return component;
		}
	
		public static GameObject FindChild(this GameObject gameObject, string name = null, bool recursive = true)
		{
			if (gameObject.FindChild<Transform>(name, recursive) is { } transform)
			{
				return transform.gameObject;
			}

			return null;
		}

		public static T FindChild<T>(this GameObject gameObject, string name = null, bool recursive = true)
			where T : Object
		{
			if (gameObject == null)
			{
				return null;
			}

			if (recursive)
			{
				return gameObject.GetComponentsInChildren<T>(true).
					FirstOrDefault(component => string.IsNullOrEmpty(name) || component.name == name);
			}
			else
			{
				for (var index = 0; index < gameObject.transform.childCount; index++)
				{
					var transform = gameObject.transform.GetChild(index);
					if (!string.IsNullOrEmpty(name) && transform.name != name)
					{
						continue;
					}
				
					if (transform.TryGetComponent(out T component))
					{
						return component;
					}
				}
			}

			return null;
		}
	}
}