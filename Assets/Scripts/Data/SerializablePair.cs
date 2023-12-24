using System;
using UnityEngine;

[Serializable]
public class SerializablePair<TKey, TValue>
{
    public SerializablePair()
    {
        
    }
    
    public SerializablePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
    
    [field: SerializeField]
    public TKey Key { get; set; }
    
    [field: SerializeField]
    public TValue Value { get; set; }
}