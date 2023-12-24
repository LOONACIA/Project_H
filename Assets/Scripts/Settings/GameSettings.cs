using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Settings/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    [field: Header("Effect")]
    [field: SerializeField]
    public GameObject SparkEffect { get; private set; }

    [field: SerializeField]
    public GameObject DashEffect { get; private set; }

    [field: Header("Actor Prefabs")]
    [field: SerializeField]
    public ActorPrefabMap[] ActorPrefabs { get; private set; }
}