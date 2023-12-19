using UnityEngine;

public class ThirdPersonDefaultAnimBehaviour : StateMachineBehaviour
{
    private IEventProxy m_eventProxy;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_ATTACK);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_DEAD);
        //animator.ResetTrigger("KnockDown");
        animator.ResetTrigger("KnockBack");

        animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_IS_HIT_PLAYING, false);
        
        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }
        
        //공격 무기에 IDLE 시그널 보냄
        m_eventProxy.DispatchEvent($"On{nameof(WeaponState.Idle)}");
    }
}
