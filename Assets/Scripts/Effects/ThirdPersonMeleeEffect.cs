using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// 3인칭 공격자의 무기 이펙트에만 붙이는 컴포넌트입니다.
/// 공격 시 이펙트가 발생합니다.
/// </summary>
public class ThirdPersonMeleeEffect : MonoBehaviour
{
    private VisualEffect m_vfx;
    private TrailCaster m_trailCaster;

    private void Awake()
    {
        m_trailCaster = GetComponentInParent<TrailCaster>();
        m_vfx = GetComponentInChildren<VisualEffect>();
    }

    private void Start()
    {
        if (m_trailCaster != null)
        {
            m_trailCaster.CheckStarted += OnAttackStarted;
            m_trailCaster.CheckFinished += OnAttackFinished;
        }
    }

    private void OnDestroy()
    {
        if (m_trailCaster != null)
        {
            m_trailCaster.CheckStarted -= OnAttackStarted;
            m_trailCaster.CheckFinished -= OnAttackFinished;
        }
    }


    private void OnAttackStarted(object sender, EventArgs e)
    {
        if (m_vfx != null)
        {
            m_vfx.SendEvent("OnPlay");
            m_vfx.SetBool("IsAttacking", true);
        }
    }

    private void OnAttackFinished(object sender, EventArgs e)
    {
        if (m_vfx != null)
        {
            m_vfx.SendEvent("OnEnd");
            m_vfx.SetBool("IsAttacking", false);
        }
    }
}
