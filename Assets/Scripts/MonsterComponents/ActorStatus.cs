using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity;
using UnityEngine;

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
}
