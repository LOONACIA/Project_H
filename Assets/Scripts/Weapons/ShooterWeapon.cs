using Cinemachine;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ShooterWeapon : Weapon
{
    [Header("Common")]
    [SerializeField]
    [Tooltip("발사체의 생성 위치")]
    private Transform m_spawnPosition;

    [SerializeField]
    private LayerMask m_aimLayers;

    [SerializeField]
    [Layer]
    private int m_damageLayer;
    
    [Header("Shotgun")]
    [SerializeField]
    [Tooltip("발사체 프리팹")]
    private GameObject m_projectilePrefab;
    
    [SerializeField]
    private float m_shootForce;
    
    [SerializeField]
    [Tooltip("샷건의 탄 퍼짐 각도")]
    private float m_shotgunSpreadAngle = 10f;
    
    [SerializeField]
    [Tooltip("한 번에 발사할 탄환의 수")]
    private int m_shotgunBulletCount = 5;
    
    [FormerlySerializedAs("m_shotgunMaxDistance")]
    [SerializeField]
    [Tooltip("샷건의 사거리")]
    private float m_shotgunRange = 10f;

    [Header("Sniper")]
    [SerializeField]
    private float m_maxDistance = 100f;
    
    [SerializeField]
    [Tooltip("플레이어의 장탄 수")]
    private int m_playerMaxAmmo = 5;
    
    [SerializeField]
    private Color m_shootLineColor;

    [SerializeField]
    private Color m_laserColor;

    private Actor m_owner;

    private CinemachineVirtualCamera m_vcam;

    private Ray m_ray;

    private bool m_isSnipingMode = true;

    private LineRenderer m_renderer;
    
    private CoroutineEx m_drawLineCoroutine;

    private Vector3 m_target;

    private float m_intensity = 1f;

    private float m_intensityMul = 0.2f;

    public int Ammo => m_owner.IsPossessed && m_isSnipingMode ? m_playerMaxAmmo : 1;

    public override Vector3 Target
    {
        get => m_target;
        set
        {
            m_target = value;
            UpdateTarget();
        }
    }

    private void Awake()
    {
        m_owner = GetComponentInParent<Actor>();
        m_vcam = m_owner.GetComponentInChildren<CinemachineVirtualCamera>();
        m_renderer = GetComponent<LineRenderer>();
    }

    private void OnDisable()
    {
        m_drawLineCoroutine?.Abort();
    }

    private void OnDestroy()
    {
        m_drawLineCoroutine?.Abort();
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
    
    private static Vector3 GetRandomConeDirection(Vector3 coneDirection, float maxAngle)
    {
        float angle = Random.Range(-maxAngle, maxAngle);
        Vector3 randomDirection = Random.insideUnitSphere;
        return Vector3.RotateTowards(coneDirection, randomDirection, Mathf.Deg2Rad * angle, 0.0f);
    }

    private void Fire()
    {
        if (m_isSnipingMode)
        {
            Snipe();
        }
        else
        {
            ShootProjectile();
        }
    }

    private void SetRay()
    {
        Transform cameraTransform = m_vcam.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 dir = Target == default ? cameraTransform.forward : (Target - cameraTransform.position).normalized;

        m_ray = new(cameraPosition, dir);
    }

    private void Snipe()
    {
        if (m_owner.IsPossessed && m_playerMaxAmmo-- <= 0)
        {
            return;
        }
        
        SetRay();

        var hits = Physics.RaycastAll(m_ray, m_maxDistance, m_aimLayers)
            .OrderBy(hit => hit.distance)
            .ToArray();

        RaycastHit end = hits.FirstOrDefault(hit => hit.transform.gameObject.layer != m_damageLayer);

        var targets = hits.TakeWhile(hit => hit.transform.gameObject.layer == m_damageLayer);

        InvokeHitEvent(ProcessHit(targets));

        Vector3 target;
        if (hits.Length > 0)
        {
            target = end.point != default ? end.point : hits[^1].point;
        }
        else
        {
            target = m_ray.GetPoint(m_maxDistance);
        }
        DrawLine(target);
    }

    private void ShootProjectile()
    {
        SetRay();
        
        bool isHit = Physics.Raycast(m_ray, out var hit, m_maxDistance, m_aimLayers);
        Vector3 target = isHit ? hit.point : m_ray.GetPoint(m_maxDistance);
        Vector3 spawnPosition = m_spawnPosition.position;
        Vector3 direction = (target - spawnPosition).normalized;
        for (int index = 0; index < m_shotgunBulletCount; index++)
        {
            Vector3 coneDirection = GetRandomConeDirection(direction, m_shotgunSpreadAngle);
        
            var projectile =
                ManagerRoot.Resource.Instantiate(m_projectilePrefab, spawnPosition, m_spawnPosition.rotation)
                    .GetComponent<Projectile>();
        
            projectile.Init(m_owner.gameObject, m_shotgunRange, m_aimLayers, info => InvokeHitEvent(Enumerable.Repeat(info, 1)));
            projectile.Rigidbody.AddForce(coneDirection * m_shootForce, ForceMode.VelocityChange);
        }
    }

    private IEnumerable<WeaponAttackInfo> ProcessHit(IEnumerable<RaycastHit> hits)
    {
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out Actor actor))
            {
                yield return new(actor, m_ray.direction, hit.point);
            }
        }
    }

    private void UpdateTarget()
    {
        if (m_owner.IsPossessed)
        {
            return;
        }
        
        UpdateAnimator();
        UpdateLine();
    }

    private void UpdateAnimator()
    {
        Vector3 from = Vector3.down;
        Vector3 to = (Target - m_spawnPosition.position).normalized;
        float angle = Vector3.Angle(from, to) / 180f;
        angle = Mathf.Clamp(angle, 0.1f, 0.9f);

        // 타겟과 거리가 너무 가까운 경우에는 정면을 보도록 설정
        if (Vector3.Distance(Target, m_spawnPosition.position) < 2f)
            angle = 0.5f;

        m_owner.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_AIM_ANGLE, angle);
    }

    private void UpdateLine()
    {
        if (Target == default)
        {
            m_renderer.positionCount = 0;
            return;
        }
        
        Vector3 direction = (Target - m_vcam.transform.position).normalized;
        Vector3 end = Target;
        if (Physics.Raycast(m_vcam.transform.position, direction, out var hit, m_maxDistance, m_aimLayers))
        {
            end = hit.point;
        }
        
        m_renderer.positionCount = 2;
        m_renderer.SetPosition(0, m_spawnPosition.position);
        m_renderer.SetPosition(1, end);
        m_renderer.startWidth = m_renderer.endWidth = 0.005f;

        m_intensity += m_intensityMul;
        m_renderer.material.SetColor("_EmissionColor", m_laserColor * m_intensity);
    }

    private void DrawLine(Vector3 target)
    {
        m_renderer.positionCount = 2;
        m_renderer.SetPosition(0, m_spawnPosition.position);
        m_renderer.SetPosition(1, target);

        //Color c = m_renderer.material.GetColor("_EmissionColor");
        
        m_renderer.material.SetColor("_EmissionColor", m_shootLineColor * 1500f);

        m_drawLineCoroutine = Utility.Lerp(0.2f, 0f, 0.5f, value => m_renderer.startWidth = m_renderer.endWidth = value, () => m_intensity = 1f);
    }
}