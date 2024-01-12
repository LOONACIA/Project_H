using System;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Settings/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    private static ES3Settings s_settings;
    
    private static readonly string s_defaultSettingsKey = "GameSettings";
    
    private static readonly string s_defaultSettingsFileName = "settings.dat";
    
    private static readonly string s_defaultSaveFileName = "save.dat";
    
    private static string s_defaultSettingsPath;
    
    private static string s_defaultSavePath;
    
    private bool m_isInitialized;

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
    
    [field: Header("Game")]
    [field: SerializeField]
    public GameData GameData { get; private set; }

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
        if (s_settings == null)
        {
            s_settings = new(ES3.EncryptionType.AES, "penguin");
        }
        
        GeneralSettings = Load();
        GameData data = Load<GameData>();
        if (data is not null)
        {
            GameData = data;
        }
        Save(GameData);
    }

    public void Save()
    {
        Save(GameData);
        Save(GeneralSettings);
    }
    
    public void Save(GameData gameData)
    {
        GameData = gameData;
        
        if (string.IsNullOrEmpty(s_defaultSavePath))
        {
            s_defaultSavePath = Path.Join(Application.dataPath, s_defaultSaveFileName);
        }
        
        ES3.Save(nameof(GameData), gameData, s_defaultSavePath, s_settings);
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
    
    private T Load<T>() where T : class
    {
        if (string.IsNullOrEmpty(s_defaultSavePath))
        {
            s_defaultSavePath = Path.Join(Application.dataPath, s_defaultSaveFileName);
        }

        try
        {
            return ES3.Load<T>(typeof(T).Name, s_defaultSavePath, default, s_settings);
        }
        catch
        {
            File.Delete(s_defaultSavePath);
            return default;
        }
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