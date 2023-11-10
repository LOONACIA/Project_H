using Cinemachine;

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
			m_currentCamera.Priority = 5;
		}
	}
}
