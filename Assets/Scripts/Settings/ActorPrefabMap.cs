using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActorPrefabMap
{
    [field: SerializeField]
    public ActorType Type { get; private set; }
    
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
