#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FeedNotification))]
    public class FeedNotificationHeader : Editor
    {
        private FeedNotification fnTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            fnTarget = (FeedNotification)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var icon = serializedObject.FindProperty("icon");
            var notificationText = serializedObject.FindProperty("notificationText");
            var localizationKey = serializedObject.FindProperty("localizationKey");

            var itemAnimator = serializedObject.FindProperty("itemAnimator");
            var iconObj = serializedObject.FindProperty("iconObj");
            var textObj = serializedObject.FindProperty("textObj");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var updateOnAnimate = serializedObject.FindProperty("updateOnAnimate");
            var minimizeAfter = serializedObject.FindProperty("minimizeAfter");
            var defaultState = serializedObject.FindProperty("defaultState");
            var afterMinimize = serializedObject.FindProperty("afterMinimize");

            var onDestroy = serializedObject.FindProperty("onDestroy");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
            ReachUIEditorHandler.DrawProperty(icon, customSkin, "Icon");
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Notification Text"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
            EditorGUILayout.PropertyField(notificationText, new GUIContent(""), GUILayout.Height(70));
            GUILayout.EndHorizontal();
            ReachUIEditorHandler.DrawProperty(localizationKey, customSkin, "Localization Key");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 10);
            ReachUIEditorHandler.DrawProperty(itemAnimator, customSkin, "Animator");
            ReachUIEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
            ReachUIEditorHandler.DrawProperty(textObj, customSkin, "Text Object");

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