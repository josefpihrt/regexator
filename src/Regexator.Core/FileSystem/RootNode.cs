// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public sealed class RootNode : FileSystemNode, INode
    {
        public RootNode(DirectoryInfo info)
            : base(info)
        {
            DirectoryInfo = info;
            SetImage(NodeImageIndex.Directory);
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            Name = FullName;
            Text = FileName;
            ToolTipText = FullName;
        }

        public void LoadNodes(Func<FileSystemInfo, bool> infoPredicate, Func<FileSystemNode, bool> nodePredicate)
        {
            if (infoPredicate == null)
                throw new ArgumentNullException(nameof(infoPredicate));

            if (nodePredicate == null)
                throw new ArgumentNullException(nameof(nodePredicate));

            Nodes.Clear();
            AddDirectoryNodes(infoPredicate, nodePredicate);
            AddProjectNodes(infoPredicate, nodePredicate);
        }

        public void AddProjectNodes(Func<FileSystemInfo, bool> infoPredicate, Func<FileSystemNode, bool> nodePredicate)
        {
            foreach (TreeNode item in NodeFactory.CreateProjectNodes(FullName, infoPredicate, nodePredicate))
                Nodes.Add(item);
        }

        public void AddDirectoryNodes(Func<FileSystemInfo, bool> infoPredicate, Func<FileSystemNode, bool> nodePredicate)
        {
            foreach (DirectoryNode node in DirectoryInfo
                .EnumerateDirectories()
                .Where(f => infoPredicate(f))
                .Select(f => new DirectoryNode(f))
                .Where(f => nodePredicate(f)))
            {
                Nodes.Add(node);
            }
        }

        internal DirectoryNode AddDirectoryNode(string dirName)
        {
            string dirPath = Path.Combine(FullName, dirName);
            DirectoryInfo dirInfo = Executor.CreateDirectory(dirPath, true);
            if (dirInfo != null)
            {
                var directoryNode = new DirectoryNode(dirInfo);
                Nodes.Add(directoryNode);
                return directoryNode;
            }

            return null;
        }

        internal ProjectNode AddProjectNode(FileInfo info)
        {
            TreeNode node = NodeFactory.CreateProjectNode(info);
            RemoveInvalidProject(info.FullName);
            Nodes.Add(node);
            return node as ProjectNode;
        }

        internal ProjectNode AddProjectNode(FileInfo info, Project project)
        {
            var node = new ProjectNode(info, project);
            RemoveInvalidProject(info.FullName);
            Nodes.Add(node);
            return node;
        }

        private void RemoveInvalidProject(string path)
        {
            InvalidProjectNode node = Nodes.OfType<InvalidProjectNode>()
                .FirstOrDefault(f => FileSystemUtility.PathEquals(f.Path, path));
            node?.Remove();
        }

        public DirectoryNode FindDirectoryNode(string dirPath)
        {
            if (!string.IsNullOrEmpty(dirPath))
                return EnumerateDirectoryNodes().FirstOrDefault(f => FileSystemUtility.PathEquals(f.FullName, dirPath));

            return null;
        }

        public ProjectNode FindProjectNode(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
                return EnumerateProjectNodes().FirstOrDefault(f => FileSystemUtility.PathEquals(f.FullName, filePath));

            return null;
        }

        public DirectoryNode[] GetDirectoryNodes()
        {
            return EnumerateDirectoryNodes().ToArray();
        }

        public IEnumerable<DirectoryNode> EnumerateDirectoryNodes()
        {
            return Nodes.OfType<DirectoryNode>();
        }

        public ProjectNode[] GetProjectNodes()
        {
            return EnumerateProjectNodes().ToArray();
        }

        public IEnumerable<ProjectNode> EnumerateProjectNodes()
        {
            return Nodes.OfType<ProjectNode>();
        }

        public IEnumerable<FileSystemNode> EnumerateDirectoryAndProjectNodes()
        {
            return Nodes
                .Cast<TreeNode>()
                .Where(f => f is DirectoryNode || f is ProjectNode)
                .Cast<FileSystemNode>();
        }

        public override RootNode GetRootNode()
        {
            return this;
        }

        public override DirectoryInfo ParentDirectory
        {
            get { return DirectoryInfo.Parent; }
        }

        public override DirectoryInfo DirectoryInfo { get; }

        public override NodeKind Kind
        {
            get { return NodeKind.Root; }
        }
    }
}