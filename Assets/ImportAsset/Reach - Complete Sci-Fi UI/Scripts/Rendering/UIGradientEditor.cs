#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(UIGradient))]
    public class UIGradientEditor : Editor
    {
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var _effectGradient = serializedObject.FindProperty("_effectGradient");
            var _gradientType = serializedObject.FindProperty("_gradientType");
            var _offset = serializedObject.FindProperty("_offset");
            var _zoom = serializedObject.FindProperty("_zoom");
            var _modifyVertices = serializedObject.FindProperty("_modifyVertices");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
            ReachUIEditorHandler.DrawPropertyCW(_effectGradient, customSkin, "Gradient", 100);
            ReachUIEditorHandler.DrawPropertyCW(_gradientType, customSkin, "Type", 100);
            ReachUIEditorHandler.DrawPropertyCW(_offset, customSkin, "Offset", 100);
            ReachUIEditorHandler.DrawPropertyCW(_zoom, customSkin, "Zoom", 100);
            _modifyVertices.boolValue = ReachUIEditorHandler.DrawToggle(_modifyVertices.boolValue, customSkin, "Complex Gradient");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif