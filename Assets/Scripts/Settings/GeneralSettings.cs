using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class GeneralSettings : INotifyPropertyChanged
{
    private float m_masterVolume = 1f;
    
    private float m_bgmVolume = 1f;
    
    private float m_sfxVolume = 1f;
    
    private float m_inputSensitivity = 5f;

    public float InputSensitivity
    {
        get => m_inputSensitivity;
        set => SetField(ref m_inputSensitivity, value, EventArgCache.InputSensitivity);
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
        internal static readonly PropertyChangedEventArgs InputSensitivity = new(nameof(InputSensitivity));
        internal static readonly PropertyChangedEventArgs MasterVolume = new(nameof(MasterVolume));
        internal static readonly PropertyChangedEventArgs BGMVolume = new(nameof(BGMVolume));
        internal static readonly PropertyChangedEventArgs SfxVolume = new(nameof(SfxVolume));
    }
}