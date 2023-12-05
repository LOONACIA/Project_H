using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;
using UnityEngine;

public class UpdateStatus : Action
{
    public SharedTransform Self;

    public SharedGameObject SelfObject;

    public SharedGameObject Body;
    
    public SharedTransform Target;

    public SharedGameObject TargetObject;

    public SharedBool IsTargetPossessed;

    public SharedInt Hp;

    public SharedFloat AttackDistance;

    public SharedBool isAttacking;

    private Monster m_owner;
    
    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
        
        Self.Value = transform;
        SelfObject.Value = gameObject;
    }

    public override void OnStart()
    {
        Body.Value = m_owner.Animator.gameObject;
        Hp.Value = m_owner.Status.Hp;
        isAttacking.Value = m_owner.Attack.IsAttacking;

        if (m_owner.Targets.Count == 0)
        {
            Target.Value = null;
            return;
        }
        
        Actor closestTarget = m_owner.Targets
            .OrderBy(target => Vector3.Distance(transform.position, target.transform.position))
            .FirstOrDefault();

        if (closestTarget != null)
        {
            IsTargetPossessed.Value = closestTarget.IsPossessed;
            Target.Value = closestTarget.transform;
            TargetObject.Value = closestTarget.gameObject;
        }
        else
        {
            IsTargetPossessed.Value = false;
            Target.Value = null;
            TargetObject.Value = null;
        }

        m_owner.Attack.Target = TargetObject.Value;
    }
}
