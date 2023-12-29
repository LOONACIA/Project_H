using Michsky.UI.Reach;
using UnityEngine;
using UIPopup = LOONACIA.Unity.UI.UIPopup;

public class UISettings : UIPopup
{
    private static readonly float s_minVolumeValue = -40f;
    
    private static readonly float s_maxVolumeValue = 3f;
    
    private static readonly float s_minSliderValue = 0f;
    
    private static readonly float s_maxSliderValue = 100f;
    
    [SerializeField]
    private HorizontalSelector m_languageSelector;
    
    [SerializeField]
    private SliderManager m_inputSensitivitySlider;
    
    [SerializeField]
    private SliderManager m_masterVolumeSlider;
    
    [SerializeField]
    private SliderManager m_bgmVolumeSlider;
    
    [SerializeField]
    private SliderManager m_sfxVolumeSlider;
    
    private GeneralSettings m_settingsCache;

    private void OnEnable()
    {
        m_settingsCache = GameManager.Settings.Load();
        LoadSettings();
    }

    public void Confirm()
    {
        GameManager.Settings.Save(GameManager.Settings.GeneralSettings);
        Close();
    }

    public void Cancel()
    {
        GameManager.Settings.Save(m_settingsCache);
        Close();
    }

    protected override void Init()
    {
        base.Init();
        
        LoadSettings();
        
        m_inputSensitivitySlider.onValueChanged.AddListener(OnInputSensitivityChanged);
        m_masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        m_bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        m_sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    /// <summary>
    /// Convert slider value to volume value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static float ConvertSliderToVolume(float value)
    {
        value = Mathf.Clamp(value, s_minSliderValue, s_maxSliderValue);

        float newValue = s_minVolumeValue + (value - s_minSliderValue) * (s_maxVolumeValue - s_minVolumeValue) / (s_maxSliderValue - s_minSliderValue);
        return newValue;
    }
    
    /// <summary>
    /// Convert volume value to slider value
    /// </summary>
    /// <param name="originalValue"></param>
    /// <returns></returns>
    private static float ConvertVolumeToSlider(float originalValue)
    {
        float newValue = s_minSliderValue + (originalValue - s_minVolumeValue) * (s_maxSliderValue - s_minSliderValue) / (s_maxVolumeValue - s_minVolumeValue);
        return newValue;
    }
    
    private void LoadSettings()
    {
        m_inputSensitivitySlider.mainSlider.value = GameManager.Settings.GeneralSettings.InputSensitivity;
        m_masterVolumeSlider.mainSlider.value = ConvertVolumeToSlider(GameManager.Settings.GeneralSettings.MasterVolume);
        m_bgmVolumeSlider.mainSlider.value = ConvertVolumeToSlider(GameManager.Settings.GeneralSettings.BGMVolume);
        m_sfxVolumeSlider.mainSlider.value = ConvertVolumeToSlider(GameManager.Settings.GeneralSettings.SfxVolume);
        m_inputSensitivitySlider.UpdateUI();
        m_masterVolumeSlider.UpdateUI();
        m_bgmVolumeSlider.UpdateUI();
        m_sfxVolumeSlider.UpdateUI();
    }

    private void OnInputSensitivityChanged(float value)
    {
        GameManager.Settings.GeneralSettings.InputSensitivity = value;
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        GameManager.Settings.GeneralSettings.MasterVolume = ConvertSliderToVolume(value);
    }
    
    private void OnBGMVolumeChanged(float value)
    {
        GameManager.Settings.GeneralSettings.BGMVolume = ConvertSliderToVolume(value);
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        GameManager.Settings.GeneralSettings.SfxVolume = ConvertSliderToVolume(value);
    }
}
