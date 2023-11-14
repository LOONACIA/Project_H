/* ================================================================
   ----------------------------------------------------------------
   Project   :   AI Tree
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2023 Renowned Games All rights reserved.
   ================================================================ */

using RenownedGames.AITree;
using UnityEditor;
using UnityEngine;

namespace RenownedGames.AITreeEditor
{
    [CustomEditor(typeof(Blackboard))]
    [CanEditMultipleObjects]
    public sealed class BlackboardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Press Enter or double click to open.", EditorStyles.centeredGreyMiniLabel);
        }
    }
}
