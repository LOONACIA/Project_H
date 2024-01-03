using Michsky.UI.Reach;
using System;
using TMPro;
using UnityEngine;

public class LocalizedObject : MonoBehaviour
{
    private enum ObjectType
    {
        Text,
        Button
    }
    
    [SerializeField]
    private string m_key;
    
    [SerializeField]
    private ObjectType m_objectType;
    
    [SerializeField]
    private TextMeshProUGUI m_textObject;
    
    [SerializeField]
    private ButtonManager m_buttonObject;

    private void OnEnable()
    {
        UpdateUI();
        GameManager.Localization.LanguageChanged += OnLanguageChanged;
    }
    
    private void OnDisable()
    {
        GameManager.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        switch (m_objectType)
        {
            case ObjectType.Text:
                UpdateText();
                break;
            case ObjectType.Button:
                UpdateButton();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateText()
    {
        if (m_textObject == null)
        {
            Debug.LogWarning($"{nameof(m_textObject)} is null.");
            return;
        }
        
        m_textObject.text = GameManager.Localization.Get(m_key);
    }
    
    private void UpdateButton()
    {
        if (m_buttonObject == null)
        {
            Debug.LogWarning($"{nameof(m_buttonObject)} is null.");
            return;
        }
        
        m_buttonObject.buttonText = GameManager.Localization.Get(m_key);
        m_buttonObject.UpdateUI();
    }
}