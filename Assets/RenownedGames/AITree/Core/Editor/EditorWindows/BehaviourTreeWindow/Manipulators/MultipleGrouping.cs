/* ================================================================
   ----------------------------------------------------------------
   Project   :   AI Tree
   Publisher :   Renowned Games
   Developer :   Zinnur Davleev
   ----------------------------------------------------------------
   Copyright 2022-2023 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RenownedGames.AITreeEditor
{
    public class MultipleGrouping : Manipulator
    {
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (target == null) return;

            if (evt.keyCode == KeyCode.G && evt.modifiers == EventModifiers.Control)
            {
                BehaviourTreeGraph graph = evt.target as BehaviourTreeGraph;
                if (graph == null) return;

                List<WrapView> selectedViews = graph.selection.OfType<WrapView>().ToList();
                if (selectedViews.Count == 0) return;

                HashSet<GroupView> affectedGroups = new HashSet<GroupView>();
                foreach (WrapView wrapView in selectedViews)
                {
                    GroupView groupView = wrapView.GetGroup();
                    if (groupView != null)
                    {
                        affectedGroups.Add(groupView);
                    }
                }

                if (affectedGroups.Count == 1)
                {
                    GroupView groupView = affectedGroups.First();
                    List<GraphElement> groupElements = groupView.containedElements.ToList();

                    if (groupElements.Count == selectedViews.Count &&
                        selectedViews.All(s => groupElements.Contains(s)))
                    {
                        return;
                    }
                }

                {
                    GroupView groupView = graph.CreateGroup(Vector2.zero);
                    groupView.AddElements(selectedViews);

                    if (affectedGroups.Count > 0)
                    {
                        List<GroupView> groupsToDelete = new List<GroupView>();
                        foreach (GroupView affectedGroup in affectedGroups)
                        {
                            if (affectedGroup.containedElements.Count() == 0)
                            {
                                groupsToDelete.Add(affectedGroup);
                            }
                        }
                        graph.DeleteElements(groupsToDelete);
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