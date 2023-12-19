#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(TimerBar))]
    public class TimerBarEditor : Editor
    {
        private GUISkin customSkin;
        private TimerBar tbTarget;
        private int currentTab;

        private void OnEnable()
        {
            tbTarget = (TimerBar)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_TimerBar");

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

            var icon = serializedObject.FindProperty("icon");
            var currentValue = serializedObject.FindProperty("currentValue");
            var timeMultiplier = serializedObject.FindProperty("timeMultiplier");
            var timerValue = serializedObject.FindProperty("timerValue");

            var barImage = serializedObject.FindProperty("barImage");
            var iconObject = serializedObject.FindProperty("iconObject");
            var altIconObject = serializedObject.FindProperty("altIconObject");
            var textObject = serializedObject.FindProperty("textObject");
            var altTextObject = serializedObject.FindProperty("altTextObject");

            var canPlay = serializedObject.FindProperty("canPlay");
            var addPrefix = serializedObject.FindProperty("addPrefix");
            var addSuffix = serializedObject.FindProperty("addSuffix");
            var prefix = serializedObject.FindProperty("prefix");
            var suffix = serializedObject.FindProperty("suffix");
            var decimals = serializedObject.FindProperty("decimals");
            var barDirection = serializedObject.FindProperty("barDirection");
            var timeMode = serializedObject.FindProperty("timeMode");

            var onValueChanged = serializedObject.FindProperty("onValueChanged");

            switch (currentTab)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
                    if (tbTarget.barImage != null) { ReachUIEditorHandler.DrawProperty(icon, customSkin, "Bar Icon"); }

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(new GUIContent("Current Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                    currentValue.floatValue = EditorGUILayout.Slider(tbTarget.currentValue, 0, timerValue.floatValue);
                    GUILayout.EndHorizontal();

                    ReachUIEditorHandler.DrawProperty(timerValue, customSkin, "Timer Value");
                    ReachUIEditorHandler.DrawProperty(timeMultiplier, customSkin, "Time Multiplier");

                    tbTarget.UpdateUI();

                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"));
                    break;

                case 1:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
                    ReachUIEditorHandler.DrawProperty(barImage, customSkin, "Bar Image");
                    ReachUIEditorHandler.DrawProperty(iconObject, customSkin, "Icon Object");
                    ReachUIEditorHandler.DrawProperty(altIconObject, customSkin, "Alt Icon Object");
                    ReachUIEditorHandler.DrawProperty(textObject, customSkin, "Text Object");
                    ReachUIEditorHandler.DrawProperty(altTextObject, customSkin, "Alt Text Object");
                    break;

                case 2:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);

                    canPlay.boolValue = ReachUIEditorHandler.DrawToggle(canPlay.boolValue, customSkin, "Can Play");
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    addPrefix.boolValue = ReachUIEditorHandler.DrawTogglePlain(addPrefix.boolValue, customSkin, "Add Prefix");
                    GUILayout.Space(3);

                    if (addPrefix.boolValue == true)
                        ReachUIEditorHandler.DrawPropertyPlainCW(prefix, customSkin, "Prefix:", 40);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    addSuffix.boolValue = ReachUIEditorHandler.DrawTogglePlain(addSuffix.boolValue, customSkin, "Add Suffix");
                    GUILayout.Space(3);

                    if (addSuffix.boolValue == true)
                        ReachUIEditorHandler.DrawPropertyPlainCW(suffix, customSkin, "Suffix:", 40);

                    GUILayout.EndVertical();

                    ReachUIEditorHandler.DrawPropertyCW(decimals, customSkin, "Decimals", 84);
                    ReachUIEditorHandler.DrawPropertyCW(barDirection, customSkin, "Bar Direction", 84);
                    ReachUIEditorHandler.DrawPropertyCW(timeMode, customSkin, "Time Mode", 84);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif