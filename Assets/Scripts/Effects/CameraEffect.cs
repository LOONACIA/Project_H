using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    #region PublicVariables
    public Vector3 cameraRotation;
    #endregion

    #region PrivateVariables
    private void Update()
    {
        GameManager.Camera.CurrentCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
        Debug.Log(GameManager.Camera.CurrentCamera.transform.localRotation.eulerAngles.y);
    }
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
