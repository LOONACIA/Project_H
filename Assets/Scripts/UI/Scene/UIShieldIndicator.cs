using LOONACIA.Unity.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShieldIndicator : UIScene
{
    private PlayerController m_player;

    private Canvas m_canvas;

    private enum Sliders
    {
        ShieldSlider
    }

    private enum Texts
    {
        ShieldText
    }

    public void SetPlayer(PlayerController player)
    {
        if (m_player != null)
        {
            m_player.ShieldChanged -= OnPlayerShieldChanged;
        }

        m_player = player;
        m_player.ShieldChanged += OnPlayerShieldChanged;

        m_canvas = GetComponentInChildren<Canvas>();
    }

    public void ShowIndicator()
    {
        m_canvas.gameObject.SetActive(true);
    }

    public void HideIndicator()
    {
        m_canvas.gameObject.SetActive(false);
    }

    protected override void Init()
    {
        base.Init();

        Bind<Slider, Sliders>();
        Bind<TextMeshProUGUI, Texts>();
    }

    private void OnPlayerShieldChanged(object sender, EventArgs e)
    {
        if (m_player.Character.Status.Shield == null
            || !m_player.Character.Status.Shield.IsValid)
        {
            HideIndicator();
        }
        else
        {
            ShowIndicator();

            var slider = Get<Slider, Sliders>(Sliders.ShieldSlider);
            slider.maxValue = m_player.Character.Status.Shield.MaxShieldPoint;
            slider.value = m_player.Character.Status.Shield.ShieldPoint;

            var text = Get<TextMeshProUGUI, Texts>(Texts.ShieldText);
            text.text = m_player.Character.Status.Shield.ShieldPoint.ToString();
        }
    }
}
