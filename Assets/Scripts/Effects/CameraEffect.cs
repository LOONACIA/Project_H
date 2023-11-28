using Cinemachine;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    #region PublicVariables
    public Vector3 cameraRotation;
    #endregion

    #region PrivateVariables
    private CinemachineBasicMultiChannelPerlin vcam;

    private void Start()
    {
        vcam = GameManager.Camera.CurrentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        GameManager.Camera.CurrentCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
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

    public void OnCameraShake()
    {
        vcam.m_AmplitudeGain = 0.3f;
    }

    public void OffCameraShake()
    {
        vcam.m_AmplitudeGain = 0f;
    }
    #endregion

    #region PrivateMethod
    #endregion
}
