using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Michsky.UI.Reach
{
    [ExecuteInEditMode]
    public class TimerBar : MonoBehaviour
    {
        // Content
        public Sprite icon;
        public float currentValue;
        public float timerValue = 60;
        public float timeMultiplier = 1;

        // Resources
        public Image barImage;
        [SerializeField] private Image iconObject;
        [SerializeField] private Image altIconObject;
        [SerializeField] private TextMeshProUGUI textObject;
        [SerializeField] private TextMeshProUGUI altTextObject;

        // Settings
        public bool canPlay = true;
        public bool addPrefix;
        public bool addSuffix;
        [Range(0, 5)] public int decimals = 0;
        public string prefix = "";
        public string suffix = "";
        public BarDirection barDirection = BarDirection.Left;
        public TimeMode timeMode = TimeMode.UnscaledDeltaTime;

        // Events
        [System.Serializable]
        public class OnValueChanged : UnityEvent<float> { }
        public OnValueChanged onValueChanged;

        // Helpers
        [HideInInspector] public Slider eventSource;

        public enum BarDirection { Left, Right, Top, Bottom }
        public enum TimeMode { DeltaTime, UnscaledDeltaTime }

        void Start()
        {
            Initialize();
            UpdateUI();
        }

        void Update()
        {
            if (Application.isPlaying == true && canPlay == false) { return; }
            else if (Application.isPlaying == true && canPlay == true)
            {
                if (currentValue > 0 && timeMode == TimeMode.UnscaledDeltaTime) { currentValue -= Time.unscaledDeltaTime * timeMultiplier; }
                else if (currentValue > 0 && timeMode == TimeMode.DeltaTime) { currentValue -= Time.deltaTime * timeMultiplier; }
                else { currentValue = 0; }
            }
          
            UpdateUI();
            SetBarDirection();
        }

        public void UpdateUI()
        {
            if (barImage != null) { barImage.fillAmount = currentValue / timerValue; }
            if (iconObject != null) { iconObject.sprite = icon; }
            if (altIconObject != null) { altIconObject.sprite = icon; }
            if (textObject != null) { UpdateText(textObject); }
            if (altTextObject != null) { UpdateText(altTextObject); }
            if (eventSource != null) { eventSource.value = currentValue; }
        }

        void UpdateText(TextMeshProUGUI txt)
        {
            if (addSuffix == true) { txt.text = currentValue.ToString("F" + decimals) + suffix; }
            else { txt.text = currentValue.ToString("F" + decimals); }

            if (addPrefix == true) { txt.text = prefix + txt.text; }
        }

        public void Initialize()
        {
            if (Application.isPlaying == true && onValueChanged.GetPersistentEventCount() != 0)
            {
                if (eventSource == null) { eventSource = gameObject.AddComponent(typeof(Slider)) as Slider; }
                eventSource.transition = Selectable.Transition.None;
                eventSource.minValue = 0;
                eventSource.maxValue = timerValue;
                eventSource.onValueChanged.AddListener(onValueChanged.Invoke);
            }

            SetBarDirection();
        }

        public void Play(bool value)
        {
            canPlay = value;
        }

        public void SetTimer(float value)
        {
            currentValue = value;
        }

        void SetBarDirection()
        {
            if (barImage != null)
            {
                barImage.type = Image.Type.Filled;
                if (barDirection == BarDirection.Left) { barImage.fillMethod = Image.FillMethod.Horizontal; barImage.fillOrigin = 0; }
                else if (barDirection == BarDirection.Right) { barImage.fillMethod = Image.FillMethod.Horizontal; barImage.fillOrigin = 1; }
                else if (barDirection == BarDirection.Top) { barImage.fillMethod = Image.FillMethod.Vertical; barImage.fillOrigin = 1; }
                else if (barDirection == BarDirection.Bottom) { barImage.fillMethod = Image.FillMethod.Vertical; barImage.fillOrigin = 0; }
            }
        }

        public void ClearEvents() { eventSource.onValueChanged.RemoveAllListeners(); }
        public void SetValue(float newValue) { currentValue = newValue; UpdateUI(); }
    }
}