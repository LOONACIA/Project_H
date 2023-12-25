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

        if(m_oldParent != null)
        {
            //이전에 초기화된 적이 있음. return;
            return;
        }

        if(transform.parent == null)
        {
            Debug.LogError("샷건 이펙트의 부모 총알이 존재하지 않음.");
        }

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
        if(m_oldParent == null)
        {
            //부모가 없으면 이펙트 발동 x
            return;
        }

        if (!m_oldParent)
        {
            //부모가 Destroy되었다면, 1초 뒤 삭제하는 절차를 밟음
            CoroutineEx.Create(this, IE_End());
            m_oldParent = null;
            return;
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
                m_visualized = false;
                vfx.SendEvent("OnEnd");
                vfx.SetBool("TrailAlive", false);
            }
        }
    }

    private IEnumerator IE_End()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
