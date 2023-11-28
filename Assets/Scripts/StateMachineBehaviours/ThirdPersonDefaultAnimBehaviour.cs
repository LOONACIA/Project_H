using UnityEngine;

public class ThirdPersonDefaultAnimBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Dead");
        //animator.ResetTrigger("KnockDown");
        animator.ResetTrigger("KnockBack");
        
        //공격 무기에 IDLE 시그널 보냄
        AttackAnimationEventReceiver receiver = animator.GetComponent<AttackAnimationEventReceiver>();
        if (receiver != null)
        {
            receiver.OnAttackIdle();
            receiver.OnBlockPushIdle();
            receiver.OnSkillIdle();
        }
    }
}
