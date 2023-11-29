using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTargetChanged : Conditional
{
    public SharedTransform target;
    private Transform m_beforeTarget;

    public override void OnAwake()
    {
        base.OnAwake();

        m_beforeTarget = target.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_beforeTarget != target.Value)
        {
            m_beforeTarget = target.Value;
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}
