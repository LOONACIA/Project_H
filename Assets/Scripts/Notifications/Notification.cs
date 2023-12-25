using System;
using UnityEngine;

[Serializable]
public abstract class Notification
{
    [field: SerializeField]
    public int Id { get; set; }
    
    [field: SerializeField]
    public string Content { get; set; }
    
    public bool IsNotified { get; set; }
}