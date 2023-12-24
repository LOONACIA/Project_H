using LOONACIA.Unity;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(MonsterMovementData), menuName = "Data/" + nameof(MonsterMovementData))]
public class MonsterMovementData : ScriptableObject
{
    [Header("Monster Movement")]
    [SerializeField]
    [Tooltip("몬스터의 일반 이동 속도")]
    private float m_moveSpeed = 3f;

    [SerializeField]
    [Tooltip("몬스터 이동속도가 최고 속도가 되는 데 걸리는 시간")]
    private float m_accelerationTime = 0.3f;

    [SerializeField]
    [Tooltip("몬스터의 각속도")]
    private float m_angularSpeed = 300f;
    
    [Space]
    [Header("Player Movement")]
    [Header("Dash")]
    [SerializeField]
    private float m_dashMultiplier;

    [Header("Forward Movement")]
    [SerializeField]
    private float m_firstMoveSpeedThreshold;
    
    [SerializeField]
    private float m_secondMoveSpeedThreshold;
    
    [SerializeField]
    private float m_thirdMoveSpeedThreshold;
    
    [Header("Otherwise Movement")]
    [SerializeField]
    private float m_firstStrafeSpeedThreshold;

    [SerializeField]
    private float m_secondStrafeSpeedThreshold;
    
    [SerializeField]
    private float m_thirdStrafeSpeedThreshold;
    
    [Header("Acceleration")]
    [SerializeField]
    private float m_firstAccelerationTime;

    [SerializeField]
    private float m_secondAccelerationTime;
    
    [SerializeField]
    private float m_thirdAccelerationTime;

    [Space]
    [SerializeField]
    private float m_frictionAcceleration;

    [Header("Jump")]
    [SerializeField]
    private bool m_canJump = true;
    
    [SerializeField]
    private float m_jumpHeight;

    [SerializeField]
    private float m_zeroGravityDuration;
    
    [SerializeField]
    private float m_fallTime;

    [Header("Ground Check")]
    [SerializeField]
    private LayerMask m_whatIsGround;
    
    [FormerlySerializedAs("_groundGravity")]
    [Header("Gravity")]
    [SerializeField]
    private float _gravity;

    [Header("Dash")]
    [SerializeField]
    [ReadOnly]
    [Layer]
    private int m_dashLayer = 17;   //현재는 사용되지 않습니다.

    [SerializeField]
    private float m_dashAmount = 5f;

    [SerializeField]
    private float m_dashDuration = 0.15f;

    [SerializeField]
    private float m_dashCoolTime = 1.0f;

    [SerializeField]
    private float m_dashDelay = 0.4f;

    [SerializeField]
    private int m_maxDashCount = 2;

    public float MoveSpeed => m_moveSpeed;

    public float AccelerationTime => m_accelerationTime;

    public float AngularSpeed => m_angularSpeed;
    
    public float DashMultiplier => m_dashMultiplier;

    public float FirstMoveSpeedThreshold => m_firstMoveSpeedThreshold;

    public float SecondMoveSpeedThreshold => m_secondMoveSpeedThreshold;

    public float ThirdMoveSpeedThreshold => m_thirdMoveSpeedThreshold;

    public float FirstStrafeSpeedThreshold => m_firstStrafeSpeedThreshold;

    public float SecondStrafeSpeedThreshold => m_secondStrafeSpeedThreshold;

    public float ThirdStrafeSpeedThreshold => m_thirdStrafeSpeedThreshold;
    
    public float FirstAccelerationTime => m_firstAccelerationTime;

    public float SecondAccelerationTime => m_secondAccelerationTime;

    public float ThirdAccelerationTime => m_thirdAccelerationTime;

    public float FrictionAcceleration => m_frictionAcceleration;
    
    public bool CanJump => m_canJump;
    
    public float JumpHeight => m_jumpHeight;
    
    public float ZeroGravityDuration => m_zeroGravityDuration;
    
    public float FallTime => m_fallTime;
    
    public LayerMask WhatIsGround => m_whatIsGround;

    public float Gravity => _gravity;

    /// <summary>
    /// 현재 모든 오브젝트와 충돌하므로 사용되지 않음.
    /// </summary>
    public int DashLayer => m_dashLayer;

    public float DashAmount => m_dashAmount;

    public float DashDuration => m_dashDuration;

    public float DashCoolTime => m_dashCoolTime;

    public float DashDelay => m_dashDelay;

    public int MaxDashCount => m_maxDashCount;
}
