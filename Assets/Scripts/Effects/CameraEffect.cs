using Cinemachine;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    #region PublicVariables
    public Vector3 cameraRotation;
    #endregion

    #region PrivateVariables

    private CinemachineVirtualCamera m_vcam;
    
    private CinemachineBasicMultiChannelPerlin m_perlin;

    private Vector3 m_IntialPos;

    private void Awake()
    {
        var parent = transform.parent;
        m_vcam = parent.GetComponentInChildren<CinemachineVirtualCamera>();
        if (m_vcam != null)
        {
            m_perlin = m_vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            Debug.Log("vcam is null", gameObject);
        }
    }

    private void Update()
    {
        m_vcam.transform.localRotation = Quaternion.Euler(cameraRotation);
    }

    private void OnEnable()
    {
        m_IntialPos = m_vcam.transform.localPosition;
    }

    private void OnDisable()
    {
        m_perlin.m_AmplitudeGain = 0f;
        m_vcam.transform.localPosition = m_IntialPos;
        cameraRotation = Vector3.zero;
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
        m_perlin.m_AmplitudeGain = 0.3f;
    }

    public void OffCameraShake()
    {
        m_perlin.m_AmplitudeGain = 0f;
    }
    #endregion

    #region PrivateMethod
    #endregion
}
