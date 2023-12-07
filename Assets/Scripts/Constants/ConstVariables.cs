using UnityEngine;

public static class ConstVariables
{
    [Header("Monster")]
    public const float MONSTER_DESTROY_WAIT_TIME = 1f;
    
    [Header("Logging")]
    public const string LOG_PATH = "Logs";
    public const string LOG_FILE_FORMAT = "log-{0}.txt";
    
    [Header("Camera")]
    public const string CAMERASHAKE_GOBLINNORMALATTACKSTART_ANIMATION_NAME = "CameraGoblinAttackStart";
    public const string CAMERASHAKE_GOBLINNORMALATTACKSTOP_ANIMATION_NAME = "CameraGoblinAttackStop";
    public const string CAMERASHAKE_GOBLINNORMALSKILLSTART_ANIMATION_NAME = "CameraGoblinSkillStart";

    public const string CAMERA_ANIMATORPARAMETER_COMBO1READY = "Combo1Ready";
    public const string CAMERA_ANIMATORPARAMETER_COMBO1PLAY = "Combo1Play";
    public const string CAMERA_ANIMATORPARAMETER_COMBO1RECOVERY = "Combo1Recovery";

    [Header("Shuriken")]
    public const string SHURIKEN_PATH = "Prefabs/Possession/Shuriken";
    public const float SHURIKEN_COOLTIME = 3f;


    [Header("Wall")]
    public const string WALL_NORMALMATERIAL_PATH = "Materials/Wall/WallMaterial";
    public const string WALL_NORMALOUTLINEMATERIAL_PATH = "Materials/Wall/WallOutline";
    public const string WALL_POSSESSIONLOADINGMATERIAL_PATH = "Materials/Wall/WallLoadingMaterial";

    [Header("Gate")]
    public const string GATE_MATERIAL_PATH = "Materials/Gate/GateMaterial";
    public const string GATE_APPEAR_MATERIAL_PATH = "Materials/Gate/AppearGateMaterial";
    public const string GATE_DISSOLVE_MATERIAL_PATH = "Materials/Gate/DissolveGateMaterial";

    [Header("Animator Parameter")]
    public const string ANIMATOR_PARAMETER_ATTACK = "Attack";
    public const string ANIMATOR_PARAMETER_BLOCK = "Block";
    public const string ANIMATOR_PARAMETER_SKILL = "Skill";
    public const string ANIMATOR_PARAMETER_POSSESS = "Possess";
    public const string ANIMATOR_PARAMETER_DEAD = "Dead";
    public const string ANIMATOR_PARAMETER_HIT = "Hit";
    public const string ANIMATOR_PARAMETER_STUN = "Stun";
    public const string ANIMATOR_PARAMETER_TARGET_CHECK = "TargetCheck";
    public const string ANIMATOR_PARAMETER_BLOCK_IMPACK_INDEX = "BlockImpactIndex";
    public const string ANIMATOR_PARAMETER_HIT_DIRECTION_X = "HitDirectionX";
    public const string ANIMATOR_PARAMETER_HIT_DIRECTION_Z = "HitDirectionZ";
    public const string ANIMATOR_PARAMETER_ATTACK_INDEX = "AttackIndex";
    public const string ANIMATOR_PARAMETER_MOVEMENT_RATIO = "MovementRatio";

    [Header("Visual Effects")]
    public const string VFX_GRAPH_PARAMETER_PARTICLE_COUNT = "ParticleCount";
    public const string VFX_GRAPH_PARAMETER_DIRECTION = "Direction";
    public const string VFX_GRAPH_EVENT_ON_PLAY = "OnPlay";
    
    [Header("UI")]
    public const float UI_DIALOG_TEXT_TYPE_INTERVAL = 0.05f;

    [Header("Tutorial")]
    public const string TUTORIAL_BROKENSHURIKEN_MATERIAL_PATH = "Materials/Monster/Shooter/BlackBody";
    public const string TUTORIAL_BROKENSHURIKEN_OUTLINEMATERIAL_PATH = "Materials/Monster/BrokenBody/BrokenBodyOutline Material";
}