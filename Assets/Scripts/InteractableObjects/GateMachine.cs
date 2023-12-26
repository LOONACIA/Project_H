using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Alarm))]
public class GateMachine : InteractableObject, IHealth
{
    [SerializeField]
    private GameObject[] m_gate;

    [SerializeField]
    private bool m_alarmWhenOpen = true;

    [SerializeField]
    private bool m_useShield;

    [SerializeField]
    private Shield m_shield;

    [SerializeField]
    private VisualEffect hitVfx;

    private List<IGate> m_gateScript = new();
    
    private Alarm m_alarm;

    public event RefEventHandler<AttackInfo> Damaged { add { } remove { } }
    public event RefEventHandler<AttackInfo> Dying { add { } remove { } }
    public event EventHandler Died { add { } remove { } }

    public int CurrentHp => 1;

    public int MaxHp => 1;

    private void Start()
    {
        foreach (var gate in m_gate)
        {
            if (gate.TryGetComponent<IGate>(out var gateScript))
                m_gateScript.Add(gateScript);
        }

        m_alarm = GetComponentInChildren<Alarm>();

        if (m_useShield == true)
        { 
            m_shield = GetComponentInChildren<Shield>();

            if (m_shield != null) 
            {
                IsInteractable = false;
            }
        }
    }

    public void Kill()
    {
    }

    public void TakeDamage(in AttackInfo damageInfo)
    {
        if (m_useShield && m_shield != null && m_shield.IsValid) 
        {
            m_shield?.TakeDamage(damageInfo.Damage);
            PlayHitVfx(damageInfo);

            if (!m_shield.IsValid)
            {
                IsInteractable = true;
            }
        }
    }

    protected override void OnInteractStart(Actor actor)
    {
        if (m_alarmWhenOpen)
        {
            m_alarm.Trigger(actor);
        }
    }

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        foreach (var gate in m_gateScript) 
        {
            StartCoroutine(gate.Open());
        }
    }

    private void PlayHitVfx(in AttackInfo damage)
    {
        if (hitVfx != null)
        {
            hitVfx.SetInt(ConstVariables.VFX_GRAPH_PARAMETER_PARTICLE_COUNT, damage.Damage * 2);
            hitVfx.SetVector3(ConstVariables.VFX_GRAPH_PARAMETER_DIRECTION, damage.AttackDirection);
            hitVfx.transform.position = damage.HitPoint;
            hitVfx.SendEvent(ConstVariables.VFX_GRAPH_EVENT_ON_PLAY);
        }
    }
}
