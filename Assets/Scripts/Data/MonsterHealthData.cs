using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterHealthData), menuName = "Data/" + nameof(MonsterHealthData))]
public class MonsterHealthData : ScriptableObject
{
    [SerializeField]
    private int _maxHp;
    
    public int MaxHp => _maxHp;
}
