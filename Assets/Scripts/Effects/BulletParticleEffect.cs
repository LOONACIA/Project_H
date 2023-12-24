using LOONACIA.Unity.Coroutines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class BulletParticleEffect : MonoBehaviour
{
    private VisualEffect vfx;

    private bool m_initialized = false;
    private Transform m_oldParent = null;
    private bool m_visualized = false;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        vfx = GetComponent<VisualEffect>();
        //부모가 사라져도 일정시간 Trail을 남기기 위해, 부모를 분리합니다.
        m_oldParent = transform.parent;
        transform.parent = null;

        m_initialized = true;
        vfx.SendEvent("OnPlay");
        vfx.SetBool("TrailAlive", true);
    }

    private void Update()
    {
        if (!m_initialized)
        {
            return;
        }

        if (!m_oldParent)
        {
            CoroutineEx.Create(this, IE_End());
        }


        if (m_oldParent.gameObject.activeSelf)
        {
            if (!m_visualized)
            {
                m_visualized = true;
                vfx.SendEvent("OnPlay");
                vfx.SetBool("TrailAlive", true);
            }
            transform.position = m_oldParent.position;
        }
        else
        {
            if (m_visualized)
            {
                vfx.SendEvent("OnEnd");
                vfx.SetBool("TrailAlive", false);
                m_visualized = false;
            }
        }
    }

    private IEnumerator IE_End()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
