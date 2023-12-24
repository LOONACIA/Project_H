using System.Buffers;
using UnityEngine;

public class FirstStage : MonoBehaviour
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
            ChangeBgm(m_bgm);
        }
    }

    public void ChangeBgm(AudioClip _audio)
    {
        SFXInfo info = new SFXInfo();

        info.audio = _audio;
        info.type = SoundType.Bgm;
        info.loop = true;

        GameManager.Sound.Play(info);
    }
}
