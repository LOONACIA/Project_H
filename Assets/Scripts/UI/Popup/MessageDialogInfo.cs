using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MessageDialogInfo
{
    [field: SerializeField]
    [field: TextArea]
    public string Message { get; private set; }
    
    [field: SerializeField]
    public UnityEvent Callback { get; private set; }
}