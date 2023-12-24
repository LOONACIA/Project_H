using LOONACIA.Unity;
using System;
using UnityEngine;

public class Quest : Notification
{
    [field: SerializeField]
    public bool IsMainQuest { get; set; }
    
    [field: SerializeField]
    [field: ReadOnly]
    public bool IsCompleted { get; set; }
}