using System;
using UnityEngine;

public class DeathAnimationEventReceiver : MonoBehaviour
{
	public event EventHandler DeathAnimationEnd;
	
	private void OnDeathAnimationEnd()
	{
		DeathAnimationEnd?.Invoke(this, EventArgs.Empty);
	}
}
