#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(HorizontalSelector))]
    public class HorizontalSelectorEditor : Editor
    {
        private GUISkin customSkin;
        private HorizontalSelector hsTarget;
        private int currentTab;

        private void OnEnable()
        {
            hsTarget = (HorizontalSelector)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_HorizontalSelector");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = ReachUIEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab_Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab_Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab_Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var items = serializedObject.FindProperty("items");
            var onValueChanged = serializedObject.FindProperty("onValueChanged");
            var defaultIndex = serializedObject.FindProperty("defaultIndex");

            var label = serializedObject.FindProperty("label");
            var selectorAnimator = serializedObject.FindProperty("selectorAnimator");
            var labelHelper = serializedObject.FindProperty("labelHelper");
            var labelIcon = serializedObject.FindProperty("labelIcon");
            var labelIconHelper = serializedObject.FindProperty("labelIconHelper");
            var indicatorParent = serializedObject.FindProperty("indicatorParent");
            var indicatorObject = serializedObject.FindProperty("indicatorObject");
            var contentLayout = serializedObject.FindProperty("contentLayout");
            var contentLayoutHelper = serializedObject.FindProperty("contentLayoutHelper");

            var enableIcon = serializedObject.FindProperty("enableIcon");
            var saveSelected = serializedObject.FindProperty("saveSelected");
            var saveKey = serializedObject.FindProperty("saveKey");
            var enableIndicator = serializedObject.FindProperty("enableIndicator");
            var invokeOnAwake = serializedObject.FindProperty("invokeOnAwake");
            var invertAnimation = serializedObject.FindProperty("invertAnimation");
            var loopSelection = serializedObject.FindProperty("loopSelection");
            var iconScale = serializedObject.FindProperty("iconScale");
            var contentSpacing = serializedObject.FindProperty("contentSpacing");
            var useLocalization = serializedObject.FindProperty("useLocalization");

            switch (currentTab)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);

                    if (Application.isPlaying == false && hsTarget.items.Count != 0)
                    {
                        if (hsTarget.defaultIndex >= hsTarget.items.Count) { hsTarget.defaultIndex = 0; }

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();

                        GUI.enabled = false;
                        EditorGUILayout.LabelField(new GUIContent("Default Item:"), customSkin.FindStyle("Text"), GUILayout.Width(74));
                        GUI.enabled = true;
                        EditorGUILayout.LabelField(new GUIContent(hsTarget.items[defaultIndex.intValue].itemTitle), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        defaultIndex.intValue = EditorGUILayout.IntSlider(defaultIndex.intValue, 0, hsTarget.items.Count - 1);

                        GUILayout.EndVertical();
                    }

                    else if (Application.isPlaying == true && hsTarget.items.Count != 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        GUI.enabled = false;

                        EditorGUILayout.LabelField(new GUIContent("Current Item:"), customSkin.FindStyle("Text"), GUILayout.Width(74));
                        EditorGUILayout.LabelField(new GUIContent(hsTarget.items[hsTarget.index].itemTitle), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        EditorGUILayout.IntSlider(hsTarget.index, 0, hsTarget.items.Count - 1);

                        GUI.enabled = true;
                        GUILayout.EndVertical();              
                    }

                    else { EditorGUILayout.HelpBox("There is no item in the list.", MessageType.Warning); }

                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(items, new GUIContent("Selector Items"), true);

                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    break;

                case 1:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
                    ReachUIEditorHandler.DrawProperty(selectorAnimator, customSkin, "Animator");
                    ReachUIEditorHandler.DrawProperty(label, customSkin, "Label");
                    ReachUIEditorHandler.DrawProperty(labelHelper, customSkin, "Label Helper");
                    ReachUIEditorHandler.DrawProperty(labelIcon, customSkin, "Label Icon");
                    ReachUIEditorHandler.DrawProperty(labelIconHelper, customSkin, "Label Icon Helper");
                    ReachUIEditorHandler.DrawProperty(indicatorParent, customSkin, "Indicator Parent");
                    ReachUIEditorHandler.DrawProperty(indicatorObject, customSkin, "Indicator Object");
                    ReachUIEditorHandler.DrawProperty(contentLayout, customSkin, "Content Layout");
                    ReachUIEditorHandler.DrawProperty(contentLayoutHelper, customSkin, "Content Layout Helper");
                    break;

                case 2:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Customization", 6);     
                    
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    enableIndicator.boolValue = ReachUIEditorHandler.DrawTogglePlain(enableIndicator.boolValue, customSkin, "Enable Indicators");
                    GUILayout.Space(3);
                    GUILayout.BeginHorizontal();

                    if (enableIndicator.boolValue == true)
                    {
                        if (hsTarget.indicatorObject == null) { EditorGUILayout.HelpBox("'Enable Indicator' is enabled but 'Indicator Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error); }
                        if (hsTarget.indicatorParent == null) { EditorGUILayout.HelpBox("'Enable Indicator' is enabled but 'Indicator Parent' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error); }
                        else { hsTarget.indicatorParent.gameObject.SetActive(true); }
                    }

                    else if (enableIndicator.boolValue == false && hsTarget.indicatorParent != null)
                        hsTarget.indicatorParent.gameObject.SetActive(false);

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    enableIcon.boolValue = ReachUIEditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");
                    GUILayout.Space(3);

                    if (enableIcon.boolValue == true && hsTarget.labelIcon == null)
                        EditorGUILayout.HelpBox("'Enable Icon' is enabled but 'Label Icon' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                    else if (enableIcon.boolValue == true && hsTarget.labelIcon != null)
                        hsTarget.labelIcon.gameObject.SetActive(true);
                    else if (enableIcon.boolValue == false && hsTarget.labelIcon != null)
                        hsTarget.labelIcon.gameObject.SetActive(false);

                    GUILayout.EndVertical();

                    if (enableIcon.boolValue == false) { GUI.enabled = false; }
                    ReachUIEditorHandler.DrawProperty(iconScale, customSkin, "Icon Scale");
                    ReachUIEditorHandler.DrawProperty(contentSpacing, customSkin, "Content Spacing");
                    GUI.enabled = true;
                    hsTarget.UpdateContentLayout();

                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
                    invokeOnAwake.boolValue = ReachUIEditorHandler.DrawToggle(invokeOnAwake.boolValue, customSkin, "Invoke On Awake", "Process events on awake.");
                    invertAnimation.boolValue = ReachUIEditorHandler.DrawToggle(invertAnimation.boolValue, customSkin, "Invert Animation");
                    loopSelection.boolValue = ReachUIEditorHandler.DrawToggle(loopSelection.boolValue, customSkin, "Loop Selection");
                    useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveSelected.boolValue = ReachUIEditorHandler.DrawTogglePlain(saveSelected.boolValue, customSkin, "Save Selected");
                    GUILayout.Space(3);

                    if (saveSelected.boolValue == true)
                    {
                        ReachUIEditorHandler.DrawPropertyCW(saveKey, customSkin, "Save Key:", 66);
                        EditorGUILayout.HelpBox("You must set a unique save key for each selector.", MessageType.Info);
                    }

                    GUILayout.EndVertical();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif