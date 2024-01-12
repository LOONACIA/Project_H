using UnityEngine;

public class BossIdleAnimBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_LOOK, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_LOOK, false);
    }
}
