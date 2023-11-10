using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	public event EventHandler AttackStart;
	
	public event EventHandler AttackFinish;
	
	public event EventHandler<IEnumerable<IHealth>> AttackHit;

	public event EventHandler SkillStart;
	
	public event EventHandler SkillFinish;

	public event EventHandler<IEnumerable<IHealth>> SkillHit; 
	
	protected virtual void OnAttackAnimationStart()
	{
        AttackStart?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnAttackAnimationEnd()
	{
        AttackFinish?.Invoke(this, EventArgs.Empty);
	}
	
	protected virtual void OnAttackHit(IEnumerable<IHealth> targets)
	{
		AttackHit?.Invoke(this, targets);
	}

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
