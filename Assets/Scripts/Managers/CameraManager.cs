using Cinemachine;
using UnityEngine;

public class CameraManager
{
	private CinemachineVirtualCamera m_currentCamera;

    public CinemachineVirtualCamera CurrentCamera
	{
		get => m_currentCamera;
		set
		{
			if (m_currentCamera != null)
			{
				m_currentCamera.Priority = 0;
			}

			m_currentCamera = value;
            Animator = m_currentCamera.GetComponent<Animator>();
			m_currentCamera.Priority = 5;
		}
	}

    public Animator Animator { get; private set; }
}
