using UnityEngine;

public class BlockAnimBehaviour : StateMachineBehaviour
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
            m_actor.Status.IsBlocking = true;               
        }
    }
}
