using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIARObjectInfoCard : UIScene
{
    private enum Images
    {
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom,
        Line
    }

    private enum Texts
    {
        Description
    }
    
    private static readonly int s_showFillAnimationKey = Animator.StringToHash("ShowFill");
    
    private static readonly int s_showScaleAnimationKey = Animator.StringToHash("ShowScale");
    
    [SerializeField]
    private float m_textYPosition = 10f;

    private Animator m_animator;

    private Image m_leftTop;

    private Image m_leftBottom;

    private Image m_rightTop;

    private Image m_rightBottom;

    private TextMeshProUGUI m_description;

    private ARObjectInfo m_info;

    private WaitForSeconds m_waitForSecondsCache;

    private CoroutineEx m_typeWritingCoroutine;

    protected override void Awake()
    {
        base.Awake();

        m_animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (m_description is not null)
        {
            // Clear text for typewriter effect.
            m_description.text = string.Empty;
        }
        
        m_animator.SetTrigger(s_showFillAnimationKey);
    }

    public void SetInfo(ARObjectInfo info)
    {
        m_typeWritingCoroutine?.Abort();
        m_info = info;
        m_waitForSecondsCache = new(info.TypeWritingDelay);
    }
    
    // Called by Animation Event.
    private void StartTypeWriting()
    {
        m_typeWritingCoroutine?.Abort();
        m_typeWritingCoroutine = CoroutineEx.Create(this, TypeDescription());
    }

    /// <summary>
    /// UI 카드의 위치를 업데이트합니다. 모든 위치 좌표는 스크린 좌표계를 기준으로 합니다.
    /// </summary>
    /// <param name="xMin"></param>
    /// <param name="xMax"></param>
    /// <param name="yMin"></param>
    /// <param name="yMax"></param>
    public void UpdatePosition(float xMin, float xMax, float yMin, float yMax)
    {
        float width = xMax - xMin;
        float scale = 1f;
        if (width < 128f)
        {
            scale = Mathf.Max(width, 32f) / 128f;
        }
        
        m_leftTop.rectTransform.anchoredPosition = transform.InverseTransformPoint(new(xMin, yMin));
        m_leftBottom.rectTransform.anchoredPosition = transform.InverseTransformPoint(new(xMin, yMax));
        m_rightTop.rectTransform.anchoredPosition = transform.InverseTransformPoint(new(xMax, yMin));
        m_rightBottom.rectTransform.anchoredPosition = transform.InverseTransformPoint(new(xMax, yMax));

        m_leftTop.rectTransform.localScale = new(scale, scale);
        m_leftBottom.rectTransform.localScale = new(scale, scale);
        m_rightTop.rectTransform.localScale = new(scale, scale);
        m_rightBottom.rectTransform.localScale = new(scale, scale);
        
        // Line의 길이를 실제 text가 렌더링되는 길이와 맞추기 위함
        float respectedWidth = width;
        respectedWidth = Mathf.Max(respectedWidth, m_description.GetRenderedValues().x);
        
        //float xOffset = (respectedWidth - width) / 2f * scale;
        float respectedXCenter = xMin + (respectedWidth / 2f);
        
        Vector3 respectedDescriptionPosition = transform.InverseTransformPoint(new(respectedXCenter, yMin + m_textYPosition * scale));
        m_description.rectTransform.anchoredPosition = respectedDescriptionPosition;
        m_description.rectTransform.sizeDelta = new(respectedWidth, m_description.rectTransform.sizeDelta.y);
    }

    protected override void Init()
    {
        base.Init();

        Bind<Image, Images>();
        Bind<TextMeshProUGUI, Texts>();

        m_leftTop = Get<Image, Images>(Images.LeftTop);
        m_leftBottom = Get<Image, Images>(Images.LeftBottom);
        m_rightTop = Get<Image, Images>(Images.RightTop);
        m_rightBottom = Get<Image, Images>(Images.RightBottom);
        m_description = Get<TextMeshProUGUI, Texts>(Texts.Description);
    }

    private IEnumerator TypeDescription()
    {
        if (m_info == null)
        {
            yield break;
        }

        string text = m_info.Description;

        yield return new WaitUntil(() => m_description != null);
        int cursor = text.Length;

        while (cursor >= 0)
        {
            yield return m_waitForSecondsCache;
            m_description.text = text[..^cursor--];
        }
    }
}