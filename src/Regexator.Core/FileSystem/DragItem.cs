// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator.FileSystem
{
    internal sealed class DragItem
    {
        private DragItem(TreeNode node, NodeKind kind)
        {
            Node = node;
            Kind = kind;
        }

        public static DragItem Create(ItemDragEventArgs e)
        {
            if (e.Item is FileSystemNode node
                && (node.Kind == NodeKind.Directory || node.Kind == NodeKind.Project || node.Kind == NodeKind.FileInput))
            {
                return new DragItem(node, node.Kind);
            }

            if (e.Item is InputNode inputNode)
                return new DragItem(inputNode, NodeKind.Input);

            return null;
        }

        public TreeNode Node { get; }

        public NodeKind Kind { get; }
    }
}
