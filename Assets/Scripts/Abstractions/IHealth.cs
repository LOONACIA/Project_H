using System;
using UnityEngine;

public interface IHealth
{
    GameObject gameObject { get; }

    int CurrentHp { get; set; }
    
    int MaxHp { get; }
    
    void Kill();
    
    event EventHandler<int> HealthChanged;

    event EventHandler Dying;
    
    event EventHandler Died;
}
