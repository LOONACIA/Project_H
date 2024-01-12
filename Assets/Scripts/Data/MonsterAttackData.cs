using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterAttackData), menuName = "Data/" + nameof(MonsterAttackData))]
public class MonsterAttackData : ScriptableObject
{
    [field: SerializeField]
    public float SkillCoolTime { get; private set; }
}