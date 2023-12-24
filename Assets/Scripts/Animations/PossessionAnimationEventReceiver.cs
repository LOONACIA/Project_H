using UnityEngine;

public class PossessionAnimationEventReceiver : MonoBehaviour, IAnimationEventReceiver
{
	private PossessionProcessor m_possession;

	public void SetPossession(PossessionProcessor possession)
	{
		m_possession = possession;
	}

	private void OnPossessAnimStart()
	{
		if (m_possession is not null)
		{
			m_possession.OnPossessAnimStart();
		}
	}
	
	private void OnPossessAnimEnd()
	{
		if (m_possession is not null)
		{
			m_possession.OnPossessAnimEnd();
		}
	}
}
