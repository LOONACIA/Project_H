using UnityEngine;

[CreateAssetMenu(fileName = nameof(SFXObjectData), menuName = "Data/" + nameof(SFXObjectData))]
public class SFXObjectData : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("표창 해킹물체에 박힐 시")]
    public SFXInfo ShurikenTargetHit { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Detection Light 노출 시")]
    public SFXInfo DetectionLight { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Save Point 성공 시")]
    public SFXInfo SavePointComplete { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Gate Open 시")]
    public SFXInfo GateOpen { get; private set; }

    [field: SerializeField]
    [field: Tooltip("해킹 완료됐을 때 UI 사운드")]
    public SFXInfo HackingComplete { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Dash 회복 되었을 때")]
    public SFXInfo DashCharged { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Dash 없을 시, 불가능한 사운드")]
    public SFXInfo DashUnable { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Object 업데이트 시 사운드")]
    public SFXInfo ObjectUpdate { get; private set; }

    [field: SerializeField]
    [field: Tooltip("쉴드 파괴 시 사운드")]
    public SFXInfo BreakShield { get; private set; }

    [field: SerializeField]
    [field: Tooltip("타이틀 씬 BGM")]
    public SFXInfo TitleSceneBGM { get; private set; }

    [field: SerializeField]
    [field: Tooltip("몬스터에 해킹 시 사운드")]
    public SFXInfo TryHackingSound { get; private set; }

    [field: SerializeField]
    [field: Tooltip("TestBGM")]
    public SFXInfo TestBGM { get; private set; }

    [field: SerializeField]
    [field: Tooltip("TestSFX")]
    public SFXInfo TestSFX { get; private set; }

    [field: SerializeField]
    [field: Tooltip("EliteBoss 타격 이펙트 시작음")]
    public SFXInfo EliteBossAttackEffectStart { get; private set; }

    [field: SerializeField]
    [field: Tooltip("EliteBoss 타격 이펙트 폭발음")]
    public SFXInfo EliteBossAttackEffectExplosion { get; private set; }
}
