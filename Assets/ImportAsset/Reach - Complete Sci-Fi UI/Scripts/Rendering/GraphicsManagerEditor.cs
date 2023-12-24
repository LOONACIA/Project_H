#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GraphicsManager))]
    public class GraphicsManagerEditor : Editor
    {
        private GraphicsManager gmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            gmTarget = (GraphicsManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var resolutionDropdown = serializedObject.FindProperty("resolutionDropdown");

            var initializeResolutions = serializedObject.FindProperty("initializeResolutions");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
            ReachUIEditorHandler.DrawPropertyCW(resolutionDropdown, customSkin, "Resolution Dropdown", 132);

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            initializeResolutions.boolValue = ReachUIEditorHandler.DrawToggle(initializeResolutions.boolValue, customSkin, "Initialize Resolutions");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif