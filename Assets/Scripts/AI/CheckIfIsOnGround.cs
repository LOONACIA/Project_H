using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfIsOnGround : Conditional
{
    private Monster m_monster;

    public override void OnAwake()
    {
        base.OnAwake();

        m_monster = GetComponent<Monster>();
    }

    public override TaskStatus OnUpdate()
    {
        return m_monster.Movement.IsOnGround ? TaskStatus.Success : TaskStatus.Failure;
    }
}
