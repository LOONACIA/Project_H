using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IHealth
{
    [SerializeField]
    private Renderer m_bossDefaultRenderer;

    private int m_currentHP = 1;

    private int m_maxHp = 1;

    public int CurrentHp => m_currentHP;

    public int MaxHp => m_maxHp;

    public event RefEventHandler<AttackInfo> Damaged;
    public event RefEventHandler<AttackInfo> Dying;
    public event EventHandler Died;

    private void Start()
    {
        if (m_bossDefaultRenderer != null)
        { 
            m_bossDefaultRenderer.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (m_bossDefaultRenderer != null)
        {
            m_bossDefaultRenderer.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (m_bossDefaultRenderer != null)
        {
            m_bossDefaultRenderer.enabled = false;
        }
    }

    // 보스 강제 사망 처리
    public void Kill()
    {
        Dying?.Invoke(this,(new(null, this, MaxHp, Vector3.zero, Vector3.zero)));
    }

    public void TakeDamage(in AttackInfo damageInfo)
    {
    }
}
