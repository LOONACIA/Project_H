using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsHit : Conditional
{
    private Monster m_owner;

    private ActorHealth m_health;

    private bool isHit;
    // Start is called before the first frame update
    public override void OnAwake()
    {
        m_owner = GetComponent<Monster>();
        m_health = GetComponent<ActorHealth>();
        m_health.Damaged += OnDamaged;
    }

    public override TaskStatus OnUpdate()
    {
        // 마지막 호출 이후 대미지를 입었고, 공격자가 빙의된 상태라면
        if (isHit)
        {
            isHit = false;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    private void OnDamaged(object sender, Actor e)
    {
        // 대미지를 입으면 Attacker에 대입
        isHit = true;
        Debug.Log(isHit);
    }
}
