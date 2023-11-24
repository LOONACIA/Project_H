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
    [Layer]
    private int m_damageLayer;

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
    }

    protected override void OnLeadInMotion()
    {
    }

    protected override void OnHitMotion()
    {
        Fire();
    }

    protected override void OnFollowThroughMotion()
    {
    }

    private void Fire()
    {
        // if (m_isSnipingMode)
        // {
        //     Snipe();
        // }
        // else
        {
            ShootProjectile();
        }
    }

    private void Aim()
    {
        Transform cameraTransform = m_vcam.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;

        m_ray = new(cameraPosition, dir);
    }

    private void Snipe()
    {
        Aim();

        var hits = Physics.RaycastAll(m_ray, m_maxDistance, m_aimLayers)
            .OrderBy(hit => hit.distance)
            .TakeWhile(hit => hit.transform.gameObject.layer == m_damageLayer)
            .ToArray();

        InvokeHitEvent(ProcessHit(hits));

        // TODO: Remove test code
        Vector3 target = hits.Length > 0 ? hits.Last().point : m_ray.GetPoint(m_maxDistance);
        DrawLine(target);
    }

    private void ShootProjectile()
    {
        Aim();

        bool isHit = Physics.Raycast(m_ray, out var hit, m_maxDistance, m_aimLayers);
        Vector3 target = isHit ? hit.point : m_ray.GetPoint(m_maxDistance);

        Vector3 spawnPosition = m_spawnPosition.position;
        Vector3 direction = (target - spawnPosition).normalized;

        var projectile =
            ManagerRoot.Resource.Instantiate(m_projectilePrefab, spawnPosition, m_spawnPosition.rotation)
                .GetComponent<Projectile>();
        
        projectile.Init(transform.root.gameObject, info => InvokeHitEvent(Enumerable.Repeat(info, 1)));
        projectile.Rigidbody.AddForce(direction * m_shootForce, ForceMode.VelocityChange);
    }

    private IEnumerable<WeaponAttackInfo> ProcessHit(IEnumerable<RaycastHit> hits)
    {
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out Actor actor))
            {
                // TODO: AttackInfo 변경 시 수정 필요
                yield return new(actor, hit.normal, hit.point);
            }
        }
    }

    // TODO: Remove test code
    private void DrawLine(Vector3 target)
    {
        m_renderer.positionCount = 2;
        m_renderer.SetPosition(0, m_spawnPosition.position);
        m_renderer.SetPosition(1, target);

        Utility.Lerp(0.1f, 0f, 0.5f, value => m_renderer.startWidth = m_renderer.endWidth = value);
    }
}