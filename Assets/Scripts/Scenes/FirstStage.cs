using System.Buffers;
using UnityEngine;
using UnityEngine.Audio;

public class FirstStage : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_bgm;

    public AudioMixer audioMixer;

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

        GameManager.Sound.Play(info).outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
    }
}
