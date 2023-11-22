using System;
using UnityEngine;

public interface IHealth
{
    GameObject gameObject { get; }

    int CurrentHp { get; }
    
    int MaxHp { get; }
    
    void Kill();
    
    event EventHandler<DamageInfo> Damaged;

    event EventHandler<DamageInfo> Dying;
    
    event EventHandler Died;
    
    void TakeDamage(DamageInfo damageInfo);
}
