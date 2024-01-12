using System;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class BulletParticleEffect : MonoBehaviour
{
    private VisualEffect m_vfx;
    private Transform m_parent = null;
    private Projectile m_projectile;

    private bool m_parentEnabled = false;
    private float m_disableDelay = 0f;

    private float m_lastDisabledTime = 0f;

    public void OnBulletEnabled(object o, EventArgs arg)
    {
        //부모 재등록
        transform.parent = m_parent;
        m_parentEnabled = true;

        //모든 파티클 초기화 및 트레일 생성 시작
        m_vfx.Reinit();
        m_vfx.SendEvent("OnPlay");
    }

    public void OnBulletDisabled(object o, EventArgs arg)
    {
        //부모로부터 detach(트레일이 갑자기 한번에 사라지면 어색하므로)
        transform.parent = null;
        m_parentEnabled = false;
        m_lastDisabledTime = Time.time;

        //트레일 생성 종료
        m_vfx.SendEvent("OnEnd");
    }

    private void Awake()
    {
        m_parent = transform.parent;
        m_vfx = GetComponent<VisualEffect>();
        m_disableDelay = m_vfx.GetFloat("TrailLifeTime") + 0.01f;
        if (m_parent != null)
        {
            m_projectile = transform.parent.GetComponent<Projectile>();
            m_projectile.OnEnableComponents += OnBulletEnabled;
            m_projectile.OnDisableComponents += OnBulletDisabled;
        }
        else
        {
            Debug.LogError("BulletParticleEffect: 부모 총알이 없음");
        }
    }

    private void OnDestroy()
    {
        if (m_projectile != null)
        {
            m_projectile.OnEnableComponents -= OnBulletEnabled;
            m_projectile.OnDisableComponents -= OnBulletDisabled;
        }
    }

    private void Update()
    {
        if(!m_parentEnabled&&m_lastDisabledTime + m_disableDelay < Time.time)
        {
            //부모가 Disable되었고, 딜레이가 지났다면 부모를 원상복귀한다(Pool로 들어간다)
            transform.parent = m_parent;
        }
    }
}
