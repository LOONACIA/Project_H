using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpdateStatus : Action
{
    public SharedTransform Self;
    
    public SharedTransform Target;

    public SharedBool IsTargetPossessed;

    public SharedInt Hp;

    public SharedFloat AttackDistance;

    public SharedBool isAttacking;

    private Monster m_owner;
    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
        
        Self.Value = transform;
    }

    public override void OnStart()
    {
        Actor closestTarget = m_owner.Targets
            .OrderBy(target => Vector3.Distance(transform.position, target.transform.position))
            .FirstOrDefault();

        IsTargetPossessed.Value = closestTarget != null && closestTarget.IsPossessed;
        Target.Value = closestTarget != null ? closestTarget.transform : null;

        Hp.Value = m_owner.Status.Hp;
        isAttacking.Value = m_owner.Attack.IsAttacking;
    }
}
