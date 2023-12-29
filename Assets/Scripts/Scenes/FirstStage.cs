using System.Buffers;
using UnityEngine;
using UnityEngine.Audio;

public class FirstStage : MonoBehaviour
{
    [SerializeField]
    private SFXInfo[] BGMS;

    public AudioMixer audioMixer;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
	{
        
    }

    public void ChangeBgm(int _index)
    {
        //GameManager.Sound.Play(BGMS[_index]).outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
    }
}
