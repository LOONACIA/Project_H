using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

//[TaskIcon("{SkinColor}IdleIcon.png")]
public class CheckIsKnockBack : Action
{
    public SharedBool IsKnockBack;

    private Monster m_owner;

    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
    }

    public override TaskStatus OnUpdate()
    {
        IsKnockBack.Value = m_owner.Status.IsKnockBack;
        if (IsKnockBack.Value == true)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}