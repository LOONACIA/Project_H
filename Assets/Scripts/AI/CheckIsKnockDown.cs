using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

//[TaskIcon("{SkinColor}IdleIcon.png")]
public class CheckIsKnockDown : Action
{
    public SharedBool IsKnockDown;

    private Monster m_owner;

    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
    }

    public override TaskStatus OnUpdate()
    {
        IsKnockDown.Value = m_owner.Status.IsKnockedDown;
        if (IsKnockDown.Value == true)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}