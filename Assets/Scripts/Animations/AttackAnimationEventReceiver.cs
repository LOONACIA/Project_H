using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEventReceiver : MonoBehaviour
{
    public event EventHandler onIdle;
    public event EventHandler onLeadIn;
    public event EventHandler onHit;
    public event EventHandler onFollowThrough;
	
    public virtual void OnAttackIdle()
    {
        onIdle?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnAttackLeadIn()
    {
        onLeadIn?.Invoke(this, EventArgs.Empty);
    }
	
    protected virtual void OnAttackHit()
    {
        onHit?.Invoke(this, EventArgs.Empty);
    }
    
    
    protected virtual void OnAttackFollowThrough()
    {
        onFollowThrough?.Invoke(this, EventArgs.Empty);
    }
	
}
