using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberCameraEffect : MonoBehaviour
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
    #endregion

    #region PrivateMethod
    #endregion
}
