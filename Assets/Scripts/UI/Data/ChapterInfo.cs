using UnityEngine;

[System.Serializable]
public class ChapterInfo
{
    [field: SerializeField]
    public string DisplayName { get; set; }
    
    [field: SerializeField]
    public SceneName SceneName { get; set; }
    
    [field: SerializeField]
    public string Thumbnail { get; set; }
    
    [field: SerializeField]
    public bool IsUnlocked { get; set; }
}
