using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public event EventHandler<IEnumerable<IHealth>> AttackHit;

    public void InvokeAttackHit(IEnumerable<IHealth> hitObjects){ AttackHit?.Invoke(this, hitObjects);}
    
    
}
