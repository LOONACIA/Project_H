using System;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Settings/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    private static readonly string s_defaultSettingsKey = "GameSettings";
    
    private static readonly string s_defaultSettingsFileName = "settings.dat";
    
    private static string s_defaultSettingsPath;
    
    private bool m_isInitialized;

    [Header("General")]
    [SerializeField]
    private GeneralSettings m_generalSettings = new();

    public GeneralSettings GeneralSettings
    {
        get => m_generalSettings;
        private set
        {
            var settings = m_generalSettings;
            m_generalSettings = value;
            OnGeneralSettingsChanged(settings, m_generalSettings);
        }
    }

    [field: Header("Quest")]
    [field: SerializeField]
    public QuestData QuestData { get; private set; }
    
    [field: Header("Effect")]
    [field: SerializeField]
    public GameObject SparkEffect { get; private set; }

    [field: SerializeField]
    public GameObject DashEffect { get; private set; }

    [field: Header("Actor Prefabs")]
    [field: SerializeField]
    public ActorPrefabMap[] ActorPrefabs { get; private set; }

    [field: Header("SFX Object ScriptableObject")]
    [field: SerializeField]
    public SFXObjectData SFXObjectDatas { get; private set; }

    [field: Header("Audio Mixer")]
    [field: SerializeField]
    public AudioMixer AudioMixer { get; private set; }

    [field: Header("Attack Light")]
    [field: SerializeField]
    public GameObject AttackLight { get; private set; }

    private void OnEnable()
    {
        GeneralSettings.PropertyChanged += OnGeneralSettingsPropertyChanged;
    }
    
    private void OnDisable()
    {
        GeneralSettings.PropertyChanged -= OnGeneralSettingsPropertyChanged;
    }

    public void Initialize()
    {
        if (m_isInitialized)
        {
            return;
        }
        
        m_isInitialized = true;
        GeneralSettings = Load();
    }

    public void Save(GeneralSettings settings)
    {
        GeneralSettings.PropertyChanged -= OnGeneralSettingsPropertyChanged;
        GeneralSettings = settings;
        GeneralSettings.PropertyChanged += OnGeneralSettingsPropertyChanged;
        
        if (string.IsNullOrEmpty(s_defaultSettingsPath))
        {
            s_defaultSettingsPath = Path.Join(Application.dataPath, s_defaultSettingsFileName);
        }
        
        ES3.Save(s_defaultSettingsKey, settings, s_defaultSettingsPath);
    }
    
    public GeneralSettings Load()
    {
        if (string.IsNullOrEmpty(s_defaultSettingsPath))
        {
            s_defaultSettingsPath = Path.Join(Application.dataPath, s_defaultSettingsFileName);
        }
        
        if (ES3.FileExists(s_defaultSettingsPath))
        {
            return ES3.Load<GeneralSettings>(s_defaultSettingsKey, s_defaultSettingsPath);
        }

        ES3.Save(s_defaultSettingsKey, GeneralSettings, s_defaultSettingsPath);

        return GeneralSettings;
    }
    
    private void OnGeneralSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GeneralSettings.MasterVolume):
                AudioMixer.SetFloat("Master", GeneralSettings.MasterVolume);
                break;
            case nameof(GeneralSettings.BGMVolume):
                AudioMixer.SetFloat("BGMSetting", GeneralSettings.BGMVolume);
                break;
            case nameof(GeneralSettings.SfxVolume):
                AudioMixer.SetFloat("SFXSetting", GeneralSettings.SfxVolume);
                AudioMixer.SetFloat("UI", GeneralSettings.SfxVolume);
                break;
        }
    }

    private void OnGeneralSettingsChanged(GeneralSettings oldSettings, GeneralSettings newSettings)
    {
        if (oldSettings is not null)
        {
            oldSettings.PropertyChanged -= OnGeneralSettingsPropertyChanged;
        }
        
        if (newSettings is not null)
        {
            newSettings.PropertyChanged += OnGeneralSettingsPropertyChanged;
            AudioMixer.SetFloat("Master", newSettings.MasterVolume);
            AudioMixer.SetFloat("BGMSetting", newSettings.BGMVolume);
            AudioMixer.SetFloat("SFXSetting", newSettings.SfxVolume);
            AudioMixer.SetFloat("UI", newSettings.SfxVolume);
        }
    }
}