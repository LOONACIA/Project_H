using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HackingConsole : HackingObject
{
    [SerializeField, Tooltip("해킹 전의 원래 메테리얼")]
    private Material m_idleMaterial;

    [SerializeField, Tooltip("해킹 후의 변경되는 메테리얼")]
    private Material m_HackingMaterial;

    [SerializeField, Tooltip("해킹이 지속되는 시간")]
    private float m_hackingCoolTime = 5f;

    [SerializeField, Tooltip("해킹에 걸리는 시간 ")]
    private float m_hackingProgressedTime = 0.5f;

    [SerializeField, Tooltip("해킹 가능한 객체들")]
    private IHackable[] m_hackable;

    private Renderer m_renderer;

    private Coroutine m_coolTimeCoroutine;

    private Coroutine m_materialCoroutine;

    [Tooltip("해킹당한 상태인지 여부")]
    private bool m_isHacking;

    [Tooltip("해킹될 때 발생하는 이벤트")]
    public event EventHandler OnHacking;

    private void Start()
    {
        m_hackable = GetComponentsInChildren<IHackable>();

        m_renderer = GetComponent<Renderer>();
        m_idleMaterial = Instantiate(m_renderer.material);
        m_HackingMaterial = Instantiate(m_HackingMaterial);
    }

    public override void Interact()
    {
        //if (m_isHacking) return;

        OnHacking?.Invoke(this, EventArgs.Empty);

        Hacking();
    }

    public void Hacking()
    {
        if (m_isHacking)
        {
            if (m_materialCoroutine != null)
                StopCoroutine(m_coolTimeCoroutine);

            m_coolTimeCoroutine = StartCoroutine(IE_WaitCoolTime());
            return;
        }

        m_isHacking = true;

        ConvertMaterial(m_idleMaterial, m_HackingMaterial);

        foreach (var hackable in m_hackable)
        {
            hackable.Hacking();
        }

        m_coolTimeCoroutine = StartCoroutine(IE_WaitCoolTime());
        //Invoke(nameof(Recovery), m_hackingCoolTime);
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
        if (m_materialCoroutine != null)
            StopCoroutine(m_materialCoroutine);

        m_materialCoroutine = StartCoroutine(IE_ConvertMaterial(from, to));
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

    private IEnumerator IE_WaitCoolTime()
    {
        float time = 0;

        while (time < m_hackingCoolTime)
        {
            time += Time.deltaTime;

            yield return null;
        }

        Recovery();
    }
}
