using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEventReceiver : MonoBehaviour
{
    public event EventHandler AttackStart;
	
    public event EventHandler AttackFinish;
	
    public event EventHandler AttackHit;
	
    protected virtual void OnAttackAnimationStart()
    {
        AttackStart?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnAttackAnimationEnd()
    {
        AttackFinish?.Invoke(this, EventArgs.Empty);
    }
	
    protected virtual void OnHitBoxCheck()
    {
        AttackHit?.Invoke(this, EventArgs.Empty);
    }
	
}
