#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(ModalWindowManager))]
    public class ModalWindowManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ModalWindowManager mwTarget;
        private int currentTab;

        private void OnEnable()
        {
            mwTarget = (ModalWindowManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachUIEditorHandler.DrawComponentHeader(customSkin, "TopHeader_ModalWIndow");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = ReachUIEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab_Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab_Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab_Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var windowIcon = serializedObject.FindProperty("windowIcon");
            var windowTitle = serializedObject.FindProperty("windowTitle");
            var windowDescription = serializedObject.FindProperty("windowDescription");

            var titleKey = serializedObject.FindProperty("titleKey");
            var descriptionKey = serializedObject.FindProperty("descriptionKey");

            var onConfirm = serializedObject.FindProperty("onConfirm");
            var onCancel = serializedObject.FindProperty("onCancel");
            var onOpen = serializedObject.FindProperty("onOpen");
            var onClose = serializedObject.FindProperty("onClose");

            var icon = serializedObject.FindProperty("icon");
            var titleText = serializedObject.FindProperty("titleText");
            var descriptionText = serializedObject.FindProperty("descriptionText");
            var confirmButton = serializedObject.FindProperty("confirmButton");
            var cancelButton = serializedObject.FindProperty("cancelButton");
            var mwAnimator = serializedObject.FindProperty("mwAnimator");

            var closeBehaviour = serializedObject.FindProperty("closeBehaviour");
            var startBehaviour = serializedObject.FindProperty("startBehaviour");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");
            var closeOnCancel = serializedObject.FindProperty("closeOnCancel");
            var closeOnConfirm = serializedObject.FindProperty("closeOnConfirm");
            var showCancelButton = serializedObject.FindProperty("showCancelButton");
            var showConfirmButton = serializedObject.FindProperty("showConfirmButton");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var animationSpeed = serializedObject.FindProperty("animationSpeed");

            switch (currentTab)
            {
                case 0:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Content", 6);

                    if (useCustomContent.boolValue == false)
                    {
                        if (mwTarget.windowIcon != null) 
                        {
                            ReachUIEditorHandler.DrawProperty(icon, customSkin, "Icon");
                            if (Application.isPlaying == false) { mwTarget.windowIcon.sprite = mwTarget.icon; }
                        }

                        if (mwTarget.windowTitle != null) 
                        {
                            ReachUIEditorHandler.DrawProperty(titleText, customSkin, "Title");
                            if (Application.isPlaying == false) { mwTarget.windowTitle.text = titleText.stringValue; }
                        }

                        if (mwTarget.windowDescription != null) 
                        {
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(new GUIContent("Description"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
                            EditorGUILayout.PropertyField(descriptionText, new GUIContent(""), GUILayout.Height(70));
                            GUILayout.EndHorizontal();
                            if (Application.isPlaying == false) { mwTarget.windowDescription.text = descriptionText.stringValue; }
                        }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled.", MessageType.Info); }

                    GUILayout.BeginHorizontal();
                    if (mwTarget.showConfirmButton == true && mwTarget.confirmButton != null && GUILayout.Button("Edit Confirm Button", customSkin.button)) { Selection.activeObject = mwTarget.confirmButton; }
                    if (mwTarget.showCancelButton == true && mwTarget.cancelButton != null && GUILayout.Button("Edit Cancel Button", customSkin.button)) { Selection.activeObject = mwTarget.cancelButton; }
                    GUILayout.EndHorizontal();

                    if (Application.isPlaying == false)
                    {
                        if (mwTarget.GetComponent<CanvasGroup>().alpha == 0 && GUILayout.Button("Set Visible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        else if (mwTarget.GetComponent<CanvasGroup>().alpha == 1 && GUILayout.Button("Set Invisible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    if (mwTarget.useCustomContent == false && mwTarget.useLocalization == true)
                    {
                        ReachUIEditorHandler.DrawHeader(customSkin, "Header_Languages", 10);
                        ReachUIEditorHandler.DrawProperty(titleKey, customSkin, "Title Key", "Used for localization.");
                        ReachUIEditorHandler.DrawProperty(descriptionKey, customSkin, "Description Key", "Used for localization.");
                    }

                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Events", 10);
                    EditorGUILayout.PropertyField(onConfirm, new GUIContent("On Confirm"), true);
                    EditorGUILayout.PropertyField(onCancel, new GUIContent("On Cancel"), true);
                    EditorGUILayout.PropertyField(onOpen, new GUIContent("On Open"), true);
                    EditorGUILayout.PropertyField(onClose, new GUIContent("On Close"), true);
                    break;

                case 1:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Resources", 6);
                    ReachUIEditorHandler.DrawProperty(windowIcon, customSkin, "Icon Object");
                    ReachUIEditorHandler.DrawProperty(windowTitle, customSkin, "Title Object");
                    ReachUIEditorHandler.DrawProperty(windowDescription, customSkin, "Description Object");
                    ReachUIEditorHandler.DrawProperty(confirmButton, customSkin, "Confirm Button");
                    ReachUIEditorHandler.DrawProperty(cancelButton, customSkin, "Cancel Button");
                    ReachUIEditorHandler.DrawProperty(mwAnimator, customSkin, "Animator");
                    break;

                case 2:
                    ReachUIEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
                    ReachUIEditorHandler.DrawProperty(animationSpeed, customSkin, "Animation Speed");
                    ReachUIEditorHandler.DrawProperty(startBehaviour, customSkin, "Start Behaviour");
                    ReachUIEditorHandler.DrawProperty(closeBehaviour, customSkin, "Close Behaviour");
                    useCustomContent.boolValue = ReachUIEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content", "Bypasses inspector values and allows manual editing.");
                    closeOnCancel.boolValue = ReachUIEditorHandler.DrawToggle(closeOnCancel.boolValue, customSkin, "Close Window On Cancel");
                    closeOnConfirm.boolValue = ReachUIEditorHandler.DrawToggle(closeOnConfirm.boolValue, customSkin, "Close Window On Confirm");
                    showCancelButton.boolValue = ReachUIEditorHandler.DrawToggle(showCancelButton.boolValue, customSkin, "Show Cancel Button");
                    showConfirmButton.boolValue = ReachUIEditorHandler.DrawToggle(showConfirmButton.boolValue, customSkin, "Show Confirm Button");
                    useLocalization.boolValue = ReachUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif