using UnityEngine;

public class BlockAnimBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var status = animator.gameObject.GetComponentInParent<ActorStatus>();
        if (status != null)
        {
            status.IsBlocking = true;               
        }
    }
}
