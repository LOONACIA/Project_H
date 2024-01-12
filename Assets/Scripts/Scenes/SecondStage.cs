using System.Linq;
using UnityEngine;

public class SecondStage : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_bgm;

	private void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
        
        GameManager.Settings.GameData.ChapterInfos.Single(info => info.SceneName == SceneName.Stage2).IsUnlocked = true;
        GameManager.Settings.Save();
	}

	private void Start()
	{
		if (m_bgm != null)
		{
			GameManager.Sound.Play(m_bgm, SoundType.Bgm);
		}
	}
}
