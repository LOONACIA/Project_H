using System;
using Cinemachine;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using UnityEngine;

[RequireComponent(typeof(PossessionProcessor))]
public partial class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterControlData m_data;
    
    [SerializeField]
    [NotifyFieldChanged(nameof(OnCharacterChanged))]
    private Actor m_character;

    private PossessionProcessor m_possession;

    private bool m_isOnPossessing;

    // Move //
    private Vector3 m_directionInput;

    // Look //
    private Vector2 m_lookDelta;

    private Transform m_cameraHolder;

    private float m_cameraRotationX;

    private bool m_isGameOver;
    
    public Actor Character => m_character;

    public event EventHandler<DamageInfo> Damaged;

    public event EventHandler<int> HpChanged;

    public event EventHandler ShieldChanged;

    private void Awake()
    {
        if (m_data == null)
        {
            m_data = ManagerRoot.Resource.Load<CharacterControlData>($"data/{nameof(CharacterControlData)}");
        }

        m_possession = GetComponent<PossessionProcessor>();
        m_possession.Possessing += OnPossessing;
        m_possession.Possessed += OnPossessed;

        GameManager.UI.ShowCrosshair();
        GameManager.UI.ShowHpIndicator(this);
        GameManager.UI.GenerateShieldIndicator(this);
        GameManager.UI.ShowShurikenIndicator(m_possession);
        GameManager.UI.ShowDamageIndicator();
    }

    private void Start()
    {
        if (m_character != null)
        {
            OnCharacterChanged(null, m_character);
        }

        InitInput();
    }

    private void OnEnable()
    {
        RegisterActorEvents();
    }

    private void OnDisable()
    {
        UnregisterActorEvents();
    }

    private void Update()
    {
        if (m_isGameOver)
        {
            return;
        }
        
#if UNITY_EDITOR
        Look();
#endif
    }

    private void FixedUpdate()
    {
        if (m_isGameOver)
        {
            return;
        }
        
#if !UNITY_EDITOR
        Look();
#endif
        Move();
    }

    private void Move()
    {
        if (m_character != null)
        {
            m_character.Move(m_directionInput);
        }
    }

    private void Jump()
    {
        if (m_character != null)
        {
            m_character.TryJump();
        }
    }

    private void Attack()
    {
        if (m_character != null)
        {
            m_character.TryAttack();
        }
    }

    private void Dash()
    {
        if (m_character != null)
        {
            m_character.Dash();
        }
    }

    private void Skill()
    {
        if (m_character != null)
        {
            m_character.Skill();
        }
    }

    private void Possess()
    {
        if (m_isOnPossessing)
        {
            return;
        }

        m_possession.TryPossess(m_character);
    }

    private void Block(bool value)
    {
        if (m_character != null)
        {
            m_character.Block(value);
        }
    }

    private void Look()
    {
        float xDelta = m_lookDelta.x * m_data.CameraHorizontalSensitivity * Time.fixedUnscaledDeltaTime;
        float yDelta = m_lookDelta.y * m_data.CameraVerticalSensitivity * Time.fixedUnscaledDeltaTime;

        float z = m_cameraHolder.eulerAngles.z;

        //Vertical Look
        m_cameraRotationX = Mathf.Clamp(m_cameraRotationX - yDelta, m_data.MinCameraVerticalAngle,
            m_data.MaxCameraVerticalAngle);
        m_cameraHolder.localRotation = Quaternion.Euler(m_cameraRotationX, 0, 0);

        Vector3 eulerAngles = m_cameraHolder.eulerAngles;
        eulerAngles.z = z;
        m_cameraHolder.eulerAngles = eulerAngles;

        // Rotate Character by Horizontal Look
        m_character.transform.Rotate(Vector3.up * xDelta);
    }

    private void OnPossessing(object sender, EventArgs e)
    {
        DisableInput();
        m_isOnPossessing = true;
    }

    private void OnPossessed(object sender, Actor actor)
    {
        EnableInput();
        m_isOnPossessing = false;

        ChangeActor(actor);
    }

    private void RegisterActorEvents()
    {
        if (m_character == null)
        {
            return;
        }
        
        m_character.Dying += OnPlayerCharacterDying;
        
        if (m_character.Status != null)
        {
            m_character.Status.HpChanged += OnHpChanged;
            m_character.Health.Damaged += OnDamaged;
            m_character.Status.ShieldChanged += OnShieldChanged;
        }
    }

    private void UnregisterActorEvents()
    {
        if (m_character == null)
        {
            return;
        }
        
        m_character.Dying -= OnPlayerCharacterDying;
        
        if (m_character.Status != null)
        {
            m_character.Status.HpChanged -= OnHpChanged;
            m_character.Status.ShieldChanged -= OnShieldChanged;

            m_character.Health.Damaged -= OnDamaged;
        }
    }
    
    private void OnPlayerCharacterDying(object sender, EventArgs e)
    {
        // TODO: 빙의 중 죽은 경우 어떻게 할 것인지 논의 필요
        if (m_isOnPossessing)
        {
            return;
        }
        
        GameManager.Instance.SetGameOver();
        m_isGameOver = GameManager.Instance.IsGameOver;
    }

    private void OnDamaged(object sender, DamageInfo e)
    {
        Damaged?.Invoke(this, e);
    }

    private void OnHpChanged(object sender, int e)
    {
        HpChanged?.Invoke(this, e);
    }
    
    private void OnShieldChanged(object sender, EventArgs e)
    {
        ShieldChanged?.Invoke(this, e);
    }

    /// <summary>
    /// 인스펙터에서 캐릭터를 변경할 때 호출되는 콜백
    /// </summary>
    /// <param name="oldActor">이전 값</param>
    /// <param name="newActor">최신 값</param>
    private void OnCharacterChanged(Actor oldActor, Actor newActor)
    {
        if (!Application.isPlaying)
        {
            if (oldActor != null)
            {
                oldActor.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 1;
            }
            
            newActor.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 2;
            return;
        }
        
        ChangeActor(oldActor, newActor);

        GameManager.Camera.CurrentCamera = m_character.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void ChangeActor(Actor newActor) => ChangeActor(m_character, newActor);

    private void ChangeActor(Actor oldActor, Actor newActor)
    {
        if (newActor == null)
        {
            return;
        }

        m_cameraRotationX = 0f;
        UnregisterActorEvents();
        if (oldActor != null)
        {
            oldActor.IsPossessed = false;
        }

        m_character = newActor;
        RegisterActorEvents();
        m_character.IsPossessed = true;
        m_cameraHolder = m_character.FirstPersonCameraPivot.transform;
        
        HpChanged?.Invoke(this, Character.Health.CurrentHp);
        ShieldChanged?.Invoke(this, EventArgs.Empty);
    }
}