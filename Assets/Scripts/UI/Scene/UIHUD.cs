using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using Michsky.UI.Reach;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : UIScene
{
    private enum Canvases
    {
        BottomLayer,
        CenterLayer,
    }
    
    private enum Panels
    {
        BackgroundPanel,
        BackPanel,
        FrontPanel,
        HackingPanel,
        AbilityPanel,
        DashPanel,
        QuestHolder,
        ShieldPanel
    }

    private enum Texts
    {
        HpText
    }

    private enum Images
    {
        Crosshair,
        HackingIndicator,
        CooldownIndicator,
        
        // 이 밑으로는 Dash Indicator만 사용
        FirstDashIndicator,
        SecondDashIndicator,
    }

    private enum Presenters
    {
        AbilityIndicator
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
    
    private Actor m_actor;
    
    private GameObject m_backgroundPanel;

    private GameObject m_backPanel;
    
    private GameObject m_frontPanel;
    
    private GameObject m_hackingPanel;
    
    private GameObject m_abilityPanel;
    
    private GameObject m_questHolder;
    
    private GameObject m_shieldPanel;

    private TextMeshProUGUI m_hpTextBox;

    private UIManagerText m_hpText;

    private GameObject m_backLayerRoot;

    private GameObject m_frontLayerRoot;

    // Center Panel의 Scale을 조절하기 위한 객체
    private GameObject m_centerLayerRoot;
    
    private ProgressBar m_abilityIndicator;
    
    private Image m_cooldownIndicator;

    private Image m_crosshair;
    
    private Image m_hackingIndicator;

    private UIManagerImage m_hackingIndicatorManager;
    
    private Image[] m_dashIndicators;

    private int m_hpBoxCursor;

    private int m_backLayerCursor;

    private int m_dashIndicatorCursor;

    private CoroutineEx m_hpChangingEffectCoroutine;
    
    private CoroutineEx m_colorChangeEffectCoroutine;
    
    private CoroutineEx m_hackingIndicatorCoroutine;
    
    private CoroutineEx m_dashIndicatorCoroutine;
    
    private UIManagerImage m_dashIndicatorManager;
    
    private bool m_dashIndicatorInitialized;

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
        Bind<Canvas, Canvases>();
        Bind<GameObject, Panels>();
        Bind<TextMeshProUGUI, Texts>();
        Bind<ProgressBar, Presenters>();
        Bind<Image, Images>();

        m_backgroundPanel = Get<GameObject, Panels>(Panels.BackgroundPanel);
        m_backPanel = Get<GameObject, Panels>(Panels.BackPanel);
        m_frontPanel = Get<GameObject, Panels>(Panels.FrontPanel);
        m_hackingPanel = Get<GameObject, Panels>(Panels.HackingPanel);
        m_abilityPanel = Get<GameObject, Panels>(Panels.AbilityPanel);
        m_questHolder = Get<GameObject, Panels>(Panels.QuestHolder);
        m_shieldPanel = Get<GameObject, Panels>(Panels.ShieldPanel);

        m_hpTextBox = Get<TextMeshProUGUI, Texts>(Texts.HpText);
        m_hpText = m_hpTextBox.GetComponent<UIManagerText>();
        
        m_abilityIndicator = Get<ProgressBar, Presenters>(Presenters.AbilityIndicator);
        m_cooldownIndicator = Get<Image, Images>(Images.CooldownIndicator);
        
        m_crosshair = Get<Image, Images>(Images.Crosshair);
        m_hackingIndicator = Get<Image, Images>(Images.HackingIndicator);
        m_hackingIndicatorManager = m_hackingIndicator.GetComponent<UIManagerImage>();

        m_backLayerRoot = m_backPanel.gameObject.FindChild("Root");
        m_frontLayerRoot = m_frontPanel.gameObject.FindChild("Root");
        m_centerLayerRoot = Get<Canvas, Canvases>(Canvases.CenterLayer).gameObject.FindChild("Root");

        int childCount = Enum.GetValues(typeof(Images)).Length - (int)Images.FirstDashIndicator;
        m_dashIndicators = new Image[childCount];
        for (int index = 0; index < childCount; index++)
        {
            m_dashIndicators[index] = Get<Image, Images>(Images.FirstDashIndicator + index);
        }

        m_dashIndicatorManager = m_dashIndicators.FirstOrDefault()?.GetComponent<UIManagerImage>();
    }

    private void RegisterEvents(PlayerController controller)
    {
        if (controller == null)
        {
            return;
        }
        UnregisterEvents(controller);

        controller.CharacterChanged += OnCharacterChanged;
        controller.Damaged += OnDamaged;
        controller.HpChanged += OnHpChanged;
        controller.AbilityRateChanged += OnAbilityRateChanged;
        controller.SkillCoolTimeChanged += OnSkillCoolTimeChanged;
    }

    private void OnSkillCoolTimeChanged(object sender, float e)
    {
        if (m_controller.Character == null || m_controller.Character.Status == null)
        {
            return;
        }

        m_cooldownIndicator.fillAmount = e;
        m_cooldownIndicator.gameObject.SetActive(m_controller.Character.Status.HasCooldown);
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
        controller.SkillCoolTimeChanged -= OnSkillCoolTimeChanged;
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
        UnregisterEvents(processor);
        
        processor.HackStarted += OnHackStarted;
        processor.Possessable += OnPossessable;
        processor.Possessing += OnPossessing;
        processor.Possessed += OnPossessed;
    }
    

    private void UnregisterEvents(PossessionProcessor processor)
    {
        if (processor == null)
        {
            return;
        }
        
        processor.HackStarted -= OnHackStarted;
        processor.Possessable -= OnPossessable;
        processor.Possessing -= OnPossessing;
        processor.Possessed -= OnPossessed;
    }

    private void OnCharacterChanged(object sender, Actor e)
    {
        if (m_actor != null)
        {
            m_actor.Status.DashCountChanged -= OnDashCountChanged;
            m_actor.Status.DashCoolTimeChanged -= OnDashCoolTimeChanged;
            m_actor.Status.ShieldChanged -= OnShieldChanged;
            m_actor.Status.ShieldDestroyed -= OnShieldDestroyed;
        }
        
        m_actor = e;
        
        ResetHpBoxes();
        m_dashIndicatorCursor = m_actor.Status.CurrentDashCount;
        
        for (int index = 0; index < m_dashIndicators.Length; index++)
        {
            m_dashIndicators[index].fillAmount = index < m_dashIndicatorCursor ? 1 : 0;
        }

        if (m_dashIndicatorInitialized && m_dashIndicators.Length > 0)
        {
            m_dashIndicatorManager.colorType = Mathf.Approximately(m_dashIndicators[0].fillAmount, 1)
                ? UIManagerImage.ColorType.Accent
                : UIManagerImage.ColorType.Negative;
        }
        
        m_shieldPanel.SetActive(false);
        
        if (m_actor != null)
        {
            m_cooldownIndicator.fillAmount = m_actor.Status.SkillCoolTime;
            m_cooldownIndicator.gameObject.SetActive(m_actor.Status.HasCooldown);
            
            m_actor.Status.DashCountChanged += OnDashCountChanged;
            m_actor.Status.DashCoolTimeChanged += OnDashCoolTimeChanged;
            m_actor.Status.ShieldChanged += OnShieldChanged;
            m_actor.Status.ShieldDestroyed += OnShieldDestroyed;
        }
    }

    private void OnShieldDestroyed(object sender, EventArgs e)
    {
        m_shieldPanel.SetActive(false);
    }

    private void OnShieldChanged(object sender, float e)
    { 
        m_shieldPanel.SetActive(e > 0);
    }

    private void OnDashCoolTimeChanged(object sender, float e)
    {
        if (m_dashIndicators.Length <= m_dashIndicatorCursor)
        {
            return;
        }
        
        m_dashIndicators[m_dashIndicatorCursor].fillAmount = e;
        if (m_dashIndicatorCursor == 0)
        {
            m_dashIndicatorManager.colorType = Mathf.Approximately(e, 1) ? UIManagerImage.ColorType.Accent : UIManagerImage.ColorType.Negative;
        }
    }

    private void OnDashCountChanged(object sender, int e)
    {
        // 초기화 시 이벤트가 발생하므로 이를 무시합니다. 대신, m_dashIndicators의 fillAmount를 모두 1로 설정합니다.
        if (!m_dashIndicatorInitialized)
        {
            m_dashIndicatorInitialized = true;
            for (int index = 0; index < m_dashIndicators.Length; index++)
            {
                m_dashIndicators[index].fillAmount = 1;
            }

            return;
        }
        
        m_dashIndicatorCoroutine?.Abort();
        if (m_dashIndicatorCursor < m_dashIndicators.Length)
        {
            m_dashIndicatorCoroutine = CoImageHighlightEffect(m_dashIndicators[m_dashIndicatorCursor], m_dashIndicators[m_dashIndicatorCursor].transform.parent);
        }

        if (m_dashIndicatorCursor < e)
        {
            GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.DashCharged);
        }

        m_dashIndicatorCursor = e;
        
        if (m_dashIndicatorCursor + 1 < m_dashIndicators.Length)
        {
            m_dashIndicators[m_dashIndicatorCursor + 1].fillAmount = 0;
        }
        
        if (m_dashIndicatorCursor < m_dashIndicators.Length)
        {
            m_dashIndicators[m_dashIndicatorCursor].fillAmount = 0;
        }
    }

    private void OnDamaged(object sender, in AttackInfo e)
    {
        m_colorChangeEffectCoroutine?.Abort();
        m_colorChangeEffectCoroutine = CoroutineEx.Create(this, CoColorChangeEffect());
    }

    private void OnHpChanged(object sender, int e)
    {
        bool isActive = e > 0;
        m_crosshair.gameObject.SetActive(isActive);
        m_hpText.gameObject.SetActive(isActive);
        m_backgroundPanel.SetActive(isActive);
        m_hackingPanel.SetActive(isActive);
        m_abilityPanel.SetActive(isActive);
        m_questHolder.SetActive(isActive);
        m_centerLayerRoot.SetActive(isActive);

        m_hpTextBox.text = e.ToString();
    }
    
    private void OnAbilityRateChanged(object sender, float e)
    {
        m_abilityIndicator.currentValue = e;
        m_abilityIndicator.UpdateUI();
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
    
    private void ClearHackingIndicator()
    {
        m_hackingIndicatorCoroutine?.Abort();
        m_hackingIndicator.fillAmount = 0;
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
    
    private void OnPossessing(object sender, EventArgs e)
    {
        ClearHackingIndicator();
    }
    
    private void OnPossessed(object sender, Actor e)
    {
        ClearHackingIndicator();
    }
    
    private void OnPossessable(object sender, EventArgs e)
    {
        m_hackingIndicator.fillAmount = 1;
        _ = CoImageHighlightEffect(m_hackingIndicator, m_hackingIndicator.transform.parent);
    }
    
    private CoroutineEx CoImageHighlightEffect(Image image, Transform root)
    {
        bool hasManager = image.TryGetComponent<UIManagerImage>(out var manager);
        
        Color color = image.color;
        Vector3 scale = root.localScale;
        return CoroutineEx.Create(this, Core(), () =>
        {
            image.color = color;
            root.transform.localScale = scale;
            if (hasManager)
            {
                manager.useCustomColor = false;
            }
        });

        IEnumerator Core()
        {
            Color colorFrom = image.color;
            Color colorTo = colorFrom * 1.5f;// new(from.r, 0.85f, 0.85f, 1);
            Vector3 scaleFrom = root.localScale;
            Vector3 scaleTo = scaleFrom * 1.05f;
            
            if (hasManager)
            {
                manager.useCustomColor = true;
            }
            
            yield return CoHighlightEffect(colorFrom, colorTo, scaleFrom, scaleTo);
            yield return CoHighlightEffect(colorTo, colorFrom, scaleTo, scaleFrom);
        }
        
        IEnumerator CoHighlightEffect(Color colorFrom, Color colorTo, Vector3 scaleFrom, Vector3 scaleTo)
        {
            float timer = 0;
            while (timer < 0.1f)
            {
                image.color = Color.Lerp(colorFrom, colorTo, timer / 0.1f);
                root.localScale = Vector3.Lerp(scaleFrom, scaleTo, timer / 0.1f);
                yield return m_waitForEndOfFrameCache;
                timer += Time.deltaTime;
            }
            image.color = colorTo;
            root.localScale = scaleTo;
        }
    }

    private void OnHackStarted(object sender, float e)
    {
        m_hackingIndicatorManager.colorType = UIManagerImage.ColorType.Negative;
        m_hackingIndicator.fillAmount = 0;
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
        m_hackingIndicatorManager.colorType = UIManagerImage.ColorType.Accent;
    }
}