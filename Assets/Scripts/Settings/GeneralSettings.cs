using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneralSettings
{
    [field: Header("General")]
    [field: SerializeField]
    public float InputSensitivity { get; set; }

    public GeneralSettings Clone()
    {
        return (GeneralSettings)MemberwiseClone();
    }
}