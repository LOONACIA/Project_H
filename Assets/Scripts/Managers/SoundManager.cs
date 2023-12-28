using DG.Tweening;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundManager
{
    private readonly AudioSource[] m_audioSources = new AudioSource[System.Enum.GetValues(typeof(SoundType)).Length];

    private readonly Dictionary<string, AudioClip> m_audioClips = new();

    public SFXObjectData ObjectDataSounds;

    public AudioMixer audioMixer;

    public bool isPlayingDetectionSound = false;

    private Tween m_BGMSettingTween;
    private float m_bgmTime;

    public void Init()
    {
        ObjectDataSounds = GameManager.Settings.SFXObjectDatas;
        audioMixer = GameManager.Settings.AudioMixer;

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
            m_audioSources[(int)SoundType.Bgm].outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[1];

            m_audioSources[(int)SoundType.UI].outputAudioMixerGroup = audioMixer.FindMatchingGroups("UI")[0];

            m_audioSources[(int)SoundType.Sfx].outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[1];
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

    public AudioSource Play(SFXInfo _info)
    {   
        return Play(_info.audio, _info.type, _info.volume, _info.pitch, _info.priority, _info.blend, _info.loop);
    }

    public AudioSource Play(AudioClip audioClip, SoundType type = SoundType.Sfx, float volume = 1.0f, float pitch = 1.0f, int priority = 128, float blend = 0f, bool loop = false)
    {
        if (audioClip == null)
        {
            return null;
        }

        AudioSource audioSource = m_audioSources[(int)type];
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.priority = priority;
        audioSource.spatialBlend = blend;
        audioSource.loop = loop;
        if (type == SoundType.Bgm)
        {
            ChangeBGM(audioSource, audioClip);
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
        }

        return audioSource;
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

    public void OffInGame()
    {
        m_bgmTime = m_audioSources[(int)SoundType.Bgm].time;
        m_audioSources[(int)SoundType.Bgm].Stop();

        //AudioMixer.SetFloat("InGame", -80f);
        m_BGMSettingTween?.Kill();
        m_BGMSettingTween = audioMixer.DOSetFloat("InGame", -10f, 2f).SetUpdate(true);
    }

    public void OnInGame()
    {
        m_audioSources[(int)SoundType.Bgm].Play();
        m_audioSources[(int)SoundType.Bgm].time = m_bgmTime;
        //AudioMixer.SetFloat("InGame", 0f);
        m_BGMSettingTween?.Kill();
        m_BGMSettingTween = audioMixer.DOSetFloat("InGame", 0f, 1f).SetUpdate(true);
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

    public SFXInfo ChangeBlend(SFXInfo _info, float value = 1.0f)
    {   
        SFXInfo audioInfo = (SFXInfo)_info.Clone();
        audioInfo.blend = value;

        return audioInfo;
    }

    public AudioClip GetCureentBGM()
    {
        return m_audioSources[(int)SoundType.Bgm].clip;
    }

    public void ChangeBGMDirectly(SFXInfo _info)
    {
        if (_info.audio == null)
        {
            return;
        }

        AudioSource audioSource = m_audioSources[(int)_info.type];
        audioSource.pitch = _info.pitch;
        audioSource.volume = _info.volume;
        audioSource.priority = _info.priority;
        audioSource.spatialBlend = _info.blend;
        audioSource.loop = _info.loop;

        audioSource.Stop();
        audioSource.clip = _info.audio;
        audioSource.Play();
    }

    private void ChangeBGM(AudioSource audioSource, AudioClip audioClip) 
    {
        audioMixer.DOSetFloat("BGM", -40f, 2f).onComplete = () =>
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
            audioMixer.DOSetFloat("BGM", 0f, 3f);
        };
    }

    
}

[System.Serializable]
public class SFXInfo : System.ICloneable
{
    public AudioClip audio;
    public SoundType type;
    public float volume = 1f;
    public float pitch = 1f;
    public int priority = 128;
    public float blend = 0f;
    public bool loop = false;

    public object Clone()
    {
        return new SFXInfo()
        {
            audio = audio,
            type = type,
            volume = volume,
            pitch = pitch,
            priority = priority,
            blend = blend,
            loop = loop,
        };
    }
}