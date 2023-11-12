using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Skill: MonoBehaviour
{
    public abstract void Cast(Monster caster, Animator animator);
    public virtual void OnSkillVfxEvent() { }
    public virtual void OnSkillAnimationEvent() { }
    public virtual void OnSkillAnimationEnd() { }

}
