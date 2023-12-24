using UnityEngine;

[CreateAssetMenu(fileName = nameof(SFXObjectData), menuName = "Data/" + nameof(SFXObjectData))]
public class SFXObjectData : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("표창 해킹물체에 박힐 시")]
    public SFXInfo ShurikenTargetHit { get; private set; }
}
