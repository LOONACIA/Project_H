using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HackingConsole : HackingObject
{
    [SerializeField]
    private Material m_idleMaterial;

    [SerializeField]
    private Material m_HackingMaterial;

    [SerializeField]
    private float m_hackingCoolTime = 5f;

    [SerializeField]
    private float m_hackingProgressedTime = 0.5f;

    private IHackable[] m_hackable;

    private Renderer m_renderer;

    private Coroutine m_coroutine;

    private bool m_isHacking;

    private void Start()
    {
        m_hackable = GetComponentsInChildren<IHackable>();

        m_renderer = GetComponent<Renderer>();
        m_idleMaterial = Instantiate(m_renderer.material);
        m_HackingMaterial = Instantiate(m_HackingMaterial);
    }

    public override void Interact()
    {
        if (m_isHacking) return;

        m_isHacking = true;

        ConvertMaterial(m_idleMaterial, m_HackingMaterial);

        foreach (var hackable in m_hackable)
        {
            hackable.Hacking();
        }

        Invoke(nameof(Recovery), m_hackingCoolTime);
    }

    private void Recovery()
    {
        m_isHacking = false;

        ConvertMaterial(m_HackingMaterial, m_idleMaterial);

        foreach (var hackable in m_hackable)
        {
            hackable.Recovery();
        }
    }

    private void ConvertMaterial(Material from, Material to)
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(IE_ConvertMaterial(from, to));
    }

    private IEnumerator IE_ConvertMaterial(Material from, Material to)
    {
        float time = 0;

        while (time < m_hackingProgressedTime) 
        {
            time += Time.deltaTime;

            m_renderer.material.Lerp(from, to, time / m_hackingProgressedTime);

            yield return null;
        }

        m_renderer.material = to;
    }
}
