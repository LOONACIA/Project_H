using UnityEngine;

public class FirstPersonDefaultAnimBehaviour : StateMachineBehaviour
{
    private IEventProxy m_eventProxy;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_ATTACK);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_SKILL);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_POSSESS);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);
        animator.ResetTrigger(ConstVariables.ANIMATOR_PARAMETER_TARGET_CHECK);
        
        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }
        
        //공격 무기에 IDLE 시그널 보냄
        m_eventProxy.DispatchEvent($"On{nameof(WeaponState.Idle)}");
        
        // AttackAnimationEventReceiver receiver = animator.GetComponent<AttackAnimationEventReceiver>();
        // if (receiver != null)
        // {
        //     receiver.OnAttackIdle();
        //     receiver.OnBlockPushIdle();
        //     receiver.OnSkillIdle();
        // }

        // 방어가 끝남
        if (animator.GetBool(ConstVariables.ANIMATOR_PARAMETER_BLOCK) == false)
        { 
            var status = animator.gameObject.GetComponentInParent<ActorStatus>();
            if (status != null)
            {
                status.IsBlocking = false;
            }
        }
    }
}
