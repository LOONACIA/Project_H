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

    [SerializeField]
    [Tooltip("빙의표창 게임오브젝트")]
    private GameObject m_shurikenObj;

    public ActorType Type => m_type;

    public float PossessionRequiredTime => m_possessionRequiredTime;

    public GameObject ShurikenObj => m_shurikenObj;
}