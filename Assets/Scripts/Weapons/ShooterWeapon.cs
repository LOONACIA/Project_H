using Cinemachine;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
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
    private float m_maxDistance = 100f;

    private CinemachineVirtualCamera m_vcam;

    private Ray m_ray;

    private RaycastHit m_hit;

    private bool m_isHitDetected;

    private bool m_isAiming;

    private void Awake()
    {
        m_vcam = transform.root.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void FixedUpdate()
    {
        // 별도 조준 기능을 넣으려는 경우 버튼 입력에 따라 m_isAiming 변수의 값을 바꾸면 됨
        //if (m_isAiming)
        {
            Aim();
        }
    }

    private void Aim()
    {
        Transform cameraTransform = m_vcam.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;

        m_ray = new(cameraPosition, dir);
        m_isHitDetected = Physics.Raycast(m_ray, out m_hit, m_maxDistance, m_aimLayers);
    }

    private void Fire()
    {
        var spawnPosition = m_spawnPosition.position;
        Vector3 target = m_isHitDetected ? m_hit.point : m_ray.GetPoint(300f);
        Vector3 direction = (target - spawnPosition).normalized;

        Debug.DrawRay(m_spawnPosition.position, direction * m_maxDistance, Color.red, 0.1f);
        return;
        var projectile =
            ManagerRoot.Resource.Instantiate(m_projectilePrefab, spawnPosition, m_spawnPosition.rotation)
                .GetComponent<Projectile>();
        //projectile.Init(Owner.gameObject, info => InvokeHitEvent(new WeaponAttackInfo[1]{info}));
        if (projectile.TryGetComponent<Rigidbody>(out var projectileRigidbody))
        {
            projectileRigidbody.AddForce(direction * m_shootForce, ForceMode.VelocityChange);
        }
    }

    protected override void Attack()
    {
        //Animator.SetTrigger(MonsterAttack.s_attackAnimationKey);
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
}