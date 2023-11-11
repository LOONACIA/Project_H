using System.Collections.Generic;
using LOONACIA.Unity.Pool;
using UnityEngine;
using UnityEngine.Pool;

namespace LOONACIA.Unity.Managers
{
	public class PoolManager
	{
		private readonly Dictionary<string, GameObject> _originals = new();

		private readonly Dictionary<string, IObjectPool<Poolable>> _registeredPools = new();

		private Transform _root;
		
		private Transform Root
		{
			get
			{
				Init();
				return _root;
			}
		}

		public void Init()
		{
			if (_root == null)
			{
				_root = new GameObject { name = "@Pool_Root" }.transform;
			}
		}

		public Poolable Get(string name)
		{
			if (!_originals.TryGetValue(name, out GameObject original))
			{
				original = Resources.Load<GameObject>($"Prefabs/{name}");
				_originals.Add(name, original);
			}

			return Get(original);
		}

		public Poolable Get(GameObject prefab)
		{
			Init(prefab);
			return _registeredPools[prefab.name].Get();
		}

		public void Release(Poolable pooledObject)
		{
			pooledObject.transform.SetParent(Root);
			pooledObject.Pool.Release(pooledObject);
		}

		public void Clear(bool destroyAssociatedObject)
		{
			foreach (Transform child in _root)
			{
				Object.Destroy(child.gameObject);
			}

			_originals.Clear();
			_registeredPools.Clear();

			if (destroyAssociatedObject)
			{
				Object.Destroy(_root.gameObject);
			}
		}

		private Poolable Create(GameObject original)
		{
			var obj = Object.Instantiate(original);
			obj.name = original.name;
			var poolable = obj.GetOrAddComponent<Poolable>();
			poolable.Pool = _registeredPools[original.name];
			return poolable;
		}

		private void OnGet(Poolable pooledObject)
		{
			pooledObject.gameObject.SetActive(true);
		}

		private void OnRelease(Poolable pooledObject)
		{
			pooledObject.gameObject.SetActive(false);
		}

		private void OnDestroy(Poolable pooledObject)
		{
			Object.Destroy(pooledObject.gameObject);
		}

		private void Init(GameObject original)
		{
			if (_registeredPools.ContainsKey(original.name))
			{
				return;
			}

			IObjectPool<Poolable> pool = new ObjectPool<Poolable>
			(
				createFunc: () => Create(original),
				actionOnGet: OnGet,
				actionOnRelease: OnRelease,
				actionOnDestroy: OnDestroy
			);

			_registeredPools.Add(original.name, pool);
		}
	}
}