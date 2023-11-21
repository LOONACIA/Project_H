using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ActorStatus : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    [Tooltip("Hp는 " + nameof(ActorHealth) + "에서 관리됨")]
    private int m_hp;

    [SerializeField]
    [ReadOnly]
    private int m_damage;

    [SerializeField]
    [ReadOnly]
    private bool m_isBlocking;
    
    [SerializeField]
    [ReadOnly]
    private float m_knockDownTime;
    
    [SerializeField]
    [ReadOnly]
    private bool m_isKnockDown;

    [SerializeField]
    [ReadOnly]
    private bool m_isKnockBack;
    
    

    private BehaviorTree m_behaviorTree;
    private SharedFloat m_aiKnockDownTime;
    
    public int Hp
    {
        get => m_hp;
        set => m_hp = value;
    }

    public int Damage
    {
        get => m_damage;
        set => m_damage = value;
    }

    public bool IsBlocking
    {
        get => m_isBlocking;
        set => m_isBlocking = value;
    }

    public bool IsKnockedDown => m_knockDownTime>0f;

    //주의: IsKnockBack값의 수정은 Monster류, Actor류 클래스에서만 일어나야함.
    public bool IsKnockBack
    {
        get => m_isKnockBack;
        set => m_isKnockBack = value;   
    }

    public float KnockDownTime
    {
        get => m_knockDownTime;
        private set
        {
            m_knockDownTime = value;
            
            //넉다운 타임이 변경될 경우, AI와 싱크를 맞춰줍니다.
            if (m_aiKnockDownTime != null)
            {
                m_aiKnockDownTime.SetValue(value);
            }
        }
    }

    public void SetKnockDown(float duration)
    {
        if (KnockDownTime > duration) return;
        else
        {
            KnockDownTime = duration;
        }
    }

    private void Awake()
    {
        m_behaviorTree = GetComponent<BehaviorTree>();
        if (m_behaviorTree)
        {
            SharedVariable holder = m_behaviorTree.GetVariable("KnockDownTime");
            m_aiKnockDownTime = holder as SharedFloat;
        }
    }

    private void Update()
    {
        UpdateKnockDownTime();
    }

    /// <summary>
    /// 남은 기절 시간을 실시간으로 업데이트합니다.
    /// </summary>
    private void UpdateKnockDownTime()
    {
        if (!IsKnockedDown)
        {
            KnockDownTime = 0f;
            return;
        }
        
        KnockDownTime -= Time.deltaTime;
        if (KnockDownTime < 0.0f)
        {
            KnockDownTime = 0f;
        }
    }
}
