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
}