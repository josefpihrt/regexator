// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class TreeViewExtensions
    {
        public static void SelectedNodeExpandAll(this TreeView trv)
        {
            SelectedNodeExpandAll(trv, false);
        }

        public static void SelectedNodeExpandAll(this TreeView trv, bool suppressUpdate)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            TreeNode node = trv.SelectedNode;
            node.ExpandAll(suppressUpdate);
            node.EnsureVisible();
        }

        public static void SelectedNodeCollapseAll(this TreeView trv)
        {
            SelectedNodeCollapseAll(trv, false);
        }

        public static void SelectedNodeCollapseAll(this TreeView trv, bool suppressUpdate)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            TreeNode node = trv.SelectedNode;
            node.CollapseAll(suppressUpdate);
            node.EnsureVisible();
        }

        public static IEnumerable<TreeNode> EnumerateAllNodes(this TreeView trv)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            return EnumerateAllNodes();

            IEnumerable<TreeNode> EnumerateAllNodes()
            {
                foreach (TreeNode node in trv.Nodes.Cast<TreeNode>().SelectMany(f => f.EnumerateAllNodes(true)))
                    yield return node;
            }
        }

        public static bool IsMouseOnPlusMinus(this TreeView trv)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            return (trv.HitTest(trv.PointToClient(Control.MousePosition)).Location
                & TreeViewHitTestLocations.PlusMinus) == TreeViewHitTestLocations.PlusMinus;
        }

        public static TreeNode FindFirst(this TreeView trv, string key)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            return trv.Nodes.Find(key, true).FirstOrDefault();
        }

        public static TreeNode FirstNodeOrDefault(this TreeView trv)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            if (trv.Nodes.Count > 0)
                return trv.Nodes[0];

            return null;
        }

        public static bool IsFirstNodeSelected(this TreeView trv)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            return trv.Nodes.Count > 0 && trv.Nodes[0].IsSelected;
        }

        public static void TrySelectFirstNode(this TreeView trv)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            trv.TrySelectFirstNode(false);
        }

        public static void TrySelectFirstNode(this TreeView trv, bool recursive)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            TreeNode node = trv.FirstNodeOrDefault();
            if (node != null)
            {
                if (recursive)
                {
                    while (node.FirstNode != null)
                        node = node.FirstNode;
                }

                trv.SelectedNode = node;
            }
        }

        public static void TryExpandFirst(this TreeView trv)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            TreeNode node = trv.FirstNodeOrDefault();

            node?.Expand();
        }

        public static void ExpandAll(this TreeView trv, bool suppressUpdate)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            if (suppressUpdate)
            {
                trv.BeginUpdate();
                trv.ExpandAll();
                trv.EndUpdate();
            }
            else
            {
                trv.ExpandAll();
            }
        }

        public static void CollapseAll(this TreeView trv, bool suppressUpdate)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            if (suppressUpdate)
                trv.BeginUpdate();

            if (trv.Nodes.Count == 1)
            {
                foreach (TreeNode node in trv.Nodes[0].Nodes)
                    node.Collapse(false);
            }
            else
            {
                trv.CollapseAll();
            }

            if (suppressUpdate)
                trv.EndUpdate();
        }

        public static void RootNodesCollapseAll(this TreeView trv)
        {
            RootNodesCollapseAll(trv, false);
        }

        public static void RootNodesCollapseAll(this TreeView trv, bool suppressUpdate)
        {
            if (trv == null)
                throw new ArgumentNullException(nameof(trv));

            if (suppressUpdate)
                trv.BeginUpdate();

            foreach (TreeNode node in trv.Nodes)
                node.Collapse(false);

            if (suppressUpdate)
                trv.EndUpdate();
        }
    }
}
