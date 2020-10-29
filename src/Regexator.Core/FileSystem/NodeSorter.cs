// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    public class NodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is DirectoryNode directoryNode)
                return CompareNode(directoryNode, y);

            if (x is ProjectNode projectNode)
                return CompareNode(projectNode, y);

            if (x is InvalidProjectNode invalidProjectNode)
                return CompareNode(invalidProjectNode, y);

            if (x is FileInputNode fileNode)
                return CompareNode(fileNode, y);

            if (x is InvalidInputNode invalidInputNode)
                return CompareNode(invalidInputNode, y);

            if (x is InputNode inputNode)
                return CompareNode(inputNode, y);

            Debug.Fail("");
            return 0;
        }

        private static int CompareNode(DirectoryNode node, object obj)
        {
            if (obj is ProjectNode || obj is InvalidProjectNode)
                return -1;

            if (obj is DirectoryNode directoryNode)
                return CompareText(node, directoryNode);

            Debug.Fail("");
            return 0;
        }

        private static int CompareNode(ProjectNode node, object obj)
        {
            if (obj is DirectoryNode)
                return 1;

            if (obj is ProjectNode projectNode)
                return CompareText(node, projectNode);

            if (obj is InvalidProjectNode invalidNode)
                return CompareText(node, invalidNode);

            Debug.Fail("");
            return 0;
        }

        private static int CompareNode(InvalidProjectNode node, object obj)
        {
            if (obj is DirectoryNode)
                return 1;

            if (obj is ProjectNode projectNode)
                return CompareText(node, projectNode);

            if (obj is InvalidProjectNode invalidNode)
                return CompareText(node, invalidNode);

            Debug.Fail("");
            return 0;
        }

        private static int CompareNode(FileInputNode node, object obj)
        {
            if (obj is InputNode)
                return 1;

            if (obj is FileInputNode inputNode)
                return CompareText(node, inputNode);

            if (obj is InvalidInputNode invalidNode)
                return CompareText(node, invalidNode);

            Debug.Fail("");
            return 0;
        }

        private static int CompareNode(InputNode node, object obj)
        {
            if (obj is FileInputNode || obj is InvalidInputNode)
                return -1;

            if (obj is InputNode inputNode)
                return CompareText(node, inputNode);

            Debug.Fail("");
            return 0;
        }

        private static int CompareNode(InvalidInputNode node, object obj)
        {
            if (obj is InputNode)
                return 1;

            if (obj is FileInputNode inputNode)
                return CompareText(node, inputNode);

            if (obj is InvalidInputNode invalidNode)
                return CompareText(node, invalidNode);

            Debug.Fail("");
            return 0;
        }

        private static int CompareText(TreeNode node1, TreeNode node2)
        {
            return string.Compare(node1.Text, node2.Text, StringComparison.CurrentCulture);
        }
    }
}
