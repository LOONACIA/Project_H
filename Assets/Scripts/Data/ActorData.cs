using UnityEngine;

[CreateAssetMenu(fileName = nameof(ActorData), menuName = "Data/" + nameof(ActorData))]
public class ActorData : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("Actor 타입")]
    public ActorType Type { get; private set; }

    [field: SerializeField]
    [field: Tooltip("빙의에 걸리는 시간")]
    public float PossessionRequiredTime { get; private set; }

    [field: SerializeField]
    [Tooltip("표창 적중 시 스턴 시간")]
    public float ShurikenStunTime { get; private set; }

    [field: SerializeField]
    [field: Tooltip("빙의 해제 시 스턴 시간")]
    public float UnpossessionStunTime { get; private set; }

    [field: SerializeField]
    [Tooltip("빙의표창 게임오브젝트")]
    public GameObject ShurikenObj { get; private set; }
}