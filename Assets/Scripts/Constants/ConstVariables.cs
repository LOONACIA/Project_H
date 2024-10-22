using LOONACIA.Unity;
using UnityEngine;

public static class ConstVariables
{
    [Header("Hacking")]
    public const float HACKING_SUCCESS_EFFECT_DURATION = 0.5f;
    
    [Header("Monster")]
    public const float MONSTER_DESTROY_WAIT_TIME = 1f;
    public const float MONSTER_SPAWN_HEIGHT = 5;
    public const float MONSTER_HEIGHT = 2f;
    public const float MONSTER_RADIUS = 0.5f;

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
    public const float SHURIKEN_COOLTIME = 0.1f;
    
    [Header("Wall")]
    public const string WALL_NORMALMATERIAL_PATH = "Materials/Wall/WallMaterial";
    public const string WALL_NORMALOUTLINEMATERIAL_PATH = "Materials/Wall/WallOutline";
    public const string WALL_POSSESSIONLOADINGMATERIAL_PATH = "Materials/Wall/WallLoadingMaterial";
    public const string WALL_HACKINGMATERIAL_PATH = "Materials/Wall/HackingWallHackingMaterial";
    public const string WALL_HACKINGNORMALMATERIAL_PATH = "Materials/Wall/HackingWallIdle";



    [Header("Gate")]
    public const string GATE_MATERIAL_PATH = "Materials/Gate/GateMaterial";
    public const string GATE_APPEAR_MATERIAL_PATH = "Materials/Gate/AppearGateMaterial";
    public const string GATE_DISSOLVE_MATERIAL_PATH = "Materials/Gate/DissolveGateMaterial";

    [Header("Animator Parameter")]
    public const string ANIMATOR_PARAMETER_ATTACK = "Attack";
    public const string ANIMATOR_PARAMETER_BLOCK = "Block";
    public const string ANIMATOR_PARAMETER_ABILITY = "Ability";
    public const string ANIMATOR_PARAMETER_POSSESS = "Possess";
    public const string ANIMATOR_PARAMETER_DEAD = "Dead";
    public const string ANIMATOR_PARAMETER_HIT = "Hit";
    public const string ANIMATOR_PARAMETER_STUN = "Stun";
    public const string ANIMATOR_PARAMETER_IS_STUNED = "IsStunned";
    public const string ANIMATOR_PARAMETER_TARGET_CHECK = "TargetCheck";
    public const string ANIMATOR_PARAMETER_BLOCK_IMPACK_INDEX = "BlockImpactIndex";
    public const string ANIMATOR_PARAMETER_HIT_DIRECTION_X = "HitDirectionX";
    public const string ANIMATOR_PARAMETER_HIT_DIRECTION_Z = "HitDirectionZ";
    public const string ANIMATOR_PARAMETER_ATTACK_INDEX = "AttackIndex";
    public const string ANIMATOR_PARAMETER_MOVEMENT_RATIO = "MovementRatio";
    public const string ANIMATOR_PARAMETER_AIM_ANGLE = "AimAngle";
    public const string ANIMATOR_PARAMETER_JUMP = "Jump";
    public const string ANIMATOR_PARAMETER_IS_HIT_PLAYING = "IsHitPlaying";
    public const string ANIMATOR_PARAMETER_KNOCKBACK = "KnockBack";
    public const string ANIMATOR_PARAMETER_LOOK = "Look";
    public const string ANIMATOR_PARAMETER_WAKE_UP = "WakeUp";

    [Header("Animator Layer")]
    public const string ANIMATOR_LAYER_BASE_LAYER = "Base Layer";
    public const string ANIMATOR_LAYER_AIM_LAYER = "Aim Layer";

    [Header("Visual Effects")]
    public const string VFX_GRAPH_PARAMETER_PARTICLE_COUNT = "ParticleCount";
    public const string VFX_GRAPH_PARAMETER_DIRECTION = "Direction";
    public const string VFX_GRAPH_EVENT_ON_PLAY = "OnPlay";
    
    [Header("UI")]
    public const float UI_DIALOG_TEXT_TYPE_INTERVAL = 0.05f;

    [Header("Tutorial")]
    public const string TUTORIAL_BROKENSHURIKEN_MATERIAL_PATH = "Materials/Monster/Shooter/BlackBody";
    public const string TUTORIAL_BROKENSHURIKEN_OUTLINEMATERIAL_PATH = "Materials/Monster/BrokenBody/BrokenBodyOutline Material";

    [Header("CollisionLayers")]
    [Layer]
    public static readonly string[] MOVEMENT_COLLISION_LAYERS = new[] { "Ground", "Wall", "Obstacle", "Gate", "BlockPlane", "Monster", "InteractableObject" };
    public const string LAYER_MONSTER = "Monster";
    public const string LAYER_GROUND = "Ground";
    public const string LAYER_WALL = "Wall";

    [Header("Sound")]
    public static readonly string SOUNDBOX = "Prefabs/Objects/SoundBox";
    public static readonly string SFXOBJECT_SRITABLEOBJECT = "Data/SFXOjbectData";
}