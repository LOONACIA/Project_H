using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsStunned : Conditional
{
    private ActorStatus m_status;

	public override void OnAwake()
    {
        m_status = GetComponent<ActorStatus>();
    }

    public override TaskStatus OnUpdate()
    {
        if (m_status.IsStunned == true)
        {
            return TaskStatus.Success;

        }
        return TaskStatus.Failure;
    }
}
