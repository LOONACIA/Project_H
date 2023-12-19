#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PauseMenuManager))]
    public class PauseMenuManagerEditor : Editor
    {
        private PauseMenuManager pmmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            pmmTarget = (PauseMenuManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var pauseMenuCanvas = serializedObject.FindProperty("pauseMenuCanvas");
            var continueButton = serializedObject.FindProperty("continueButton");
            var panelManager = serializedObject.FindProperty("panelManager");
            var background = serializedObject.FindProperty("background");

            var setTimeScale = serializedObject.FindProperty("setTimeScale");
            var inputBlockDuration = serializedObject.FindProperty("inputBlockDuration");
            var menuCursorState = serializedObject.FindProperty("menuCursorState");
            var gameCursorState = serializedObject.FindProperty("gameCursorState");
            var hotkey = serializedObject.FindProperty("hotkey");

            var onOpen = serializedObject.FindProperty("onOpen");
            var onClose = serializedObject.FindProperty("onClose");

            if (pmmTarget.pauseMenuCanvas != null)
            {
                ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);
                GUILayout.BeginHorizontal();

                if (Application.isPlaying == false)
                {
                    if (pmmTarget.pauseMenuCanvas.gameObject.activeSelf == false && GUILayout.Button("Show Pause Menu", customSkin.button))
                    {
                        pmmTarget.pauseMenuCanvas.gameObject.SetActive(true);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }

                    else if (pmmTarget.pauseMenuCanvas.gameObject.activeSelf == true && GUILayout.Button("Hide Pause Menu", customSkin.button))
                    {
                        pmmTarget.pauseMenuCanvas.gameObject.SetActive(false);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }

                if (GUILayout.Button("Select Object", customSkin.button)) { Selection.activeObject = pmmTarget.pauseMenuCanvas; }
                GUILayout.EndHorizontal();
            }

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 10);
            ReachUIEditorHandler.DrawProperty(pauseMenuCanvas, customSkin, "Pause Canvas");
            ReachUIEditorHandler.DrawProperty(continueButton, customSkin, "Continue Button");
            ReachUIEditorHandler.DrawProperty(panelManager, customSkin, "Panel Manager");
            ReachUIEditorHandler.DrawProperty(background, customSkin, "Background");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 10);
            setTimeScale.boolValue = ReachUIEditorHandler.DrawToggle(setTimeScale.boolValue, customSkin, "Set Time Scale", "Sets the time scale depending on the pause menu state.");
            ReachUIEditorHandler.DrawProperty(inputBlockDuration, customSkin, "Input Block Duration", "Block input in specific amount of time to provide smooth visuals.");
            ReachUIEditorHandler.DrawProperty(menuCursorState, customSkin, "Menu Cursor State");
            ReachUIEditorHandler.DrawProperty(gameCursorState, customSkin, "Game Cursor State");
            EditorGUILayout.PropertyField(hotkey, new GUIContent("Hotkey"), true);

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
            EditorGUILayout.PropertyField(onOpen, new GUIContent("On Open"), true);
            EditorGUILayout.PropertyField(onClose, new GUIContent("On Close"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif