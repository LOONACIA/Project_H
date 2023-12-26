using UnityEngine;

[CreateAssetMenu(fileName = nameof(HUDSettings), menuName = "Settings/" + nameof(HUDSettings))]
public class HUDSettings : ScriptableObject
{
    [field: SerializeField]
    public LayerMask AimLayers { get; private set; }
    
    [field: SerializeField]
    public LayerMask ObstacleLayers { get; private set; }

    [field: SerializeField]
    public float CheckRadius { get; private set; } = 0.5f;

    [field: SerializeField]
    public float MinDistance { get; private set; } = 5f;

    [field: SerializeField]
    public float MaxDistance { get; private set; } = 30f;
    
    [field: SerializeField]
    public float DotProductSensitivity { get; private set; } = 0.9f;

    [field: SerializeField]
    public SerializableDictionary<ActorType, Sprite> CrosshairSprites { get; private set; }
}
