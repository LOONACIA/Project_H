using Cinemachine;
using LOONACIA.Unity;
using UnityEngine;

public abstract class Gun : Weapon
{
    [Header("Common")]
    [SerializeField]
    [Tooltip("발사체의 생성 위치")]
    protected Transform m_spawnPosition;
    
    [SerializeField]
    protected LayerMask m_aimLayers;

    [SerializeField]
    [Layer]
    protected int m_damageLayer;

    [SerializeField]
    [Tooltip("장탄 수. -1이면 무한")]
    protected int m_maxAmmo = 5;
    
    protected CinemachineVirtualCamera m_virtualCamera;

    [SerializeField]
    private int m_ammo;

    private Ray m_ray;

    private Vector3 m_target;

    public int Ammo
    {
        get => m_ammo;
        set => m_ammo = Mathf.Clamp(value, 0, Mathf.Max(-1, 0));
    }

    public override Vector3 Target
    {
        get => m_target;
        set
        {
            m_target = value;
            OnTargetChanged();
        }
    }

    public override bool CanAttack => m_ammo > 0 || m_maxAmmo == -1;

    protected override void Awake()
    {
        base.Awake();
        
        m_ammo = m_maxAmmo;
        m_virtualCamera = Owner.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    protected override void OnAttackState()
    {
        m_ammo--;
        Fire();
    }

    protected abstract void Fire();

    protected ref Ray GetRay()
    {
        Transform cameraTransform = m_virtualCamera.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 dir = Target == default ? cameraTransform.forward : (Target - cameraTransform.position).normalized;

        m_ray.origin = cameraPosition;
        m_ray.direction = dir;
        return ref m_ray;
    }
    
    protected virtual void OnTargetChanged()
    {
        if (Owner.IsPossessed)
        {
            return;
        }
        
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        Vector3 from = Vector3.down;
        Vector3 to = (Target - m_spawnPosition.position).normalized;
        float angle = Vector3.Angle(from, to) / 180f;
        angle = Mathf.Clamp(angle, 0.1f, 0.9f);

        // 타겟과 거리가 너무 가까운 경우에는 정면을 보도록 설정
        if (Vector3.Distance(Target, m_spawnPosition.position) < 2f)
        {
            angle = 0.5f;
        }

        Owner.Animator.SetFloat(ConstVariables.ANIMATOR_PARAMETER_AIM_ANGLE, angle);
    }
}