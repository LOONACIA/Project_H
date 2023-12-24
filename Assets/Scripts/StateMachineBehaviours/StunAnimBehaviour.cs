using UnityEngine;

public class StunAnimBehaviour : StateMachineBehaviour
{
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
    }
}
