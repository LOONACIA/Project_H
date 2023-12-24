using UnityEngine;

[CreateAssetMenu(fileName = nameof(SFXMonsterData), menuName = "Data/" + nameof(SFXMonsterData))]
public class SFXMonsterData : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("몬스터 객체")]
    public ActorType Type { get; private set; }

    [field: SerializeField]
    [field: Tooltip("공격1 (Shooter일 시, 샷건)")]
    public SFXInfo Attack1 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("공격2 (Shooter일 시, 저격총)")]
    public SFXInfo Attack2 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("공격3")]
    public SFXInfo Attack3 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("점프")]
    public SFXInfo Jump { get; private set; }

    [field: SerializeField]
    [field: Tooltip("걷기")]
    public SFXInfo Walk { get; private set; }

    [field: SerializeField]
    [field: Tooltip("대쉬")]
    public SFXInfo Dash { get; private set; }

    [field: SerializeField]
    [field: Tooltip("우클릭 동작")]
    public SFXInfo Skill { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Melee일 시, 막을 때 소리 - Elite 일 시, 방어막 있을 시 맞는 소리1")]
    public SFXInfo Shield1 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Melee일 시, 막을 때 소리 - Elite 일 시, 방어막 있을 시 맞는 소리2")]
    public SFXInfo Shield2{ get; private set; }

    [field: SerializeField]
    [field: Tooltip("Melee일 시, 막을 때 소리 - Elite 일 시, 방어막 있을 시 맞는 소리3")]
    public SFXInfo Shield3 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Melee일 시, 막기 자세에서 미는 소리")]
    public SFXInfo ShieldPush { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Shuriken 날리는 소리")]
    public SFXInfo Shuriken { get; private set; }

    [field: SerializeField]
    [field: Tooltip("착지")]
    public SFXInfo Landing { get; private set; }

    [field: SerializeField]
    [field: Tooltip("피격 소리1")]
    public SFXInfo Hit1 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("피격 소리2")]
    public SFXInfo Hit2 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("피격 소리3")]
    public SFXInfo Hit3 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("1인칭 죽음")]
    public SFXInfo FPDeath { get; private set; }

    [field: SerializeField]
    [field: Tooltip("3인칭 죽음1")]
    public SFXInfo TPDeath1 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("3인칭 죽음2")]
    public SFXInfo TPDeath2 { get; private set; }

    [field: SerializeField]
    [field: Tooltip("3인칭 죽음3")]
    public SFXInfo TPDeath3 { get; private set; }
}