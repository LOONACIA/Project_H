using UnityEngine;

public class ARObject : MonoBehaviour, IARObject
{
    [field: SerializeField]
    public ARObjectInfo Info { get; private set; }
}
