using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStage : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_bgm;

	private void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Start()
	{
		if (m_bgm != null)
		{   
            SFXInfo info = new SFXInfo();

            info.audio = m_bgm;
            info.type = SoundType.Bgm;
            info.loop = true;
			GameManager.Sound.Play(info);
		}
	}
}
