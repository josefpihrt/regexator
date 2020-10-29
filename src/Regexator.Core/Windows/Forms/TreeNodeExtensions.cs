// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class TreeNodeExtensions
    {
        public static TreeNode Parent(this TreeNode node, int levels)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (levels < 1)
                throw new ArgumentOutOfRangeException(nameof(levels));

            TreeNode parent = node.Parent;

            for (int i = 1; i < levels && parent != null; i++)
                parent = parent.Parent;

            return parent;
        }

        public static IEnumerable<TreeNode> EnumerateAllNodes(this TreeNode node, bool include)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return EnumerateAllNodes();

            IEnumerable<TreeNode> EnumerateAllNodes()
            {
                if (include)
                    yield return node;

                foreach (TreeNode item in node.Nodes.Cast<TreeNode>())
                {
                    foreach (TreeNode item2 in item.EnumerateAllNodes(true))
                        yield return item2;
                }
            }
        }

        [DebuggerStepThrough]
        public static TreeNode NextOrPreviousNode(this TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.NextNode ?? node.PrevNode;
        }

        [DebuggerStepThrough]
        public static void ExpandAll(this TreeNode node, bool suppressUpdate)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (suppressUpdate)
            {
                TreeView trv = node.TreeView;

                trv?.BeginUpdate();

                node.ExpandAll();

                trv?.EndUpdate();
            }
            else
            {
                node.ExpandAll();
            }
        }

        [DebuggerStepThrough]
        public static void CollapseAll(this TreeNode node)
        {
            CollapseAll(node, false);
        }

        [DebuggerStepThrough]
        public static void CollapseAll(this TreeNode node, bool suppressUpdate)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (suppressUpdate)
            {
                TreeView trv = node.TreeView;

                trv?.BeginUpdate();

                node.Collapse(false);

                trv?.EndUpdate();
            }
            else
            {
                node.Collapse(false);
            }
        }

        public static bool IsSubNodeOf(this TreeNode node, TreeNode parent)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            TreeNode n = node.Parent;
            while (n != null)
            {
                if (n.Equals(parent))
                    return true;

                n = n.Parent;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static TreeNode GetRootNode(this TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            while (node.Parent != null)
                node = node.Parent;

            return node;
        }

        [DebuggerStepThrough]
        public static TreeNode[] GetParentNodes(this TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var lst = new List<TreeNode>();
            node = node.Parent;

            while (node != null)
            {
                lst.Add(node);
                node = node.Parent;
            }

            lst.Reverse();
            return lst.ToArray();
        }
    }
}
