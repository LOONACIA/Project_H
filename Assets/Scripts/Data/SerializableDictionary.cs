using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
    private List<SerializablePair<TKey, TValue>> m_pairs = new();

    public void OnBeforeSerialize()
    {
        m_pairs = this.Select(kvp => new SerializablePair<TKey, TValue>(kvp.Key, kvp.Value)).ToList();
    }

    public void OnAfterDeserialize()
    {
        Clear();
        foreach (var pair in m_pairs)
        {
            if (!TryAdd(pair.Key, pair.Value))
            {
                KeyValuePair<TKey, TValue> kvp = new();
                TryAdd(kvp.Key, pair.Value);
            }
        }
    }
}
