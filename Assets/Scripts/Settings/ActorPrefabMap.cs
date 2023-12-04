using System;
using UnityEngine;

[Serializable]
public class ActorPrefabMap
{
    [field: SerializeField]
    public ActorType Type { get; private set; }
    
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
