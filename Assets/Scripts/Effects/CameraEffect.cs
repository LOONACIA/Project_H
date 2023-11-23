using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void OnCombo1Ready()
    {
        GameManager.Camera.Animator.SetTrigger(ConstVariables.CAMERA_ANIMATORPARAMETER_COMBO1READY);
    }

    public void OnCombo1Play()
    {
        GameManager.Camera.Animator.SetTrigger(ConstVariables.CAMERA_ANIMATORPARAMETER_COMBO1PLAY);
    }

    public void OnCombo1Recovery()
    {
        GameManager.Camera.Animator.SetTrigger(ConstVariables.CAMERA_ANIMATORPARAMETER_COMBO1RECOVERY);
    }
    #endregion

    #region PrivateMethod
    #endregion
}
