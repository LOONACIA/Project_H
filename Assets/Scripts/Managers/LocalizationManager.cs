using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class LocalizationManager
{
    private static readonly CultureInfo s_defaultCulture = new("en");
    
    private static readonly CultureInfoComparer s_cultureInfoComparer = new();

    private readonly Dictionary<string, LangResources> m_langResources = new();

    private string m_currentLanguage;

    static LocalizationManager()
    {
        SupportedCultures = new List<CultureInfo> { s_defaultCulture, new("ko"), }.AsReadOnly();
    }

    public static IReadOnlyList<CultureInfo> SupportedCultures { get; }

    public event EventHandler LanguageChanged;
    
    public static bool IsEquals(CultureInfo left, CultureInfo right)
    {
        return s_cultureInfoComparer.Equals(left, right);
    }

    public void Init()
    {
        CultureInfo currentCulture = GameManager.Settings.GeneralSettings.CurrentLanguage ??= CultureInfo.CurrentUICulture;
        SetLanguage(currentCulture);

        var langResources = Resources.LoadAll<LangResources>("Localization");
        foreach (var resource in langResources)
        {
            m_langResources.Add(resource.name, resource);
        }
    }

    public void SetLanguage(CultureInfo cultureInfo)
    {
        if (!SupportedCultures.Contains(cultureInfo, s_cultureInfoComparer))
        {
            cultureInfo = s_defaultCulture;
        }

        if (Equals(m_currentLanguage, cultureInfo.TwoLetterISOLanguageName))
        {
            return;
        }

        m_currentLanguage = cultureInfo.TwoLetterISOLanguageName;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public string Get(string key)
    {
        if (!m_langResources.TryGetValue(m_currentLanguage, out var langResource))
        {
            return key;
        }

        key = key.ToLower();

        return langResource.Resources.GetValueOrDefault(key, key);
    }
    
    private class CultureInfoComparer : IEqualityComparer<CultureInfo>
    {
        public bool Equals(CultureInfo x, CultureInfo y)
        {
            if (x == null && y != null)
            {
                return false;
            }

            if (y == null)
            {
                return false;
            }

            CultureInfo left = x;
            while (!left.Parent.Equals(CultureInfo.InvariantCulture))
            {
                left = x.Parent;
            }
            
            CultureInfo right = y;
            while (!right.Parent.Equals(CultureInfo.InvariantCulture))
            {
                right = y.Parent;
            }
            
            return left.CompareInfo.Equals(right.CompareInfo);
        }

        public int GetHashCode(CultureInfo obj)
        {
            return obj.TwoLetterISOLanguageName.GetHashCode();
        }
    }
}