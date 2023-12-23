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
            GameManager.Sound.Play(m_bgm, SoundType.Bgm);
        }
    }
}
