#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ShopButtonManager))]
    public class ShopButtonManagerEditor : Editor
    {
        private ShopButtonManager buttonTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            buttonTarget = (ShopButtonManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_Button");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            buttonTarget.latestTabIndex = ReachUIEditorHandler.DrawTabs(buttonTarget.latestTabIndex, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab_Content")))
                buttonTarget.latestTabIndex = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab_Resources")))
                buttonTarget.latestTabIndex = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab_Settings")))
                buttonTarget.latestTabIndex = 2;

            GUILayout.EndHorizontal();

            var animator = serializedObject.FindProperty("animator");
            var purchaseButton = serializedObject.FindProperty("purchaseButton");
            var purchasedButton = serializedObject.FindProperty("purchasedButton");
            var purchasedIndicator = serializedObject.FindProperty("purchasedIndicator");
            var purchaseModal = serializedObject.FindProperty("purchaseModal");
            var iconObj = serializedObject.FindProperty("iconObj");
            var titleObj = serializedObject.FindProperty("titleObj");
            var descriptionObj = serializedObject.FindProperty("descriptionObj");
            var priceIconObj = serializedObject.FindProperty("priceIconObj");
            var priceObj = serializedObject.FindProperty("priceObj");
            var filterObj = serializedObject.FindProperty("filterObj");

            var state = serializedObject.FindProperty("state");
            var buttonIcon = serializedObject.FindProperty("buttonIcon");
            var buttonTitle = serializedObject.FindProperty("buttonTitle");
            var titleLocalizationKey = serializedObject.FindProperty("titleLocalizationKey");
            var buttonDescription = serializedObject.FindProperty("buttonDescription");
            var descriptionLocalizationKey = serializedObject.FindProperty("descriptionLocalizationKey");
            var priceIcon = serializedObject.FindProperty("priceIcon");
            var priceText = serializedObject.FindProperty("priceText");
            var backgroundFilter = serializedObject.FindProperty("backgroundFilter");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableTitle = serializedObject.FindProperty("enableTitle");
            var enableDescription = serializedObject.FindProperty("enableDescription");
            var enablePrice = serializedObject.FindProperty("enablePrice");
            var enableFilter = serializedObject.FindProperty("enableFilter");
            var useModalWindow = serializedObject.FindProperty("useModalWindow");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var navigationMode = serializedObject.FindProperty("navigationMode");
            var wrapAround = serializedObject.FindProperty("wrapAround");
            var selectOnUp = serializedObject.FindProperty("selectOnUp");
            var selectOnDown = serializedObject.FindProperty("selectOnDown");
            var selectOnLeft = serializedObject.FindProperty("selectOnLeft");
            var selectOnRight = serializedObject.FindProperty("selectOnRight");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useSounds = serializedObject.FindProperty("useSounds");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");

            var onClick = serializedObject.FindProperty("onClick");
            var onPurchaseClick = serializedObject.FindProperty("onPurchaseClick");
            var onPurchase = serializedObject.FindProperty("onPurchase");

            switch (buttonTarget.latestTabIndex)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
                    ReachUIEditorHandler.DrawPropertyCW(state, customSkin, "Item State", 110);

                    if (useCustomContent.boolValue == false)
                    {
                        if (buttonTarget.iconObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableIcon.boolValue = ReachUIEditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");

                            GUILayout.Space(4);

                            if (enableIcon.boolValue == true)
                            {
                                ReachUIEditorHandler.DrawPropertyCW(buttonIcon, customSkin, "Button Icon", 110);
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.titleObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableTitle.boolValue = ReachUIEditorHandler.DrawTogglePlain(enableTitle.boolValue, customSkin, "Enable Title");

                            GUILayout.Space(4);

                            if (enableTitle.boolValue == true)
                            {
                                ReachUIEditorHandler.DrawPropertyCW(buttonTitle, customSkin, "Button Text", 110);
                                if (useLocalization.boolValue == true) { ReachUIEditorHandler.DrawPropertyCW(titleLocalizationKey, customSkin, "Localization Key", 110); }
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.descriptionObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableDescription.boolValue = ReachUIEditorHandler.DrawTogglePlain(enableDescription.boolValue, customSkin, "Enable Description");

                            GUILayout.Space(4);

                            if (enableDescription.boolValue == true)
                            {
                                ReachUIEditorHandler.DrawPropertyCW(buttonDescription, customSkin, "Description", 110);
                                if (useLocalization.boolValue == true) { ReachUIEditorHandler.DrawPropertyCW(descriptionLocalizationKey, customSkin, "Localization Key", 110); }
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.priceObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enablePrice.boolValue = ReachUIEditorHandler.DrawTogglePlain(enablePrice.boolValue, customSkin, "Enable Price");

                            GUILayout.Space(4);

                            if (enablePrice.boolValue == true)
                            {
                                ReachUIEditorHandler.DrawPropertyCW(priceIcon, customSkin, "Price Icon", 110);
                                ReachUIEditorHandler.DrawPropertyCW(priceText, customSkin, "Price Text", 110);
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.filterObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableFilter.boolValue = ReachUIEditorHandler.DrawTogglePlain(enableFilter.boolValue, customSkin, "Enable Hover Filter");

                            GUILayout.Space(4);

                            if (enableFilter.boolValue == true)
                            {
                                ReachUIEditorHandler.DrawPropertyCW(backgroundFilter, customSkin, "Background Filter", 110);
                            }

                            GUILayout.EndVertical();
                        }

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Space(-3);

                        useModalWindow.boolValue = ReachUIEditorHandler.DrawTogglePlain(useModalWindow.boolValue, customSkin, "Use Modal Window");

                        GUILayout.Space(4);

                        if (useModalWindow.boolValue == true)
                        {
                            ReachUIEditorHandler.DrawPropertyCW(purchaseModal, customSkin, "Purchase Window", 110);
                        }

                        GUILayout.EndVertical();

                        if (Application.isPlaying == false) { buttonTarget.UpdateUI(); }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled. Content is now managed manually.", MessageType.Info); }

                    isInteractable.boolValue = ReachUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");

                    if (Application.isPlaying == true && GUILayout.Button("Update UI", customSkin.button)) { buttonTarget.UpdateUI(); }

                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
                    EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
                    EditorGUILayout.PropertyField(onPurchaseClick, new GUIContent("On Purchase Click"), true);
                    EditorGUILayout.PropertyField(onPurchase, new GUIContent("On Purchase"), true);
                    break;

                case 1:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
                    ReachUIEditorHandler.DrawProperty(animator, customSkin, "Animator");
                    ReachUIEditorHandler.DrawProperty(purchaseButton, customSkin, "Purchase Button");
                    ReachUIEditorHandler.DrawProperty(purchasedButton, customSkin, "Purchased Button");
                    ReachUIEditorHandler.DrawProperty(purchasedIndicator, customSkin, "Purchased Indicator");
                    ReachUIEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
                    ReachUIEditorHandler.DrawProperty(titleObj, customSkin, "Title Object");
                    ReachUIEditorHandler.DrawProperty(descriptionObj, customSkin, "Description Object");
                    ReachUIEditorHandler.DrawProperty(priceObj, customSkin, "Price Object");
                    ReachUIEditorHandler.DrawProperty(priceIconObj, customSkin, "Price Icon Object");
                    ReachUIEditorHandler.DrawProperty(filterObj, customSkin, "Filter Object");
                    break;

                case 2:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
                    isInteractable.boolValue = ReachUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    useCustomContent.boolValue = ReachUIEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content", "Bypasses inspector values and allows manual editing.");
                    useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    GUI.enabled = true;
                    useSounds.boolValue = ReachUIEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Button Sounds");
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    useUINavigation.boolValue = ReachUIEditorHandler.DrawTogglePlain(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

                    GUILayout.Space(4);

                    if (useUINavigation.boolValue == true)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        ReachUIEditorHandler.DrawPropertyPlain(navigationMode, customSkin, "Navigation Mode");

                        if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Horizontal)
                        {
                            EditorGUI.indentLevel = 1;
                            wrapAround.boolValue = ReachUIEditorHandler.DrawToggle(wrapAround.boolValue, customSkin, "Wrap Around");
                            EditorGUI.indentLevel = 0;
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Vertical)
                        {
                            wrapAround.boolValue = ReachUIEditorHandler.DrawTogglePlain(wrapAround.boolValue, customSkin, "Wrap Around");
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Explicit)
                        {
                            EditorGUI.indentLevel = 1;
                            ReachUIEditorHandler.DrawPropertyPlain(selectOnUp, customSkin, "Select On Up");
                            ReachUIEditorHandler.DrawPropertyPlain(selectOnDown, customSkin, "Select On Down");
                            ReachUIEditorHandler.DrawPropertyPlain(selectOnLeft, customSkin, "Select On Left");
                            ReachUIEditorHandler.DrawPropertyPlain(selectOnRight, customSkin, "Select On Right");
                            EditorGUI.indentLevel = 0;
                        }

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();
                    buttonTarget.UpdateUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif