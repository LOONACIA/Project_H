using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BloodEffect : MonoBehaviour
{
    private VisualEffect m_effect;

    private float m_duration;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    public void Show(Vector3 position, float duration)
    {
        Init();
        m_effect.SetVector3("BloodBoxCenter", position);
        m_duration = duration;
        
        StartCoroutine(CoShowEffect());
    }

    private void Init()
    {
        if (m_effect == null)
        {
            m_effect = GetComponent<VisualEffect>();
        }
    }
    
    private IEnumerator CoShowEffect()
    {
        // duration 동안 이펙트 지속
        yield return new WaitForSeconds(m_duration);
        if (m_effect == null)
        {
            yield break;
        }

        // 이펙트 종료
        m_effect.Stop();

        // 남은 파티클이 없어질 때까지 대기 
        while (m_effect != null && m_effect.aliveParticleCount != 0)
        {
            yield return null;
        }

        // 이펙트 오브젝트 제거
        if (m_effect != null)
        {
            ManagerRoot.Resource.Release(gameObject);
        }
    }
}