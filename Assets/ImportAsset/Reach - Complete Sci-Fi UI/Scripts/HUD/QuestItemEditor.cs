#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuestItem))]
    public class QuestItemEditor : Editor
    {
        private QuestItem qiTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            qiTarget = (QuestItem)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var questText = serializedObject.FindProperty("questText");
            var localizationKey = serializedObject.FindProperty("localizationKey");

            var questAnimator = serializedObject.FindProperty("questAnimator");
            var questTextObj = serializedObject.FindProperty("questTextObj");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var updateOnAnimate = serializedObject.FindProperty("updateOnAnimate");
            var minimizeAfter = serializedObject.FindProperty("minimizeAfter");
            var defaultState = serializedObject.FindProperty("defaultState");
            var afterMinimize = serializedObject.FindProperty("afterMinimize");

            var onDestroy = serializedObject.FindProperty("onDestroy");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Quest Text"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
            EditorGUILayout.PropertyField(questText, new GUIContent(""), GUILayout.Height(70));
            GUILayout.EndHorizontal();
            ReachUIEditorHandler.DrawProperty(localizationKey, customSkin, "Localization Key");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 10);
            ReachUIEditorHandler.DrawProperty(questAnimator, customSkin, "Quest Animator");
            ReachUIEditorHandler.DrawProperty(questTextObj, customSkin, "Quest Text Object");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
            updateOnAnimate.boolValue = ReachUIEditorHandler.DrawToggle(updateOnAnimate.boolValue, customSkin, "Update On Animate");
            ReachUIEditorHandler.DrawProperty(minimizeAfter, customSkin, "Minimize After");
            ReachUIEditorHandler.DrawProperty(defaultState, customSkin, "Default State");
            ReachUIEditorHandler.DrawProperty(afterMinimize, customSkin, "After Minimize");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
            EditorGUILayout.PropertyField(onDestroy, new GUIContent("On Destroy"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif