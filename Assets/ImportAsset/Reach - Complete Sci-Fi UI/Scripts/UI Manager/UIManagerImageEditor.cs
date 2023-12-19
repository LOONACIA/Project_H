#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(UIManagerImage))]
    public class UIManagerImageEditor : Editor
    {
        private UIManagerImage uimiTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            uimiTarget = (UIManagerImage)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var colorType = serializedObject.FindProperty("colorType");
            var useCustomColor = serializedObject.FindProperty("useCustomColor");
            var useCustomAlpha = serializedObject.FindProperty("useCustomAlpha");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
            ReachUIEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);

            if (uimiTarget.UIManagerAsset != null)
            {
                ReachUIEditorHandler.DrawProperty(colorType, customSkin, "Color Type");
                useCustomColor.boolValue = ReachUIEditorHandler.DrawToggle(useCustomColor.boolValue, customSkin, "Use Custom Color");
                if (useCustomColor.boolValue == true) { GUI.enabled = false; }
                useCustomAlpha.boolValue = ReachUIEditorHandler.DrawToggle(useCustomAlpha.boolValue, customSkin, "Use Custom Alpha");
            }

            else { EditorGUILayout.HelpBox("UI Manager should be assigned.", MessageType.Error); }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif