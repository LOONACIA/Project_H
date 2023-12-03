using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SyncronizeRotation : Action
{
    [SerializeField]
    private SharedFloat m_epsilon;

    [SerializeField]
    private SharedFloat m_rotationSpeed;
    
    [SerializeField]
    private SharedTransform m_transform;

    [SerializeField]
    private SharedQuaternion m_rotation;

    public override TaskStatus OnUpdate()
    {
        var currentRotation = transform.rotation;
        var targetRotation = m_transform.Value != null ? m_transform.Value.rotation : m_rotation.Value;
        transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, m_rotationSpeed.Value * Time.deltaTime);
        
        return Quaternion.Angle(currentRotation, targetRotation) > m_epsilon.Value ? TaskStatus.Running : TaskStatus.Success;
    }
    
    public override void OnReset()
    {
        m_epsilon = 0.1f;
        m_rotationSpeed = 1f;
        m_transform = null;
        m_rotation = Quaternion.identity;
    }
}
