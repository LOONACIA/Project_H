#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalizationManager))]
    public class LocalizationManagerEditor : Editor
    {
        private LocalizationManager lmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            lmTarget = (LocalizationManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var languageSelector = serializedObject.FindProperty("languageSelector");

            var setLanguageOnAwake = serializedObject.FindProperty("setLanguageOnAwake");
            var updateItemsOnSet = serializedObject.FindProperty("updateItemsOnSet");
            var saveLanguageChanges = serializedObject.FindProperty("saveLanguageChanges");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
            ReachUIEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");
            ReachUIEditorHandler.DrawProperty(languageSelector, customSkin, "Language Selector");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            setLanguageOnAwake.boolValue = ReachUIEditorHandler.DrawToggle(setLanguageOnAwake.boolValue, customSkin, "Set Language On Awake");
            updateItemsOnSet.boolValue = ReachUIEditorHandler.DrawToggle(updateItemsOnSet.boolValue, customSkin, "Update Items On Language Set");
            saveLanguageChanges.boolValue = ReachUIEditorHandler.DrawToggle(saveLanguageChanges.boolValue, customSkin, "Save Language Changes");
            LocalizationManager.enableLogs = ReachUIEditorHandler.DrawToggle(LocalizationManager.enableLogs, customSkin, "Enable Logs");

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif