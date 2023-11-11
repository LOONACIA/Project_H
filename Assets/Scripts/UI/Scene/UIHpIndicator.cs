using LOONACIA.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHpIndicator : UIScene
{
    private CharacterController m_player;
    
    private enum Sliders
    {
        HpSlider
    }
    
    private enum Texts
    {
        HpText
    }
    
    public void SetPlayer(CharacterController player)
    {
        if (m_player != null)
        {
            m_player.HpChanged -= OnPlayerHpChanged;
        }
        
        m_player = player;
        m_player.HpChanged += OnPlayerHpChanged;
    }

    private void OnPlayerHpChanged(object sender, int e)
    {
        var slider = Get<Slider, Sliders>(Sliders.HpSlider);
        slider.value = e;
        slider.maxValue = m_player.Character.Health.MaxHp;

        var text = Get<TextMeshProUGUI, Texts>(Texts.HpText);
        text.text = e.ToString();
    }

    protected override void Init()
    {
        base.Init();
        
        Bind<Slider, Sliders>();
        Bind<TextMeshProUGUI, Texts>();
    }
}
