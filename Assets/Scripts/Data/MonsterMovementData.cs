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

    [Header("Dashing Layer")]
    [SerializeField]
    [Layer]
    private int m_dashLayer = 17;
    
    [FormerlySerializedAs("_groundGravity")]
    [Header("Gravity")]
    [SerializeField]
    private float _gravity;
    
    public float MoveSpeed => m_moveSpeed;
    
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

    public int DashLayer => m_dashLayer;

    public float Gravity => _gravity;
}
