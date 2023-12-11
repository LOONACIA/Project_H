using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeleeEffect : MonoBehaviour
{
    public VisualEffect vfx;

    public Vector3 position;
    public Vector3 rotation;

    public void OnMeleeVfxPlay()
    {
        vfx.transform.localPosition = position;
        vfx.transform.localRotation = Quaternion.Euler(rotation);
        vfx.Play();
    }
}
