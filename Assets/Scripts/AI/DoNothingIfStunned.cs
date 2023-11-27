using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingIfStunned : Action
{
    private ActorStatus m_status;
    
    public override void OnAwake()
    {
        base.OnAwake();

        m_status = GetComponent<ActorStatus>();
    }

    public override TaskStatus OnUpdate()
    {
        return m_status.IsStunned ? TaskStatus.Running : TaskStatus.Failure;
    }
}
