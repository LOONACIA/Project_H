using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckIsHit : Conditional
{
    private Monster m_owner;

    private ActorHealth m_health;

    public SharedBool IsHit;
    // Start is called before the first frame update
    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
        m_health = GetComponent<ActorHealth>();
        m_health.Damaged += OnDamaged;
    }

    public override TaskStatus OnUpdate()
    {
        if(IsHit.Value == true)
        {
            return TaskStatus.Success;

        }
        return TaskStatus.Failure;
        // 마지막 호출 이후 대미지를 입었고, 공격자가 빙의된 상태라면
        //return IsHit.Value == true ? TaskStatus.Success : TaskStatus.Failure;

    }

    private void OnDamaged(object sender, in AttackInfo e)
    {
        // 대미지를 입으면 Attacker에 대입
        IsHit.Value = true;
    }
}
