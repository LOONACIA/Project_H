using System;
using UnityEngine;

public interface IHealth
{
    GameObject gameObject { get; }

    int CurrentHp { get; }
    
    int MaxHp { get; }
    
    void Kill();
    
    event RefEventHandler<AttackInfo> Damaged;

    event RefEventHandler<AttackInfo> Dying;
    
    event EventHandler Died;
    
    void TakeDamage(in AttackInfo damageInfo);
}
