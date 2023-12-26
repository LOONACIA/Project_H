using LOONACIA.Unity;
using System;
using UnityEngine;

public class ARObject : MonoBehaviour, IARObject
{
    [field: SerializeField]
    [field: ReadOnly]
    public bool IsActivated { get; private set; }

    [field: SerializeField]
    public ARObjectInfo Info { get; private set; }

    private void OnEnable()
    {
        IsActivated = true;
    }

    private void OnDisable()
    {
        IsActivated = false;
    }
}
