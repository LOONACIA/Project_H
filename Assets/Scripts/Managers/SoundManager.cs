using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

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

    public void Play(string path, SoundType type = SoundType.Sfx, float volume = 1.0f, float pitch = 1.0f, int priority = 128)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, volume, pitch, priority);
    }

    public void Play(SFXInfo _info)
    {
        Play(_info.audio, _info.type, _info.volume, _info.pitch, _info.priority, _info.blend, _info.loop);
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.Sfx, float volume = 1.0f, float pitch = 1.0f, int priority = 128, float blend = 0f, bool loop = false)
    {
        if (audioClip == null)
        {
            return;
        }

        AudioSource audioSource = m_audioSources[(int)type];
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.priority = priority;
        audioSource.spatialBlend = blend;
        audioSource.loop = loop;
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

    public AudioSource PlayClipAt(SFXInfo _info, Vector3 pos)
    {
        GameObject obj = ManagerRoot.Resource.Instantiate(Resources.Load<GameObject>(ConstVariables.SOUNDBOX));

        obj.transform.position = pos;

        var audioSource = obj.GetComponent<AudioSource>();

        audioSource.clip = _info.audio;
        audioSource.pitch = _info.pitch;
        audioSource.volume = _info.volume;
        audioSource.priority = _info.priority;
        audioSource.spatialBlend = _info.blend;
        audioSource.Play();

        GameManager.Instance.StartCoroutine(Release());

        return audioSource;

        IEnumerator Release()
        {
            yield return new WaitForSeconds(_info.audio.length);
            ManagerRoot.Resource.Release(obj);
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

[System.Serializable]
public class SFXInfo
{
    public AudioClip audio;
    public SoundType type;
    public float volume = 1f;
    public float pitch = 1f;
    public int priority = 128;
    public float blend = 0f;
    public bool loop = false;
}