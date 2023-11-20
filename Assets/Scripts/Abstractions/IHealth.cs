using System;
using UnityEngine;

public interface IHealth
{
    GameObject gameObject { get; }

    int CurrentHp { get; }
    
    int MaxHp { get; }
    
    void Kill();
    
    event EventHandler<Actor> Damaged;

    event EventHandler Dying;
    
    event EventHandler Died;
    
    void TakeDamage(int damage, Vector3 attackDirection, Actor attacker);
}
