using Michsky.UI.Reach;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UIPopup = LOONACIA.Unity.UI.UIPopup;

public class UISettings : UIPopup
{
    private static string s_defaultSettingsPath;
    
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

    protected override void Awake()
    {
        base.Awake();
        
        s_defaultSettingsPath = Path.Join(Application.dataPath, "settings.es3");
    }

    private void OnEnable()
    {
        if (ES3.FileExists(s_defaultSettingsPath))
        {
            GameManager.Settings.GeneralSettings = ES3.Load<GeneralSettings>("GameSettings", s_defaultSettingsPath);
            LoadSettings();
        }
        else
        {
            ES3.Save("GameSettings", GameManager.Settings.GeneralSettings, s_defaultSettingsPath);
            LoadSettings();
        }
    }

    public void Confirm()
    {
        ES3.Save("GameSettings", GameManager.Settings.GeneralSettings, s_defaultSettingsPath);
        Close();
    }

    public void Cancel()
    {
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
    
    private void LoadSettings()
    {
        m_inputSensitivitySlider.mainSlider.value = GameManager.Settings.GeneralSettings.InputSensitivity;
        // m_masterVolumeSlider.mainSlider.value = GameManager.Settings.GeneralSettings.MasterVolume;
        // m_bgmVolumeSlider.mainSlider.value = GameManager.Settings.GeneralSettings.BGMVolume;
        // m_sfxVolumeSlider.mainSlider.value = GameManager.Settings.GeneralSettings.SFXVolume;
        m_inputSensitivitySlider.UpdateUI();
        m_masterVolumeSlider.UpdateUI();
        m_bgmVolumeSlider.UpdateUI();
        m_sfxVolumeSlider.UpdateUI();
    }

    private void OnInputSensitivityChanged(float arg0)
    {
        GameManager.Settings.GeneralSettings.InputSensitivity = arg0;
    }
    
    private void OnMasterVolumeChanged(float arg0)
    {
    }
    
    private void OnBGMVolumeChanged(float arg0)
    {
    }
    
    private void OnSFXVolumeChanged(float arg0)
    {
    }
}
