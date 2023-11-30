using LOONACIA.Unity;
using LOONACIA.Unity.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressRing : UIPopup
{
    public enum TextDisplayMode
    {
        /// <summary>
        /// 텍스트를 표시하지 않음
        /// </summary>
        None,
        
        /// <summary>
        /// 현재 진행률을 텍스트로 표시
        /// </summary>
        Progress,
        
        /// <summary>
        /// 특정 텍스트를 표시
        /// </summary>
        Text
    }
    
	private enum Images
    {
        ProgressRing
    }

    private enum Texts
    {
        Text
    }
    
    private Image m_progressRing;
    
    private TextMeshProUGUI m_text;
    
    private TextDisplayMode m_displayMode;
    
    public TextDisplayMode DisplayMode
    {
        get => m_displayMode;
        set
        {
            m_displayMode = value;
            UpdateText();
        }
    }
    
    public Color ProgressRingColor
    {
        get => m_progressRing.color;
        set => m_progressRing.color = value;
    }
    
    public Color TextColor
    {
        get => m_text.color;
        set => m_text.color = value;
    }

    private void OnEnable()
    {
        UpdateProgress(default);
    }

    private void OnDisable()
    {
        UpdateProgress(default);
    }

    public void SetText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        
        DisplayMode = TextDisplayMode.Text;
        m_text.text = text;
    }

    public void UpdateProgress(float progress)
    {  
        if (DisplayMode == TextDisplayMode.Progress)
        {
            m_text.text = $"{progress * 100f:F0}%";
        }
        
        m_progressRing.fillAmount = progress;
    }

    protected override void Init()
    {
        base.Init();
        
        Bind<Image, Images>();
        Bind<TextMeshProUGUI, Texts>();
        
        m_progressRing = Get<Image, Images>(Images.ProgressRing);
        m_text = Get<TextMeshProUGUI, Texts>(Texts.Text);
    }
    
    private void UpdateText()
    {
        switch (m_displayMode)
        {
            case TextDisplayMode.None:
                m_text.gameObject.SetActive(false);
                return;
            case TextDisplayMode.Progress:
                m_text.gameObject.SetActive(true);
                break;
            case TextDisplayMode.Text:
                m_text.gameObject.SetActive(true);
                break;
        }
    }
}
