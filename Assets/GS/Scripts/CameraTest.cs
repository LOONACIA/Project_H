using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
public class CameraTest : MonoBehaviour
{
    [FormerlySerializedAs("characterController")]
    public PlayerController m_playerController;


}
#endif
