#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIManagerAudio))]
    public class UIManagerAudioEditor : Editor
    {
        private UIManagerAudio uimaTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            uimaTarget = (UIManagerAudio)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var audioMixer = serializedObject.FindProperty("audioMixer");
            var audioSource = serializedObject.FindProperty("audioSource");
            var masterSlider = serializedObject.FindProperty("masterSlider");
            var musicSlider = serializedObject.FindProperty("musicSlider");
            var SFXSlider = serializedObject.FindProperty("SFXSlider");
            var UISlider = serializedObject.FindProperty("UISlider");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
            ReachUIEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");
            ReachUIEditorHandler.DrawProperty(audioMixer, customSkin, "Audio Mixer");
            ReachUIEditorHandler.DrawProperty(audioSource, customSkin, "Audio Source");
            ReachUIEditorHandler.DrawProperty(masterSlider, customSkin, "Master Slider");
            ReachUIEditorHandler.DrawProperty(musicSlider, customSkin, "Music Slider");
            ReachUIEditorHandler.DrawProperty(SFXSlider, customSkin, "SFX Slider");
            ReachUIEditorHandler.DrawProperty(UISlider, customSkin, "UI Slider");

            if (Application.isPlaying == true)
                return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif