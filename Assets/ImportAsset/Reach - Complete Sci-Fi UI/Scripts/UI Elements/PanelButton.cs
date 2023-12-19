using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Reach
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class PanelButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Content
        public Sprite buttonIcon;
        public string buttonText = "Button";

        // Resources
        [SerializeField] private CanvasGroup normalCG;
        [SerializeField] private CanvasGroup highlightCG;
        [SerializeField] private CanvasGroup selectCG;
        [SerializeField] private TextMeshProUGUI normalTextObj;
        [SerializeField] private TextMeshProUGUI highlightTextObj;
        [SerializeField] private TextMeshProUGUI selectTextObj;
        [SerializeField] private Image normalImageObj;
        [SerializeField] private Image highlightImageObj;
        [SerializeField] private Image selectedImageObj;
        [SerializeField] private GameObject seperator;

        // Settings
        public bool isInteractable = true;
        public bool isSelected;
        public bool useLocalization = true;
        public bool useCustomText = false;
        public bool useSeperator = true;
        public bool useUINavigation = false;
        public Navigation.Mode navigationMode = Navigation.Mode.Automatic;
        public GameObject selectOnUp;
        public GameObject selectOnDown;
        public GameObject selectOnLeft;
        public GameObject selectOnRight;
        public bool wrapAround = false;
        public bool useSounds = true;
        [Range(1, 15)] public float fadingMultiplier = 8;

        // Events
        public UnityEvent onClick;
        public UnityEvent onHover;

        // Helpers
        bool isInitialized = false;
        Button targetButton;
        LocalizedObject localizedObject;

        void OnEnable()
        {
            if (!isInitialized) { Initialize(); }
            UpdateUI();
        }

        void Initialize()
        {
            if (!Application.isPlaying)
                return;

            if (UIManagerAudio.instance == null) { useSounds = false; }
            if (useUINavigation) { AddUINavigation(); }
            if (gameObject.GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            selectCG.alpha = 0;

            if (useLocalization)
            {
                localizedObject = gameObject.GetComponent<LocalizedObject>();

                if (localizedObject == null || !localizedObject.CheckLocalizationStatus()) { useLocalization = false; }
                else if (useLocalization && !string.IsNullOrEmpty(localizedObject.localizationKey))
                {
                    // Forcing button to take the localized output on awake
                    buttonText = localizedObject.GetKeyOutput(localizedObject.localizationKey);

                    // Change button text on language change
                    localizedObject.onLanguageChanged.AddListener(delegate
                    {
                        buttonText = localizedObject.GetKeyOutput(localizedObject.localizationKey);
                        UpdateUI();
                    });
                }
            }

            isInitialized = true;
        }

        public void AddUINavigation()
        {
            if (targetButton == null)
            {
                targetButton = gameObject.AddComponent<Button>();
                targetButton.transition = Selectable.Transition.None;
            }

            Navigation customNav = new Navigation();
            customNav.mode = navigationMode;

            if (navigationMode == Navigation.Mode.Vertical || navigationMode == Navigation.Mode.Horizontal) { customNav.wrapAround = wrapAround; }
            else if (navigationMode == Navigation.Mode.Explicit) { StartCoroutine("InitUINavigation", customNav); return; }

            targetButton.navigation = customNav;
        }

        public void DisableUINavigation()
        {
            if (targetButton != null)
            {
                Navigation customNav = new Navigation();
                Navigation.Mode navMode = Navigation.Mode.None;
                customNav.mode = navMode;
                targetButton.navigation = customNav;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable) { return; }
            if (useSounds) { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.clickSound); }
        
            onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isInteractable || isSelected) { return; }
            if (useSounds) { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.hoverSound); }

            onHover.Invoke();
            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetNormal");
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetHighlight");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetNormal");
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            onClick.Invoke();
        }

        public void UpdateUI()
        {
            if (useSeperator && transform.parent != null && transform.GetSiblingIndex() != transform.parent.childCount - 1 && seperator != null) { seperator.SetActive(true); }
            else if (seperator != null) { seperator.SetActive(false); }

            if (useCustomText)
                return;

            if (normalTextObj != null) { normalTextObj.text = buttonText; }
            if (highlightTextObj != null) { highlightTextObj.text = buttonText; }
            if (selectTextObj != null) { selectTextObj.text = buttonText; }

            if (normalImageObj != null && buttonIcon != null) { normalImageObj.transform.parent.gameObject.SetActive(true); normalImageObj.sprite = buttonIcon; }
            else if (normalImageObj != null && buttonIcon == null) { normalImageObj.transform.parent.gameObject.SetActive(false); }

            if (highlightImageObj != null && buttonIcon != null) { highlightImageObj.transform.parent.gameObject.SetActive(true); highlightImageObj.sprite = buttonIcon; }
            else if (highlightImageObj != null && buttonIcon == null) { highlightImageObj.transform.parent.gameObject.SetActive(false); }

            if (selectedImageObj != null && buttonIcon != null) { selectedImageObj.transform.parent.gameObject.SetActive(true); selectedImageObj.sprite = buttonIcon; }
            else if (selectedImageObj != null && buttonIcon == null) { selectedImageObj.transform.parent.gameObject.SetActive(false); }

            if (isSelected)
            {
                normalCG.alpha = 0;
                highlightCG.alpha = 0;
                selectCG.alpha = 1;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
            if (isSelected) { StartCoroutine("SetSelect"); }
            else { StartCoroutine("SetNormal"); }
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetHighlight");
            StopCoroutine("SetSelect");

            while (normalCG.alpha < 0.99f)
            {
                normalCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 1;
            highlightCG.alpha = 0;
            selectCG.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetSelect");

            while (highlightCG.alpha < 0.99f)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 0;
            highlightCG.alpha = 1;
            selectCG.alpha = 0;
        }

        IEnumerator SetSelect()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");

            while (selectCG.alpha < 0.99f)
            {
                normalCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCG.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCG.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCG.alpha = 0;
            highlightCG.alpha = 0;
            selectCG.alpha = 1;
        }
    }
}