#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SocialsWidget))]
    public class SocialsWidgetEditor : Editor
    {
        private SocialsWidget swTarget;
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            swTarget = (SocialsWidget)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_SocialsWidget");

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

            var socials = serializedObject.FindProperty("socials");

            var itemPreset = serializedObject.FindProperty("itemPreset");
            var itemParent = serializedObject.FindProperty("itemParent");
            var buttonPreset = serializedObject.FindProperty("buttonPreset");
            var buttonParent = serializedObject.FindProperty("buttonParent");
            var background = serializedObject.FindProperty("background");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var timer = serializedObject.FindProperty("timer");
            var tintSpeed = serializedObject.FindProperty("tintSpeed");
            var tintCurve = serializedObject.FindProperty("tintCurve");

            switch (currentTab)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(socials, new GUIContent("Socials"), true);
                    EditorGUI.indentLevel = 0;
                    break;

                case 1:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
                    ReachUIEditorHandler.DrawProperty(itemPreset, customSkin, "Item Preset");
                    ReachUIEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    ReachUIEditorHandler.DrawProperty(buttonPreset, customSkin, "Button Preset");
                    ReachUIEditorHandler.DrawProperty(buttonParent, customSkin, "Button Parent");
                    ReachUIEditorHandler.DrawProperty(background, customSkin, "Background");
                    break;

                case 2:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
                    useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    ReachUIEditorHandler.DrawProperty(timer, customSkin, "Timer");
                    ReachUIEditorHandler.DrawProperty(tintSpeed, customSkin, "Tint Speed");
                    ReachUIEditorHandler.DrawProperty(tintCurve, customSkin, "Tint Curve");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif