using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(VolumetricLineBehavior))]
public class Laser : MonoBehaviour, IHackable
{
    [SerializeField]
    private float m_progressTime = 0.5f;

    private CapsuleCollider m_collider;

    private VolumetricLineBehavior m_volumetricLineBehavior;

    private float m_originWidth;

    private bool m_isHacking;

    private void Start()
    {
        m_collider = GetComponent<CapsuleCollider>();
        m_volumetricLineBehavior = GetComponent<VolumetricLineBehavior>();

        m_collider.center = (m_volumetricLineBehavior.StartPos + m_volumetricLineBehavior.EndPos) / 2;
        m_collider.height = MathF.Abs(m_volumetricLineBehavior.StartPos.y - m_volumetricLineBehavior.EndPos.y);

        m_originWidth = m_volumetricLineBehavior.LineWidth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
        {
            actor.Health.Kill();
        }
    }


    public void Hacking()
    {
        if (m_isHacking) return;

        m_isHacking = true;
        m_collider.enabled = false;

        StartCoroutine(IE_ChangeWidth(m_originWidth, 0));

    }

    public void Recovery()
    {
        m_isHacking = false;
        m_collider.enabled = true;

        StartCoroutine(IE_ChangeWidth(0, m_originWidth));
    }

    private IEnumerator IE_ChangeWidth(float from, float to)
    {
        float time = 0;

        while (time < m_progressTime)
        {
            time += Time.deltaTime;

            m_volumetricLineBehavior.LineWidth = Mathf.Lerp(from, to, time / m_progressTime);

            yield return null;
        }
    }
}