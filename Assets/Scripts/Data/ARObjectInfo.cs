using UnityEngine;

[CreateAssetMenu(fileName = nameof(ARObjectInfo), menuName = "Data/" + nameof(ARObjectInfo))]
public class ARObjectInfo : ScriptableObject
{
	[field: SerializeField]
    public string Name { get; private set; }
    
    [field: SerializeField]
    public string Description { get; private set; }
    
    [field: SerializeField]
    public Sprite Image { get; private set; }

    [field: SerializeField]
    public float TypeWritingDelay { get; private set; } = 0.05f;
}
