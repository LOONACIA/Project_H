using LOONACIA.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHpIndicator : UIScene
{
    private PlayerController m_player;
    
    private enum Sliders
    {
        HpSlider
    }
    
    private enum Texts
    {
        HpText
    }
    
    public void SetPlayer(PlayerController player)
    {
        if (m_player != null)
        {
            m_player.HpChanged -= OnPlayerHpChanged;
        }
        
        m_player = player;
        m_player.HpChanged += OnPlayerHpChanged;
    }
    
    protected override void Init()
    {
        base.Init();
        
        Bind<Slider, Sliders>();
        Bind<TextMeshProUGUI, Texts>();
    }

    private void OnPlayerHpChanged(object sender, int e)
    {
        var slider = Get<Slider, Sliders>(Sliders.HpSlider);
        slider.maxValue = m_player.Character.Health.MaxHp;
        slider.value = e;

        var text = Get<TextMeshProUGUI, Texts>(Texts.HpText);
        text.text = e.ToString();
    }
}
