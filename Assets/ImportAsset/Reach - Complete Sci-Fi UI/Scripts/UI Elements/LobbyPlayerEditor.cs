#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LobbyPlayer))]
    public class LobbyPlayerEditor : Editor
    {
        private LobbyPlayer lpTarget;
        private GUISkin customSkin;
        public bool showResources;

        private void OnEnable()
        {
            lpTarget = (LobbyPlayer)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var playerPicture = serializedObject.FindProperty("playerPicture");
            var playerName = serializedObject.FindProperty("playerName");
            var additionalText = serializedObject.FindProperty("additionalText");
            var currentState = serializedObject.FindProperty("currentState");

            var emptyParent = serializedObject.FindProperty("emptyParent");
            var readyParent = serializedObject.FindProperty("readyParent");
            var notReadyParent = serializedObject.FindProperty("notReadyParent");
            var playerIndicatorReady = serializedObject.FindProperty("playerIndicatorReady");
            var playerIndicatorNotReady = serializedObject.FindProperty("playerIndicatorNotReady");
            var pictureReadyImg = serializedObject.FindProperty("pictureReadyImg");
            var pictureNotReadyImg = serializedObject.FindProperty("pictureNotReadyImg");
            var nameReadyTMP = serializedObject.FindProperty("nameReadyTMP");
            var nameNotReadyTMP = serializedObject.FindProperty("nameNotReadyTMP");
            var adtReadyTMP = serializedObject.FindProperty("adtReadyTMP");
            var adtNotReadyTMP = serializedObject.FindProperty("adtNotReadyTMP");

            var onEmpty = serializedObject.FindProperty("onEmpty");
            var onReady = serializedObject.FindProperty("onReady");
            var onUnready = serializedObject.FindProperty("onUnready");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
            ReachUIEditorHandler.DrawProperty(playerPicture, customSkin, "Player Picture");
            ReachUIEditorHandler.DrawProperty(playerName, customSkin, "Player Name");
            ReachUIEditorHandler.DrawProperty(additionalText, customSkin, "Additional Text");
            ReachUIEditorHandler.DrawProperty(currentState, customSkin, "Current State");

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 10);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            showResources = GUILayout.Toggle(showResources, new GUIContent("Show Resources", "Current state: " + showResources.ToString()), customSkin.FindStyle("Toggle"));
            showResources = GUILayout.Toggle(showResources, new GUIContent("", "Current state: " + showResources.ToString()), customSkin.FindStyle("ToggleHelper"));
            GUILayout.EndHorizontal();

            if (showResources == true)
            {
                ReachUIEditorHandler.DrawProperty(emptyParent, customSkin, "Empty Parent");
                ReachUIEditorHandler.DrawProperty(readyParent, customSkin, "Ready Parent");
                ReachUIEditorHandler.DrawProperty(notReadyParent, customSkin, "Not Ready Parent");
                ReachUIEditorHandler.DrawProperty(playerIndicatorReady, customSkin, "Indicator Ready");
                ReachUIEditorHandler.DrawProperty(playerIndicatorNotReady, customSkin, "Indicator Not Ready");
                ReachUIEditorHandler.DrawProperty(pictureReadyImg, customSkin, "Picture Ready");
                ReachUIEditorHandler.DrawProperty(pictureNotReadyImg, customSkin, "Picture Not Ready");
                ReachUIEditorHandler.DrawProperty(nameReadyTMP, customSkin, "Name Ready");
                ReachUIEditorHandler.DrawProperty(nameNotReadyTMP, customSkin, "Name Not Ready");
                ReachUIEditorHandler.DrawProperty(adtReadyTMP, customSkin, "Adt. Ready");
                ReachUIEditorHandler.DrawProperty(adtNotReadyTMP, customSkin, "Adt. Not Ready");
            }

            ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
            EditorGUILayout.PropertyField(onEmpty, new GUIContent("On Empty"), true);
            EditorGUILayout.PropertyField(onReady, new GUIContent("On Ready"), true);
            EditorGUILayout.PropertyField(onUnready, new GUIContent("On Unready"), true);

            if (Application.isPlaying == false) { Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif