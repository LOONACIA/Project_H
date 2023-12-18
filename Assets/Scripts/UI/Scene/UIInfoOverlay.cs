using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using Michsky.UI.Shift;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInfoOverlay : UIScene
{
    private enum Canvases
    {
        BackLayerCanvas,
        FrontLayerCanvas
    }
    
    private enum Texts
    {
        HPText
    }    
    
    private readonly List<RectTransform> m_backLayerHpBoxes = new();
    
    private readonly List<RectTransform> m_frontLayerHpBoxes = new();
    
    private readonly WaitForEndOfFrame m_waitForEndOfFrameCache = new();

    [SerializeField]
    private GameObject m_hpBoxPrefab;
    
    [SerializeField]
    private int m_hpPerBox = 10;

    private PlayerController m_controller;
    
    private Canvas m_backLayerCanvas;
    
    private Canvas m_frontLayerCanvas;

    private TextMeshProUGUI m_hpTextBox;

    private GameObject m_backLayerRoot;
    
    private GameObject m_frontLayerRoot;

    private int m_hpBoxCursor;

    private int m_backLayerCursor;
    
    private CoroutineEx m_hpChangingEffectCoroutine;

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

    public void Register(PlayerController controller)
    {
        UnregisterEvents();
        m_controller = controller;
        RegisterEvents();
    }

    protected override void Init()
    {
        Bind<Canvas, Canvases>();
        Bind<TextMeshProUGUI, Texts>();
        
        m_backLayerCanvas = Get<Canvas, Canvases>(Canvases.BackLayerCanvas);
        m_frontLayerCanvas = Get<Canvas, Canvases>(Canvases.FrontLayerCanvas);

        m_hpTextBox = Get<TextMeshProUGUI, Texts>(Texts.HPText);

        m_backLayerRoot = m_backLayerCanvas.gameObject.FindChild("Root");
        m_frontLayerRoot = m_frontLayerCanvas.gameObject.FindChild("Root");
    }

    private void RegisterEvents()
    {
        if (m_controller == null)
        {
            return;
        }
        
        m_controller.CharacterChanged += OnCharacterChanged;
        m_controller.HpChanged += OnHpChanged;
    }

    private void UnregisterEvents()
    {
        if (m_controller == null)
        {
            return;
        }
        
        m_controller.CharacterChanged -= OnCharacterChanged;
        m_controller.HpChanged -= OnHpChanged;
    }
    
    private void OnCharacterChanged(object sender, Actor e)
    {
        ResetHpBoxes();
    }
    
    private void OnHpChanged(object sender, int e)
    {
        m_hpTextBox.text = e.ToString();
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
            RectTransform backLayerHpBox = ManagerRoot.Resource.Instantiate(m_hpBoxPrefab, m_backLayerRoot.transform).GetComponent<RectTransform>();
            backLayerHpBox.rotation = m_backLayerRoot.transform.rotation;
            RectTransform frontLayerHpBox = ManagerRoot.Resource.Instantiate(m_hpBoxPrefab, m_frontLayerRoot.transform).GetComponent<RectTransform>();
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
        return m_controller.Character.Health.CurrentHp % m_hpPerBox / (float)m_hpPerBox;// m_controller.Character.Health.CurrentHp / (float)(m_hpPerBox * (m_hpBoxCursor + 1));
    }

    private IEnumerator CoHpChangingEffect()
    {
        yield return new WaitUntil(() => m_controller != null);
        
        while (true)
        {
            int currentHp = m_controller.Character.Health.CurrentHp;
            float frontLayerScale = 1f;
            int cursor = m_hpBoxCursor + 1;
            if (m_hpBoxCursor * m_hpPerBox > currentHp)
            {
                frontLayerScale = 0f;
            }
            else if (cursor * m_hpPerBox > currentHp)
            {
                frontLayerScale = GetScale();
            }

            UpdateHpBox(in m_frontLayerHpBoxes, ref m_hpBoxCursor, frontLayerScale, false);
            
            float backLayerScale = m_frontLayerHpBoxes[m_backLayerCursor].localScale.x;
            UpdateHpBox(in m_backLayerHpBoxes, ref m_backLayerCursor, backLayerScale, true);
            
            // if (m_controller.Character.Health.CurrentHp <= 0)
            // {
            //     break;
            // }

            yield return m_waitForEndOfFrameCache;
        }
    }

    private void UpdateHpBox(in List<RectTransform> list, ref int cursor, float xScale, bool isLerp)
    {
        const float scaleDelta = 1f;
        float delta = (1 - xScale) * (scaleDelta * Time.unscaledDeltaTime);
        float scale = isLerp ? Mathf.Max(xScale, list[cursor].localScale.x - delta) : xScale;
        RectTransform rectTransform = list[cursor];

        if (scale <= 0f)
        {
            scale = 0f;
            --cursor;
        }

        if (cursor < 0)
        {
            cursor = 0;
        }
        
        rectTransform.localScale = new(scale, 1, 1);
    }
}