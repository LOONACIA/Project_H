using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ActorData), menuName = "Data/" + nameof(ActorData))]
public class ActorData : ScriptableObject
{
    [SerializeField]
    [Tooltip("Actor 타입")]
    private ActorType m_type;
    
    [SerializeField]
    [Tooltip("빙의에 걸리는 시간")]
    private float m_possessionRequiredTime;
    
    public ActorType Type => m_type;

    public float PossessionRequiredTime => m_possessionRequiredTime;
}