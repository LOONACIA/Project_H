using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonDefaultAnimBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_ATTACK);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_SKILL);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_POSSESS);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_TARGET_CHECK);
        
        animator.GetComponent<AttackAnimationEventReceiver>().OnAttackIdle();
    }
}
