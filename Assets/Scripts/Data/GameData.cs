using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameData), menuName = "Data/" + nameof(GameData))]
public class GameData : ScriptableObject
{
    [field: SerializeField]
    public ChapterInfo[] ChapterInfos { get; private set; }
}
