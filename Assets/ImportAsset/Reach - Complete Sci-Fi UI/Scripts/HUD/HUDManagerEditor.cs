#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HUDManager))]
    public class HUDManagerEditor : Editor
    {
        private HUDManager hmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            hmTarget = (HUDManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var HUDPanel = serializedObject.FindProperty("HUDPanel");

            var fadeSpeed = serializedObject.FindProperty("fadeSpeed");
            var defaultBehaviour = serializedObject.FindProperty("defaultBehaviour");

            var onSetVisible = serializedObject.FindProperty("onSetVisible");
            var onSetInvisible = serializedObject.FindProperty("onSetInvisible");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
            ReachUIEditorHandler.DrawProperty(HUDPanel, customSkin, "HUD Panel");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            ReachUIEditorHandler.DrawProperty(fadeSpeed, customSkin, "Fade Speed", "Sets the fade animation speed.");
            ReachUIEditorHandler.DrawProperty(defaultBehaviour, customSkin, "Default Behaviour");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
            EditorGUILayout.PropertyField(onSetVisible, new GUIContent("On Set Visible"), true);
            EditorGUILayout.PropertyField(onSetInvisible, new GUIContent("On Set Invisible"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif