using UnityEngine;

public class FirstPersonDefaultAnimBehaviour : StateMachineBehaviour
{
    private Actor m_actor;
    
    private IEventProxy m_eventProxy;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_ATTACK);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_ABILITY);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_POSSESS);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_TARGET_CHECK);
        
        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }
        
        //공격 무기에 IDLE 시그널 보냄
        m_eventProxy.DispatchEvent($"On{nameof(WeaponState.Idle)}");

        if (m_actor == null)
        {
            m_actor = animator.gameObject.GetComponentInParent<Actor>();
        }
        
        // 방어가 끝남
        if (m_actor != null && !animator.GetBool(ConstVariables.ANIMATOR_PARAMETER_ABILITY))
        {
            m_actor.Status.IsBlocking = false;               
        }
    }
}
