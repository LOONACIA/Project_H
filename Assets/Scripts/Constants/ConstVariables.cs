using System.Collections;
using System.Collections.Generic;
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

    [Header("Shuriken")]
    public const string SHURIKEN_PATH = "Prefabs/Possession/Shuriken";


    [Header("Wall")]
    public const string WALL_NORMALMATERIAL_PATH = "Materials/Wall/WallMaterial";
    public const string WALL_NORMALOUTLINEMATERIAL_PATH = "Materials/Wall/WallOutline";
    public const string WALL_POSSESSIONLOADINGMATERIAL_PATH = "Materials/Wall/WallLoadingMaterial";

    [Header("Animator Parameter")]
    public const string ANIMATOR_PARAMETER_ATTACK = "Attack";
    public const string ANIMATOR_PARAMETER_SKILL = "Skill";
    public const string ANIMATOR_PARAMETER_POSSESS = "Possess";
    public const string ANIMATOR_PARAMETER_DEAD = "Dead";
    public const string ANIMATOR_PARAMETER_HIT = "Hit";
    public const string ANIMATOR_PARAMETER_TARGET_CHECK = "TargetCheck";
    public const string ANIMATOR_PARAMETER_BLOCK_IMPACK_INDEX = "BlockImpactIndex";
    public const string ANIMATOR_PARAMETER_HIT_DIRECTION_X = "HitDirectionX";
    public const string ANIMATOR_PARAMETER_HIT_DIRECTION_Z = "HitDirectionZ";
}