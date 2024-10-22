using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SetTargetIfDamaged : Conditional
{
    public SharedTransform Target;

    private Monster m_owner;

    private ActorHealth m_health;

    private Actor m_attacker;
    
    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
        m_health = GetComponent<ActorHealth>();
        m_health.Damaged += OnDamaged;
    }

    public override TaskStatus OnUpdate()
    {
        // 마지막 호출 이후 대미지를 입었고, 공격자가 빙의된 상태라면
        if (m_attacker != null && m_attacker.IsPossessed)
        {
            // Owner의 Target list에 Attacker가 없다면
            if (!m_owner.Targets.Contains(m_attacker))
            {
                //플레이어를 제외하고 모두 삭제
                m_owner.Targets.Clear();
                // Target list에 추가
                m_owner.Targets.Add(m_attacker);
            }

            Target.Value = null;
            m_attacker = null;
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
    
    private void OnDamaged(object sender, in AttackInfo e)
    {
        if (e.Attacker is null)
        {
            return;
        }
        
        // 대미지를 입으면 Attacker에 대입
        if (e.Attacker.TryGetComponent<Actor>(out var actor))
        {
            m_attacker = actor;
        }
    }
}
