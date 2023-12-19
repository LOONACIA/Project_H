#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : Editor
    {
        private AchievementManager amTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            amTarget = (AchievementManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var allParent = serializedObject.FindProperty("allParent");
            var commonParent = serializedObject.FindProperty("commonParent");
            var rareParent = serializedObject.FindProperty("rareParent");
            var legendaryParent = serializedObject.FindProperty("legendaryParent");
            var achievementPreset = serializedObject.FindProperty("achievementPreset");
            var totalUnlockedObj = serializedObject.FindProperty("totalUnlockedObj");
            var totalValueObj = serializedObject.FindProperty("totalValueObj");
            var commonUnlockedObj = serializedObject.FindProperty("commonUnlockedObj");
            var commonlTotalObj = serializedObject.FindProperty("commonlTotalObj");
            var rareUnlockedObj = serializedObject.FindProperty("rareUnlockedObj");
            var rareTotalObj = serializedObject.FindProperty("rareTotalObj");
            var legendaryUnlockedObj = serializedObject.FindProperty("legendaryUnlockedObj");
            var legendaryTotalObj = serializedObject.FindProperty("legendaryTotalObj");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useAlphabeticalOrder = serializedObject.FindProperty("useAlphabeticalOrder");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
            ReachUIEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");

            if (amTarget.UIManagerAsset != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Library Preset"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                GUI.enabled = false;
                amTarget.UIManagerAsset.achievementLibrary = EditorGUILayout.ObjectField(amTarget.UIManagerAsset.achievementLibrary, typeof(AchievementLibrary), true) as AchievementLibrary;
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 10);
            ReachUIEditorHandler.DrawProperty(achievementPreset, customSkin, "Achievement Preset");
            ReachUIEditorHandler.DrawProperty(allParent, customSkin, "All Parent");
            ReachUIEditorHandler.DrawProperty(commonParent, customSkin, "Common Parent");
            ReachUIEditorHandler.DrawProperty(rareParent, customSkin, "Rare Parent");
            ReachUIEditorHandler.DrawProperty(legendaryParent, customSkin, "Legendary Parent");
            ReachUIEditorHandler.DrawProperty(totalUnlockedObj, customSkin, "Total Unlocked");
            ReachUIEditorHandler.DrawProperty(totalValueObj, customSkin, "Total Value");
            ReachUIEditorHandler.DrawProperty(commonUnlockedObj, customSkin, "Common Unlocked");
            ReachUIEditorHandler.DrawProperty(commonlTotalObj, customSkin, "Commonl Total");
            ReachUIEditorHandler.DrawProperty(rareUnlockedObj, customSkin, "Rare Unlocked");
            ReachUIEditorHandler.DrawProperty(rareTotalObj, customSkin, "Rare Total");
            ReachUIEditorHandler.DrawProperty(legendaryUnlockedObj, customSkin, "Legendary Unlocked");
            ReachUIEditorHandler.DrawProperty(legendaryTotalObj, customSkin, "Legendary Total");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
            useAlphabeticalOrder.boolValue = ReachUIEditorHandler.DrawToggle(useAlphabeticalOrder.boolValue, customSkin, "Use Alphabetical Order");

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif