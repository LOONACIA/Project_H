using UnityEngine;

public class EnterAttackStateBehaviour : StateMachineBehaviour
{   
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var status = animator.gameObject.GetComponentInParent<ActorStatus>();
        if (status != null)
        {
            status.IsBlocking = false;
        }

        animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_BLOCK, false);
    }
}
