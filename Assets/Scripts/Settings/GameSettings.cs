using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Settings/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    [field: Header("Effect")]
    [field: SerializeField]
    public GameObject BloodEffect { get; private set; }

    [field: Header("Actor Prefabs")]
    [field: SerializeField]
    public ActorPrefabMap[] ActorPrefabs { get; private set; }
}