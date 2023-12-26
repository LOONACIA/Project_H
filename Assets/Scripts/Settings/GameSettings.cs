using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Settings/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    [field: Header("Quest")]
    [field: SerializeField]
    public QuestData QuestData { get; private set; }
    
    [field: Header("Effect")]
    [field: SerializeField]
    public GameObject SparkEffect { get; private set; }

    [field: SerializeField]
    public GameObject DashEffect { get; private set; }

    [field: Header("Actor Prefabs")]
    [field: SerializeField]
    public ActorPrefabMap[] ActorPrefabs { get; private set; }

    [field: Header("SFX Object ScriptableObject")]
    [field: SerializeField]
    public SFXObjectData SFXObjectDatas { get; private set; }

    [field: Header("Audio Mixer")]
    [field: SerializeField]
    public AudioMixer AudioMixer { get; private set; }
}