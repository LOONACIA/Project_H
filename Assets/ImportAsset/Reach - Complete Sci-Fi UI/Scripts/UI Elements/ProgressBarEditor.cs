#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(ProgressBar))]
    public class ProgressBarEditor : Editor
    {
        private GUISkin customSkin;
        private ProgressBar pbTarget;
        private int currentTab;

        private void OnEnable()
        {
            pbTarget = (ProgressBar)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_ProgressBar");

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
            var minValue = serializedObject.FindProperty("minValue");
            var maxValue = serializedObject.FindProperty("maxValue");
            var minValueLimit = serializedObject.FindProperty("minValueLimit");
            var maxValueLimit = serializedObject.FindProperty("maxValueLimit");

            var barImage = serializedObject.FindProperty("barImage");
            var iconObject = serializedObject.FindProperty("iconObject");
            var altIconObject = serializedObject.FindProperty("altIconObject");
            var textObject = serializedObject.FindProperty("textObject");
            var altTextObject = serializedObject.FindProperty("altTextObject");

            var addPrefix = serializedObject.FindProperty("addPrefix");
            var addSuffix = serializedObject.FindProperty("addSuffix");
            var prefix = serializedObject.FindProperty("prefix");
            var suffix = serializedObject.FindProperty("suffix");
            var decimals = serializedObject.FindProperty("decimals");
            var barDirection = serializedObject.FindProperty("barDirection");

            var onValueChanged = serializedObject.FindProperty("onValueChanged");

            switch (currentTab)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
                    if (pbTarget.barImage != null) { ReachUIEditorHandler.DrawPropertyCW(icon, customSkin, "Bar Icon", 99); }

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(new GUIContent("Current Percent"), customSkin.FindStyle("Text"), GUILayout.Width(100));
                    currentValue.floatValue = EditorGUILayout.Slider(pbTarget.currentValue, minValue.floatValue, maxValue.floatValue);
                    GUILayout.EndHorizontal();

                    pbTarget.UpdateUI();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(new GUIContent("Min / Max Value"), customSkin.FindStyle("Text"), GUILayout.Width(110));
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(2);

                    minValue.floatValue = EditorGUILayout.Slider(minValue.floatValue, minValueLimit.floatValue, maxValue.floatValue - 1);
                    maxValue.floatValue = EditorGUILayout.Slider(maxValue.floatValue, minValue.floatValue + 1, maxValueLimit.floatValue);

                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    EditorGUILayout.HelpBox("You can increase the min/max value limit by changing 'Value Limit' in the settings tab.", MessageType.Info);
                    GUILayout.EndVertical();

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

                    ReachUIEditorHandler.DrawPropertyCW(decimals, customSkin, "Decimals", 80);


                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Min Limit"), customSkin.FindStyle("Text"), GUILayout.Width(80));
                    EditorGUILayout.PropertyField(minValueLimit, new GUIContent(""));

                    if (minValueLimit.floatValue >= maxValueLimit.floatValue) { minValueLimit.floatValue = maxValueLimit.floatValue - 1; }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Max Limit"), customSkin.FindStyle("Text"), GUILayout.Width(80));
                    EditorGUILayout.PropertyField(maxValueLimit, new GUIContent(""));

                    if (maxValueLimit.floatValue <= minValue.floatValue) { maxValueLimit.floatValue = minValue.floatValue + 1; }

                    GUILayout.EndHorizontal();

                    ReachUIEditorHandler.DrawPropertyCW(barDirection, customSkin, "Bar Direction", 80);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif