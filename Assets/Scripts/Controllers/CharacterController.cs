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
    
    private PossessionProcessor m_possession;

    [SerializeField]
    [NotifyFieldChanged(nameof(OnCharacterChanged))]
    // 인스펙터 확인용 필드
    private Actor m_currentActor;
    
    private Actor m_character;

    private bool m_isOnPossessing;

    // Move //
    private Vector3 m_directionInput;

    // Look //
    private Vector2 m_lookDelta;
    
    private Transform m_cameraHolder;

    private float m_cameraRotationX;
    
    private void Awake()
    {
        if (m_data == null) 
        {
            m_data = ManagerRoot.Resource.Load<CharacterControlData>($"data/{nameof(CharacterControlData)}");
        }
        
        m_possession = GetComponent<PossessionProcessor>();
        m_possession.Possessing += OnPossessing;
        m_possession.Possessed += OnPossessed;
    }

    private void Start()
    {
        if (m_currentActor != null)
        {
            OnCharacterChanged();
        }
        
        InitInput();
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
        
        m_isOnPossessing = true;
        
        m_possession.TryPossess(m_character);
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
    }
    
    private void OnPossessed(object sender, Actor actor)
    {
        EnableInput();
        m_isOnPossessing = false;

        ChangeActor(actor);
    }

    private void OnCharacterChanged()
    {
        ChangeActor(m_currentActor);
        GameManager.Camera.CurrentCamera = m_currentActor.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void ChangeActor(Actor newActor)
    {
        if (newActor == null)
        {
            return;
        }
        
        m_cameraRotationX = 0f;
        if (m_character != null)
        {
            m_character.Unpossessed();
        }

        m_character = m_currentActor = newActor;
        m_character.Possessed();
        m_cameraHolder = m_character.FirstPersonCameraPivot.transform;
    }
}
