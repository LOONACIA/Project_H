using BehaviorDesigner.Runtime;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

public class AlarmTarget : Action
{
    public SharedTransform Target;

    public SharedFloat Radius;

    public SharedLayerMask Recipient;

    private Monster m_actor;

    public override void OnAwake()
    {
        base.OnAwake();

        m_actor = GetComponent<Monster>();
    }

    public override TaskStatus OnUpdate()
    {
        if (Target.Value == null)
        {
            if (m_actor.Targets.Count == 0)
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Running;
            }
        }
        Actor actor = Target.Value.GetComponent<Actor>();
        SendMessageToOtherMonsters(actor);
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
    }

    /// <summary>
    /// 주변에 있는 몬스터를 찾고, 대상을 타겟으로 지정하게 합니다.
    /// </summary>
    /// <param name="target">타겟으로 지정할 대상입니다.</param>
    /// <param name="radius">주변 몬스터를 찾는데 사용할 거리입니다.</param>
    /// <param name="clearExistingTargets">기존 타겟을 제거할지 여부를 나타내는 값입니다.</param>
    private void SendMessageToOtherMonsters(Actor target, float radius = Mathf.Infinity, bool clearExistingTargets = true)
    {
        var monstersInRadius = Physics.OverlapSphere(transform.position, radius, Recipient.Value)
            .Select(col => col.GetComponent<Monster>())
            .Where(monster => monster != null);
        
        foreach (var monster in monstersInRadius)
        {
            if (clearExistingTargets)
            {
                monster.Targets.Clear();
            }

            if (monster == target || monster.Targets.Contains(target))
            {
                continue;
            }

            // 만약 벽이 두 몬스터 사이에 있다면, 타겟으로 지정하지 않습니다.
            Vector3 targetPosition = target.transform.position;
            Vector3 recipientPosition = monster.transform.position;
            Vector3 direction = (targetPosition - recipientPosition).normalized;
            float distance = Vector3.Distance(targetPosition, recipientPosition);
            if (Physics.Raycast(recipientPosition, direction, distance, LayerMask.GetMask("Wall")))
            {
                continue;
            }

            monster.Targets.Add(target);
        }
    }
}
