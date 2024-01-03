using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

[Serializable]
public class GeneralSettings : INotifyPropertyChanged
{
    [SerializeField]
    private int m_cultureId = CultureInfo.CurrentUICulture.LCID;

    private CultureInfo m_culture;
    
    [SerializeField]
    private float m_lookSensitivity = 5f;
    
    [SerializeField]
    private bool m_invertVerticalView;
    
    [SerializeField]
    private float m_masterVolume = 1f;
    
    [SerializeField]
    private float m_bgmVolume = 1f;
    
    [SerializeField]
    private float m_sfxVolume = 1f;

    public CultureInfo CurrentLanguage
    {
        get => m_culture ??= new(m_cultureId);
        set
        {
            if (m_cultureId == value.LCID)
            {
                return;
            }
            
            m_culture = value;
            SetField(ref m_cultureId, value.LCID, EventArgCache.CurrentLanguage);
        }
    }

    public float LookSensitivity
    {
        get => m_lookSensitivity;
        set => SetField(ref m_lookSensitivity, value, EventArgCache.LookSensitivity);
    }

    public bool InvertVerticalView
    {
        get => m_invertVerticalView;
        set => SetField(ref m_invertVerticalView, value, EventArgCache.InvertVerticalView);
    }

    public float MasterVolume
    {
        get => m_masterVolume;
        set => SetField(ref m_masterVolume, value, EventArgCache.MasterVolume);
    }

    public float BGMVolume
    {
        get => m_bgmVolume;
        set => SetField(ref m_bgmVolume, value, EventArgCache.BGMVolume);
    }

    public float SfxVolume
    {
        get => m_sfxVolume;
        set => SetField(ref m_sfxVolume, value, EventArgCache.SfxVolume);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        PropertyChanged?.Invoke(this, eventArgs);
    }

    protected bool SetField<T>(ref T field, T value, PropertyChangedEventArgs eventArgs)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }
        
        field = value;
        OnPropertyChanged(eventArgs);
        return true;
    }
    
    private static class EventArgCache
    {
        internal static readonly PropertyChangedEventArgs CurrentLanguage = new(nameof(GeneralSettings.CurrentLanguage));
        internal static readonly PropertyChangedEventArgs LookSensitivity = new(nameof(GeneralSettings.LookSensitivity));
        internal static readonly PropertyChangedEventArgs InvertVerticalView = new(nameof(GeneralSettings.InvertVerticalView));
        internal static readonly PropertyChangedEventArgs MasterVolume = new(nameof(GeneralSettings.MasterVolume));
        internal static readonly PropertyChangedEventArgs BGMVolume = new(nameof(GeneralSettings.BGMVolume));
        internal static readonly PropertyChangedEventArgs SfxVolume = new(nameof(GeneralSettings.SfxVolume));
    }
}