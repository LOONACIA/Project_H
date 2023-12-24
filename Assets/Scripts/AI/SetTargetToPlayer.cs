using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class SetTargetToPlayer : Action
{
    [SerializeField]
    private SharedTransform m_target;
    
    private PlayerController m_controller;

    private Monster m_monster;
    
    public override void OnAwake()
    {
        m_controller = Object.FindObjectOfType<PlayerController>();
        m_monster = GetComponent<Monster>();
    }

    public override void OnStart()
    {
        m_monster.Targets.RemoveAll(target => !target.IsPossessed);
        Actor player = m_controller.Character;
        if (!m_monster.Targets.Contains(player))
        {
            m_monster.Targets.Add(player);
        }

        m_target.Value = player.transform;
    }
}
