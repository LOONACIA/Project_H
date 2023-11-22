using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using TMPro;
using BehaviorDesigner.Runtime;

public class AnimationAction : Action
{
    private Monster m_monster;
    
    public SharedBool isAttacking;
    public override void OnAwake()
    {
        m_monster = GetComponent<Monster>();
    }

    // Start is called before the first frame update
    public override TaskStatus OnUpdate()
    {
        m_monster.TryAttack();
        isAttacking.Value = m_monster.Attack.IsAttacking;
        if (isAttacking.Value)
        {
            Debug.Log("공격중");
            return TaskStatus.Running;
        }
        Debug.Log(isAttacking.Value);
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        isAttacking = false;
    }
}
