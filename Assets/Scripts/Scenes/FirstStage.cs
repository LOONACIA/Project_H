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
}
