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
    private enum Panels
    {
        BackgroundPanel,
        BackPanel,
        FrontPanel,
        CenterPanel
    }

    private enum Texts
    {
        HpText
    }

    private enum Images
    {
        Crosshair,
        HackingIndicator
    }

    private enum Presenters
    {
        AbilityPresenter
    }

    private readonly List<RectTransform> m_backLayerHpBoxes = new();

    private readonly List<RectTransform> m_frontLayerHpBoxes = new();

    private readonly WaitForEndOfFrame m_waitForEndOfFrameCache = new();
    
    private readonly WaitForSeconds m_waitForSecondsCache = new(0.2f);
    
    private HUDSettings m_settings;

    [SerializeField]
    private GameObject m_hpBoxPrefab;

    [SerializeField]
    private int m_hpPerBox = 10;

    private PlayerController m_controller;

    private PossessionProcessor m_processor;
    
    private GameObject m_backgroundPanel;

    private GameObject m_backPanel;
    
    private GameObject m_frontPanel;
    
    private GameObject m_centerPanel;

    private TextMeshProUGUI m_hpTextBox;

    private UIManagerText m_hpText;

    private GameObject m_backLayerRoot;

    private GameObject m_frontLayerRoot;

    // Center Layer의 Scale을 조절하기 위한 객체
    private GameObject m_centerLayerRoot;
    
    private ProgressBar m_abilityPresenter;

    private Image m_crosshair;
    
    private Image m_hackingIndicator;

    private int m_hpBoxCursor;

    private int m_backLayerCursor;

    private CoroutineEx m_hpChangingEffectCoroutine;
    
    private CoroutineEx m_colorChangeEffectCoroutine;
    
    private CoroutineEx m_hackingIndicatorCoroutine;

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

    public UIHUD Init(HUDSettings settings)
    {
        m_settings = settings;
        return this;
    }

    public UIHUD RegisterController(PlayerController controller)
    {
        UnregisterEvents(m_controller);
        m_controller = controller;
        RegisterEvents(m_controller);
        return this;
    }
    
    public UIHUD RegisterProcessor(PossessionProcessor processor)
    {
        UnregisterEvents(m_processor);
        m_processor = processor;
        RegisterEvents(m_processor);
        return this;
    }

    protected override void Init()
    {
        Bind<GameObject, Panels>();
        Bind<TextMeshProUGUI, Texts>();
        Bind<ProgressBar, Presenters>();
        Bind<Image, Images>();

        m_backgroundPanel = Get<GameObject, Panels>(Panels.BackgroundPanel);
        m_backPanel = Get<GameObject, Panels>(Panels.BackPanel);
        m_frontPanel = Get<GameObject, Panels>(Panels.FrontPanel);
        m_centerPanel = Get<GameObject, Panels>(Panels.CenterPanel);

        m_hpTextBox = Get<TextMeshProUGUI, Texts>(Texts.HpText);
        m_hpText = m_hpTextBox.GetComponent<UIManagerText>();
        
        m_abilityPresenter = Get<ProgressBar, Presenters>(Presenters.AbilityPresenter);
        
        m_crosshair = Get<Image, Images>(Images.Crosshair);
        m_hackingIndicator = Get<Image, Images>(Images.HackingIndicator);

        m_backLayerRoot = m_backPanel.gameObject.FindChild("Root");
        m_frontLayerRoot = m_frontPanel.gameObject.FindChild("Root");
        m_centerLayerRoot = m_centerPanel.gameObject.FindChild("Root");
    }

    private void RegisterEvents(PlayerController controller)
    {
        if (controller == null)
        {
            return;
        }

        controller.CharacterChanged += OnCharacterChanged;
        controller.Damaged += OnDamaged;
        controller.HpChanged += OnHpChanged;
        controller.AbilityRateChanged += OnAbilityRateChanged;
    }

    private void RegisterEvents()
    {
        RegisterEvents(m_controller);
        RegisterEvents(m_processor);
    }
    
    private void UnregisterEvents()
    {
        UnregisterEvents(m_controller);
        UnregisterEvents(m_processor);
    }
    
    private void RegisterEvents(PossessionProcessor processor)
    {
        if (processor == null)
        {
            return;
        }
        
        processor.TargetHit += OnTargetHit;
        processor.Possessable += OnPossessable;
    }

    private void UnregisterEvents(PossessionProcessor processor)
    {
        if (processor == null)
        {
            return;
        }
        
        processor.TargetHit -= OnTargetHit;
        processor.Possessable -= OnPossessable;
    }

    private void UnregisterEvents(PlayerController controller)
    {
        if (controller == null)
        {
            return;
        }

        controller.CharacterChanged -= OnCharacterChanged;
        controller.Damaged -= OnDamaged;
        controller.HpChanged -= OnHpChanged;
        controller.AbilityRateChanged -= OnAbilityRateChanged;
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
        bool isActive = e > 0;
        m_abilityPresenter.gameObject.SetActive(isActive);
        m_crosshair.gameObject.SetActive(isActive);
        m_hpText.gameObject.SetActive(isActive);
        m_backgroundPanel.SetActive(isActive);
        m_centerPanel.SetActive(isActive);

        m_hpTextBox.text = e.ToString();
    }
    
    private void OnAbilityRateChanged(object sender, float e)
    {
        m_abilityPresenter.currentValue = e;
        m_abilityPresenter.UpdateUI();
    }
    
    private void UpdateCrosshair()
    {
        if (m_settings.CrosshairSprites.TryGetValue(m_controller.Character.Data.Type, out Sprite sprite))
        {
            m_crosshair.sprite = sprite;
        }

        m_crosshair.transform.localScale = m_controller.Character switch
        {
            Monster { Attack: { CurrentWeapon: Shotgun } } => Vector3.one * 1.5f,
            Monster { Attack: { CurrentWeapon: Sniper } } => Vector3.one * 0.5f,
            _ => Vector3.one
        };

        m_centerLayerRoot.transform.localScale = m_controller.Character switch
        {
            Monster { Attack: { CurrentWeapon: Sniper } } => Vector3.one,
            _ => Vector3.one * 1.25f
        };
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
    
    private void OnPossessable(object sender, EventArgs e)
    {
        m_hackingIndicator.fillAmount = 1;
        Color from = m_hackingIndicator.color;
        Color to = new(from.r, 0.85f, 0.85f, 1);
        Utility.Lerp(from, to, 0.1f, color => m_hackingIndicator.color = color,
            () =>
            {
                Utility.Lerp(to, from, 0.1f,
                    color => m_hackingIndicator.color = color);
            });
        
        Vector3 scaleFrom = m_centerLayerRoot.transform.localScale;
        Vector3 scaleTo = scaleFrom * 1.05f;
        Utility.Lerp(scaleFrom, scaleTo, 0.1f, scale => m_centerLayerRoot.transform.localScale = scale,
            () =>
            {
                Utility.Lerp(scaleTo, scaleFrom, 0.1f,
                    scale => m_centerLayerRoot.transform.localScale = scale);
            });
    }

    private void OnTargetHit(object sender, float e)
    {
        m_hackingIndicatorCoroutine?.Abort();
        m_hackingIndicatorCoroutine = CoroutineEx.Create(this, CoHackingIndicator(e));
    }

    private IEnumerator CoHackingIndicator(float seconds)
    {
        float timer = 0;
        while (timer < seconds)
        {
            m_hackingIndicator.fillAmount = timer / seconds;
            yield return m_waitForEndOfFrameCache;
            timer += Time.deltaTime;
        }

        m_hackingIndicator.fillAmount = 1f;
    }
}