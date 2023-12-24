using UnityEngine;

[CreateAssetMenu(fileName = nameof(CharacterControlData), menuName = "Data/" + nameof(CharacterControlData))]
public class CharacterControlData : ScriptableObject
{
    [Header("Camera")]
    [SerializeField]
    private float m_cameraVerticalSensitivity = 5f;
    
    [SerializeField]
    private float m_cameraHorizontalSensitivity = 5f;

    [SerializeField]
    private float m_minCameraVerticalAngle = -20f;
    
    [SerializeField]
    private float m_maxCameraVerticalAngle = 80f;
    
    public float CameraVerticalSensitivity => m_cameraVerticalSensitivity;

    public float CameraHorizontalSensitivity => m_cameraHorizontalSensitivity;

    public float MinCameraVerticalAngle => m_minCameraVerticalAngle;

    public float MaxCameraVerticalAngle => m_maxCameraVerticalAngle;
}
