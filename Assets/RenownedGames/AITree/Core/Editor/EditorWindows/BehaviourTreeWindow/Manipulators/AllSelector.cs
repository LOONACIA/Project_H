/* ================================================================
   ----------------------------------------------------------------
   Project   :   AI Tree
   Publisher :   Renowned Games
   Developer :   Zinnur Davleev
   ----------------------------------------------------------------
   Copyright 2022-2023 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RenownedGames.AITreeEditor
{
    public class AllSelector : Manipulator
    {
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (target == null) return;

            if (evt.modifiers == EventModifiers.Control && evt.keyCode == KeyCode.A)
            {
                BehaviourTreeGraph graph = evt.target as BehaviourTreeGraph;
                if (graph == null) return;

                graph.ClearSelection();
                foreach (GraphElement element in graph.graphElements)
                {
                    if (element is ISelectable selectable && element is not Edge)
                    {
                        graph.AddToSelection(selectable);
                    }
                }
            }
        }

        #region [IManipulator Implementation]
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }
        #endregion
    }
}