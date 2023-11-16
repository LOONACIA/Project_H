using System;
using Cinemachine;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using UnityEngine;

[RequireComponent(typeof(PossessionProcessor))]
public partial class CharacterController : MonoBehaviour
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
    
    public Actor Character => m_character;

    public event EventHandler<int> HpChanged;

    private void Awake()
    {
        if (m_data == null)
        {
            m_data = ManagerRoot.Resource.Load<CharacterControlData>($"data/{nameof(CharacterControlData)}");
        }

        m_possession = GetComponent<PossessionProcessor>();
        m_possession.Possessing += OnPossessing;
        m_possession.Possessed += OnPossessed;
        
        GameManager.UI.ShowHpIndicator(this);
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
    }

    private void OnDisable()
    {
    }

    private void Update()
    {
        Look();
    }

    private void FixedUpdate()
    {
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

        //m_isOnPossessing = true;

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
        float xDelta = m_lookDelta.x * m_data.CameraHorizontalSensitivity * Time.unscaledDeltaTime;
        float yDelta = m_lookDelta.y * m_data.CameraVerticalSensitivity * Time.unscaledDeltaTime;

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
        if (m_character != null)
        {
            m_character.Health.Damaged += OnDamaged;
        }
    }
    
    private void UnregisterActorEvents()
    {
        if (m_character != null)
        {
            m_character.Health.Damaged -= OnDamaged;
        }
    }

    private void OnDamaged(object sender, Actor e)
    {
        HpChanged?.Invoke(this, Character.Health.CurrentHp);
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
        if (oldActor != null)
        {
            oldActor.Unpossessed();
        }

        m_character = newActor;
        m_character.Possessed();
        m_cameraHolder = m_character.FirstPersonCameraPivot.transform;
        
        HpChanged?.Invoke(this, Character.Health.CurrentHp);
    }
}