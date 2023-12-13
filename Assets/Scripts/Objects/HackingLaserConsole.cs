using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HackingLaserConsole : HackingObject
{
    [SerializeField]
    private Material m_idleMaterial;

    [SerializeField]
    private Material m_HackingMaterial;

    [SerializeField]
    private float m_hackingCoolTime = 5f;

    private Laser[] m_lasers;

    private void Start()
    {
        m_lasers = GetComponentsInChildren<Laser>();
    }

    public override void Interact()
    {
        foreach (var laser in m_lasers)
        { 
            laser.Hacking();
        }

        Invoke(nameof(Recovery), m_hackingCoolTime);
    }

    private void Recovery()
    {
        foreach (var laser in m_lasers)
        {
            laser.Recovery();
        }
    }
}
