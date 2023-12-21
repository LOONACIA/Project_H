using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using Michsky.UI.Reach;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : UIScene
{
    // private enum Canvases
    // {
    //     BackLayerCanvas,
    //     FrontLayerCanvas
    // }

    private enum Panels
    {
        BackPanel,
        FrontPanel,
    }

    private enum Texts
    {
        HpText
    }

    private enum Images
    {
        Crosshair
    }

    private enum Presenters
    {
        AbilityPresenter
    }

    private readonly List<RectTransform> m_backLayerHpBoxes = new();

    private readonly List<RectTransform> m_frontLayerHpBoxes = new();

    private readonly WaitForEndOfFrame m_waitForEndOfFrameCache = new();
    
    private readonly WaitForSeconds m_waitForSecondsCache = new(0.2f);

    [SerializeField]
    private GameObject m_hpBoxPrefab;

    [SerializeField]
    private int m_hpPerBox = 10;

    private PlayerController m_controller;

    private GameObject m_backPanel;
    
    private GameObject m_frontPanel;

    private TextMeshProUGUI m_hpTextBox;

    private UIManagerText m_hpText;

    private GameObject m_backLayerRoot;

    private GameObject m_frontLayerRoot;
    
    private ProgressBar m_abilityPresenter;

    private Image m_crosshair;

    private int m_hpBoxCursor;

    private int m_backLayerCursor;

    private CoroutineEx m_hpChangingEffectCoroutine;
    
    private CoroutineEx m_colorChangeEffectCoroutine;

    private void OnEnable()
    {
        RegisterEvents();
        m_hpChangingEffectCoroutine = CoroutineEx.Create(this, CoHpChangingEffect());
    }

    private void OnDisable()
    {
        UnregisterEvents();
        m_hpChangingEffectCoroutine?.Abort();
    }

    private void Update()
    {
        UpdateCrosshair();
    }

    public void Register(PlayerController controller)
    {
        UnregisterEvents();
        m_controller = controller;
        RegisterEvents();
    }

    protected override void Init()
    {
        Bind<GameObject, Panels>();
        Bind<TextMeshProUGUI, Texts>();
        Bind<ProgressBar, Presenters>();
        Bind<Image, Images>();

        m_backPanel = Get<GameObject, Panels>(Panels.BackPanel);
        m_frontPanel = Get<GameObject, Panels>(Panels.FrontPanel);

        m_hpTextBox = Get<TextMeshProUGUI, Texts>(Texts.HpText);
        m_hpText = m_hpTextBox.GetComponent<UIManagerText>();
        
        m_abilityPresenter = Get<ProgressBar, Presenters>(Presenters.AbilityPresenter);
        
        m_crosshair = Get<Image, Images>(Images.Crosshair);

        m_backLayerRoot = m_backPanel.gameObject.FindChild("Root");
        m_frontLayerRoot = m_frontPanel.gameObject.FindChild("Root");
    }

    private void RegisterEvents()
    {
        if (m_controller == null)
        {
            return;
        }

        m_controller.CharacterChanged += OnCharacterChanged;
        m_controller.Damaged += OnDamaged;
        m_controller.HpChanged += OnHpChanged;
        m_controller.AbilityRateChanged += OnAbilityRateChanged;
    }

    private void UnregisterEvents()
    {
        if (m_controller == null)
        {
            return;
        }

        m_controller.CharacterChanged -= OnCharacterChanged;
        m_controller.Damaged -= OnDamaged;
        m_controller.HpChanged -= OnHpChanged;
        m_controller.AbilityRateChanged -= OnAbilityRateChanged;
    }

    private void OnCharacterChanged(object sender, Actor e)
    {
        ResetHpBoxes();
    }
    
    private void OnDamaged(object sender, in AttackInfo e)
    {
        m_colorChangeEffectCoroutine?.Abort();
        m_colorChangeEffectCoroutine = CoroutineEx.Create(this, CoColorChangeEffect());
    }

    private void OnHpChanged(object sender, int e)
    {
        m_hpTextBox.enabled = e > 0;

        m_hpTextBox.text = e.ToString();
    }
    
    private void OnAbilityRateChanged(object sender, float e)
    {
        m_abilityPresenter.currentValue = e;
        m_abilityPresenter.UpdateUI();
    }
    
    private void UpdateCrosshair()
    {
        if (m_controller.Character is Monster { Attack: { CurrentWeapon: Shotgun } })
        {
            m_crosshair.transform.localScale = Vector3.one * 2.5f;
        }
        else
        {
            m_crosshair.transform.localScale = Vector3.one * 0.7f;
        }
    }

    private void ResetHpBoxes()
    {
        int respectedHpBoxCount = Mathf.CeilToInt(m_controller.Character.Health.MaxHp / (float)m_hpPerBox) - 1;
        int count = m_backLayerHpBoxes.Count;
        while (count-- > respectedHpBoxCount)
        {
            RectTransform backLayerHpBox = m_backLayerHpBoxes[count];
            RectTransform frontLayerHpBox = m_frontLayerHpBoxes[count];
            m_backLayerHpBoxes.Remove(backLayerHpBox);
            m_frontLayerHpBoxes.Remove(frontLayerHpBox);
            ManagerRoot.Resource.Release(backLayerHpBox.gameObject);
            ManagerRoot.Resource.Release(frontLayerHpBox.gameObject);
        }

        while (count++ < respectedHpBoxCount)
        {
            RectTransform backLayerHpBox = ManagerRoot.Resource.Instantiate(m_hpBoxPrefab, m_backLayerRoot.transform)
                .GetComponent<RectTransform>();
            backLayerHpBox.rotation = m_backLayerRoot.transform.rotation;
            RectTransform frontLayerHpBox = ManagerRoot.Resource.Instantiate(m_hpBoxPrefab, m_frontLayerRoot.transform)
                .GetComponent<RectTransform>();
            frontLayerHpBox.rotation = m_frontLayerRoot.transform.rotation;
            m_backLayerHpBoxes.Add(backLayerHpBox);
            m_frontLayerHpBoxes.Add(frontLayerHpBox);
            backLayerHpBox.GetComponent<UIManagerImage>().colorType = UIManagerImage.ColorType.Negative;
            frontLayerHpBox.GetComponent<UIManagerImage>().colorType = UIManagerImage.ColorType.Secondary;
        }

        for (int index = 0; index < count - 1; index++)
        {
            m_backLayerHpBoxes[index].localScale = m_frontLayerHpBoxes[index].localScale = Vector3.one;
        }

        m_hpBoxCursor = m_backLayerCursor = count - 1;
        m_backLayerHpBoxes[m_hpBoxCursor].localScale = m_frontLayerHpBoxes[m_hpBoxCursor].localScale = new(1, 1, 1);
    }

    private float GetScale()
    {
        return m_controller.Character.Health.CurrentHp % m_hpPerBox / (float)m_hpPerBox;
    }

    private IEnumerator CoHpChangingEffect()
    {
        yield return new WaitUntil(() => m_controller != null);

        while (m_backLayerCursor >= 0)
        {
            int currentHp = m_controller.Character.Health.CurrentHp;
            float frontLayerScale = 1f;

            if (m_hpBoxCursor * m_hpPerBox > currentHp)
            {
                frontLayerScale = 0f;
            }
            else if ((m_hpBoxCursor + 1) * m_hpPerBox > currentHp)
            {
                frontLayerScale = GetScale();
            }

            UpdateHpBox(m_frontLayerHpBoxes, ref m_hpBoxCursor, frontLayerScale, false);

            float backLayerScale = m_frontLayerHpBoxes[m_backLayerCursor].localScale.x;
            UpdateHpBox(m_backLayerHpBoxes, ref m_backLayerCursor, backLayerScale, true, m_backLayerCursor - m_hpBoxCursor + 1);
            
            yield return m_waitForEndOfFrameCache;
        }
    }

    private IEnumerator CoColorChangeEffect()
    {
        m_hpText.colorType = UIManagerText.ColorType.Negative;
        yield return m_waitForSecondsCache;
        m_hpText.colorType = UIManagerText.ColorType.Secondary;
    }

    private static void UpdateHpBox(IReadOnlyList<RectTransform> list, ref int cursor, float xScale, bool isLerp, float lerpDelta = 1)
    {
        RectTransform rectTransform = list[cursor];
        float scale = xScale;
        if (isLerp)
        {
            float delta = (1 - xScale) * (lerpDelta * Time.unscaledDeltaTime);
            scale = Mathf.Max(xScale, rectTransform.localScale.x - delta);
        }

        if (scale <= 0f)
        {
            scale = 0f;
            cursor = Mathf.Max(cursor - 1, 0);
        }

        rectTransform.localScale = new(scale, 1, 1);
    }
}