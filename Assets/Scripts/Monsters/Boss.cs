using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IHealth
{
    private int m_currentHP = 1;

    private int m_maxHp = 1;

    public int CurrentHp => m_currentHP;

    public int MaxHp => m_maxHp;

    public event RefEventHandler<AttackInfo> Damaged { add { } remove { } }
    public event RefEventHandler<AttackInfo> Dying;
    public event EventHandler Died { add { } remove { } }

    // 보스 강제 사망 처리
    public void Kill()
    {
        Dying?.Invoke(this,(new(null, this, MaxHp, Vector3.zero, Vector3.zero)));
    }

    public void TakeDamage(in AttackInfo damageInfo)
    {
    }
}
