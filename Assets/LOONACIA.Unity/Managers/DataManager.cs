using System.Collections.Generic;
using UnityEngine;

namespace LOONACIA.Unity.Managers
{
	public class DataManager
	{
		private readonly Dictionary<string, object> _data = new();

		public T Load<T>(string name = null)
			where T : class
		{
			if (string.IsNullOrEmpty(name))
			{
				name = typeof(T).Name;
			}

			if (_data.TryGetValue(name, out object cache))
			{
				return cache as T;
			}

			var textAsset = ManagerRoot.Resource.Load<TextAsset>($"Data/{name}");
			var data = JsonUtility.FromJson<T>(textAsset.text);
			_data.Add(name, data);

			return data;
		}
	}
}
