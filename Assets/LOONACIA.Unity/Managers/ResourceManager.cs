using LOONACIA.Unity.Pool;
using UnityEngine;

namespace LOONACIA.Unity.Managers
{
	public class ResourceManager
	{
		public T Load<T>(string path) where T : Object
		{
			return Resources.Load<T>(path);
		}

		public GameObject Instantiate(string name, Transform parent = null, bool usePool = true)
		{
			if (usePool)
			{
				var poolable = ManagerRoot.Pool.Get(name);
				poolable.transform.parent = parent;
				return poolable.gameObject;
			}
			else
			{
				GameObject original = Load<GameObject>($"Prefabs/{name}");
				if (original == null)
				{
					UnityEngine.Debug.Log($"Failed to load: {original}");
					return null;
				}

				return Object.Instantiate(original, parent);
			}
		}

		public GameObject Instantiate(GameObject prefab, Transform parent = null, bool usePool = true)
		{
			if (usePool)
			{
				var poolable = ManagerRoot.Pool.Get(prefab);
				poolable.transform.parent = parent;
				poolable.transform.localPosition = Vector3.zero;
				return poolable.gameObject;
			}
			else
			{
				return Object.Instantiate(prefab, parent);
			}
		}

		public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool usePool = true)
		{
			GameObject go = Instantiate(prefab, parent, usePool);
			go.transform.position = position;
			go.transform.rotation = rotation;

			return go;
		}

		public void Release(GameObject go)
		{
			if (go == null)
			{
				return;
			}

			if (go.TryGetComponent<Poolable>(out var poolable))
			{
				ManagerRoot.Pool.Release(poolable);
			}
			else
			{
				Object.Destroy(go);
			}
		}
	}
}