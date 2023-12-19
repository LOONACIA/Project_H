#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PanelButton))]
    public class PanelButtonEditor : Editor
    {
        private PanelButton buttonTarget;
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            buttonTarget = (PanelButton)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_PanelButton");

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

            var buttonIcon = serializedObject.FindProperty("buttonIcon");
            var buttonText = serializedObject.FindProperty("buttonText");

            var normalCG = serializedObject.FindProperty("normalCG");
            var highlightCG = serializedObject.FindProperty("highlightCG");
            var selectCG = serializedObject.FindProperty("selectCG");
            var normalTextObj = serializedObject.FindProperty("normalTextObj");
            var highlightTextObj = serializedObject.FindProperty("highlightTextObj");
            var selectTextObj = serializedObject.FindProperty("selectTextObj");
            var normalImageObj = serializedObject.FindProperty("normalImageObj");
            var highlightImageObj = serializedObject.FindProperty("highlightImageObj");
            var selectedImageObj = serializedObject.FindProperty("selectedImageObj");
            var seperator = serializedObject.FindProperty("seperator");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var isSelected = serializedObject.FindProperty("isSelected");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useCustomText = serializedObject.FindProperty("useCustomText");
            var useSeperator = serializedObject.FindProperty("useSeperator");
            var useSounds = serializedObject.FindProperty("useSounds");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");

            var onClick = serializedObject.FindProperty("onClick");
            var onHover = serializedObject.FindProperty("onHover");

            switch (currentTab)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
                    ReachUIEditorHandler.DrawPropertyCW(buttonIcon, customSkin, "Button Icon", 80);
                    if (useCustomText.boolValue == false) { ReachUIEditorHandler.DrawPropertyCW(buttonText, customSkin, "Button Text", 80); }
                    if (buttonTarget.buttonIcon != null || useCustomText.boolValue == false) { buttonTarget.UpdateUI(); }

                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
                    EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
                    EditorGUILayout.PropertyField(onHover, new GUIContent("On Hover"), true);
                    break;

                case 1:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
                    ReachUIEditorHandler.DrawProperty(normalCG, customSkin, "Normal CG");
                    ReachUIEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    ReachUIEditorHandler.DrawProperty(selectCG, customSkin, "Select CG");
                    ReachUIEditorHandler.DrawProperty(normalTextObj, customSkin, "Normal Text");
                    ReachUIEditorHandler.DrawProperty(highlightTextObj, customSkin, "Highlight Text");
                    ReachUIEditorHandler.DrawProperty(selectTextObj, customSkin, "Select Text");
                    ReachUIEditorHandler.DrawProperty(normalImageObj, customSkin, "Normal Image");
                    ReachUIEditorHandler.DrawProperty(highlightImageObj, customSkin, "Highlight Image");
                    ReachUIEditorHandler.DrawProperty(selectedImageObj, customSkin, "Select Image");
                    ReachUIEditorHandler.DrawProperty(seperator, customSkin, "Seperator");
                    break;

                case 2:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
                    isInteractable.boolValue = ReachUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    isSelected.boolValue = ReachUIEditorHandler.DrawToggle(isSelected.boolValue, customSkin, "Is Selected");
                    useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    useCustomText.boolValue = ReachUIEditorHandler.DrawToggle(useCustomText.boolValue, customSkin, "Use Custom Text", "Bypasses inspector values and allows manual editing.");
                    useSeperator.boolValue = ReachUIEditorHandler.DrawToggle(useSeperator.boolValue, customSkin, "Use Seperator");
                    useUINavigation.boolValue = ReachUIEditorHandler.DrawToggle(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");
                    useSounds.boolValue = ReachUIEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Button Sounds");
                    ReachUIEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier", "Set the animation fade multiplier.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif