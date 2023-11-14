using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimationEventReceiver : MonoBehaviour
{
    public event EventHandler SkillStart;
	
    public event EventHandler SkillFinish;

    public event EventHandler<IEnumerable<IHealth>> SkillHit; 
	

    protected virtual void OnSkillAnimationStart()
    {
        SkillStart?.Invoke(this, EventArgs.Empty);
    }
	
    protected virtual void OnSkillAnimationEnd()
    {
        SkillFinish?.Invoke(this, EventArgs.Empty);
    }
	
    protected virtual void OnSkillHit(IEnumerable<IHealth> targets)
    {
        SkillHit?.Invoke(this, targets);
    }
}
