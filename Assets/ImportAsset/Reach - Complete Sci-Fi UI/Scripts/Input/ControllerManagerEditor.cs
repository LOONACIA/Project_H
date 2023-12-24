#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ControllerManager))]
    public class ControllerManagerEditor : Editor
    {
        private ControllerManager cmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            cmTarget = (ControllerManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var presetManager = serializedObject.FindProperty("presetManager");
            var firstSelected = serializedObject.FindProperty("firstSelected");
            var gamepadObjects = serializedObject.FindProperty("gamepadObjects");
            var keyboardObjects = serializedObject.FindProperty("keyboardObjects");

            var alwaysUpdate = serializedObject.FindProperty("alwaysUpdate");
            var affectCursor = serializedObject.FindProperty("affectCursor");
            var gamepadHotkey = serializedObject.FindProperty("gamepadHotkey");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
            ReachUIEditorHandler.DrawProperty(presetManager, customSkin, "Preset Manager");
            ReachUIEditorHandler.DrawProperty(firstSelected, customSkin, "First Selected", "UI element to be selected first in the home panel (e.g. Play button).");

            GUILayout.BeginVertical();
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(gamepadObjects, new GUIContent("Gamepad Objects"), true);
            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(keyboardObjects, new GUIContent("Keyboard Objects"), true);
            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            alwaysUpdate.boolValue = ReachUIEditorHandler.DrawToggle(alwaysUpdate.boolValue, customSkin, "Always Update");
            affectCursor.boolValue = ReachUIEditorHandler.DrawToggle(affectCursor.boolValue, customSkin, "Affect Cursor", "Changes the cursor state depending on the controller state.");
            EditorGUILayout.PropertyField(gamepadHotkey, new GUIContent("Gamepad Hotkey", "Triggers to switch to gamepad when pressed."), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif