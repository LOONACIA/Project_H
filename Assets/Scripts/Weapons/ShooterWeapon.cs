using Cinemachine;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShooterWeapon : Weapon
{
    [SerializeField]
    [Tooltip("발사체 프리팹")]
    private GameObject m_projectilePrefab;

    [SerializeField]
    [Tooltip("발사체의 생성 위치")]
    private Transform m_spawnPosition;

    [SerializeField]
    private float m_shootForce;

    [SerializeField]
    private LayerMask m_aimLayers;

    [SerializeField]
    private int m_damage = 5;

    [SerializeField]
    private float m_maxDistance = 100f;

    private CinemachineVirtualCamera m_vcam;

    private Ray m_ray;

    private bool m_isSnipingMode = true;

    // TODO: Remove test code
    private LineRenderer m_renderer;

    private void Awake()
    {
        m_vcam = transform.root.GetComponentInChildren<CinemachineVirtualCamera>();
        m_renderer = GetComponent<LineRenderer>();
    }

    public void ChangeMode()
    {
        m_isSnipingMode = !m_isSnipingMode;
    }
    
    protected override void Attack()
    {
        Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
    }

    protected override void OnLeadInMotion()
    {
        Debug.Log($"사격 모션 시작(선딜), State: {State}");
    }

    protected override void OnHitMotion()
    {
        Debug.Log($"사격, State: {State}");
        Fire();
    }

    protected override void OnFollowThroughMotion()
    {
        Debug.Log($"사격 모션 종료(후딜), State: {State}");
    }
    
    private void Fire()
    {
        if (m_isSnipingMode)
        {
            Snipe();
        }
        else
        {
            // TODO: 샷건 모드
        }
    }
    
    private void Snipe()
    {
        Transform cameraTransform = m_vcam.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;

        m_ray = new(cameraPosition, dir);
        RaycastHit[] hits = Physics.RaycastAll(m_ray, m_maxDistance, m_aimLayers);
        InvokeHitEvent(ProcessHit(hits));

        // TODO: Remove test code
        Vector3 target = hits.Length > 0 ? hits.Last().point : m_ray.GetPoint(m_maxDistance);
        DrawLine(target);
    }

    private IEnumerable<AttackInfo> ProcessHit(IEnumerable<RaycastHit> hits)
    {
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out IHealth health))
            {
                // TODO: AttackInfo 변경 시 수정 필요
                yield return new(m_damage, hit.normal, Owner, health);
            }
        }
    }

    // TODO: Remove test code
    private void DrawLine(Vector3 target)
    {
        m_renderer.SetPosition(0, m_spawnPosition.position);
        m_renderer.SetPosition(1, target);

        Utility.Lerp(0.1f, 0f, 0.5f, value => m_renderer.startWidth = m_renderer.endWidth = value);
    }
}