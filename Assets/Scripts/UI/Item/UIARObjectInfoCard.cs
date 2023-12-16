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

    private RectTransform m_canvasRectTransform;

    private CanvasScaler m_canvasScaler;

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
        m_canvasRectTransform = GetComponentInChildren<Canvas>().transform as RectTransform;
        m_canvasScaler = GetComponentInChildren<CanvasScaler>();
    }

    private void OnEnable()
    {
        if (m_description is not null)
        {
            // Clear for typewriter effect.
            m_description.maxVisibleCharacters = 0;
        }
        
        m_animator.SetTrigger(s_showFillAnimationKey);
    }

    public void SetInfo(ARObjectInfo info)
    {
        m_typeWritingCoroutine?.Abort();
        m_info = info;
        m_description.text = m_info.Description;
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
        float distanceScale = 1f;
        if (width < 128f)
        {
            distanceScale = Mathf.Max(width, 32f) / 128f;
        }

        m_leftTop.rectTransform.anchoredPosition = m_canvasRectTransform.InverseTransformPoint(new(xMin, yMin));
        m_leftBottom.rectTransform.anchoredPosition = m_canvasRectTransform.InverseTransformPoint(new(xMin, yMax));
        m_rightTop.rectTransform.anchoredPosition = m_canvasRectTransform.InverseTransformPoint(new(xMax, yMin));
        m_rightBottom.rectTransform.anchoredPosition = m_canvasRectTransform.InverseTransformPoint(new(xMax, yMax));

        m_leftTop.rectTransform.localScale = new(distanceScale, distanceScale);
        m_leftBottom.rectTransform.localScale = new(distanceScale, distanceScale);
        m_rightTop.rectTransform.localScale = new(distanceScale, distanceScale);
        m_rightBottom.rectTransform.localScale = new(distanceScale, distanceScale);
        
        // UI가 반응형으로 동작하게 만드는 스케일 변수
        float reactiveXScale = m_canvasScaler.referenceResolution.x / Screen.width;
        
        // Line의 길이를 실제 text가 렌더링되는 길이와 맞추기 위함
        float respectedWidth = Mathf.Max(width, m_description.GetPreferredValues(m_description.text).x / reactiveXScale);
        
        float respectedXCenter = xMin + (respectedWidth / 2f);
        
        Vector3 respectedDescriptionPosition = m_canvasRectTransform.InverseTransformPoint(new(respectedXCenter, yMin + m_textYPosition * distanceScale));
        m_description.rectTransform.anchoredPosition = respectedDescriptionPosition;
        m_description.rectTransform.sizeDelta = new(respectedWidth * reactiveXScale, m_description.rectTransform.sizeDelta.y);
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

        yield return new WaitUntil(() => m_description != null);
        
        int count = 0;
        int maxLength = m_info.Description.Length;
        
        while (count < maxLength)
        {
            yield return m_waitForSecondsCache;
            m_description.maxVisibleCharacters = ++count;
        }
    }
}