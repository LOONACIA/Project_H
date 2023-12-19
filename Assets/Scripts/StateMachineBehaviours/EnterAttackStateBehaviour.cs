using UnityEngine;

public class EnterAttackStateBehaviour : StateMachineBehaviour
{
    private Actor m_actor;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_actor == null)
        {
            m_actor = animator.gameObject.GetComponentInParent<Actor>();
        }
        
        if (m_actor != null)
        {
            m_actor.Status.IsBlocking = false;
        }

        animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_ABILITY, false);
    }
}