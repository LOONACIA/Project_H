using LOONACIA.Unity.Managers;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private readonly AudioSource[] m_audioSources = new AudioSource[System.Enum.GetValues(typeof(SoundType)).Length];
    
    private readonly Dictionary<string, AudioClip> m_audioClips = new();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new() { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < soundNames.Length; i++)
            {
                GameObject go = new() { name = soundNames[i] };
                m_audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.SetParent(root.transform);
            }

            m_audioSources[(int)SoundType.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in m_audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        m_audioClips.Clear();
    }

    public void Play(string path, SoundType type = SoundType.Sfx, float volume = 1.0f, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, volume, pitch);
    }

	public void Play(AudioClip audioClip, SoundType type = SoundType.Sfx, float volume = 1.0f, float pitch = 1.0f)
	{
        if (audioClip == null)
        {
            return;
        }

        AudioSource audioSource = m_audioSources[(int)type];
        audioSource.pitch = pitch;
        audioSource.volume = volume;
		if (type == SoundType.Bgm)
		{
            audioSource.Stop();
			audioSource.clip = audioClip;
			audioSource.Play();
		}
		else
		{
			audioSource.PlayOneShot(audioClip);
		}
	}

	private AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.Sfx)
    {
        if (!path.Contains("Sounds/"))
        {
            path = $"Sounds/{path}";
        }

        if (type == SoundType.Sfx && !m_audioClips.TryGetValue(path, out AudioClip audioClip))
		{
            audioClip = ManagerRoot.Resource.Load<AudioClip>(path);
            m_audioClips.Add(path, audioClip);
		}
        else
        {
            audioClip = ManagerRoot.Resource.Load<AudioClip>(path);
        }

        if (audioClip == null)
        {
            Debug.Log($"[SoundManager] Not found audio clip: {path}");
        }

		return audioClip;
    }
}
