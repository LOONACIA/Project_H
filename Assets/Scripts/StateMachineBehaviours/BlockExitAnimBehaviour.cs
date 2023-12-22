using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockExitAnimBehaviour : StateMachineBehaviour
{
    private IEventProxy m_eventProxy;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }
        
        m_eventProxy?.DispatchEvent("OnBlockExit");
    }
}
