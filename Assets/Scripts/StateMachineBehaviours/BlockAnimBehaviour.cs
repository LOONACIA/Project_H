using UnityEngine;

public class BlockAnimBehaviour : StateMachineBehaviour
{
    private Actor m_actor;

    private IEventProxy m_eventProxy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_actor == null)
        {
            m_actor = animator.gameObject.GetComponentInParent<Actor>();
        }
        
        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }

        if (m_actor != null)
        {
            m_actor.Status.IsBlocking = true;
            m_eventProxy?.DispatchEvent($"On{nameof(AbilityState.Activate)}");
        }
    }
}