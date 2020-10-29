// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    public static class NodeFactory
    {
        public static IEnumerable<TreeNode> CreateProjectNodes(
            string path,
            Func<FileSystemInfo, bool> infoPredicate,
            Func<FileSystemNode, bool> nodePredicate)
        {
            return CreateProjectNodes(
                IO.FileSystem.EnumerateFileInfos(
                    new DirectoryInfo(path),
                    FileSystemManager.ProjectSearchPattern,
                    SearchOption.TopDirectoryOnly)
                    .Where(f => infoPredicate(f)),
                nodePredicate);
        }

        public static TreeNode CreateProjectNode(FileInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return CreateProjectNodes(new FileInfo[] { info }, _ => true).FirstOrDefault();
        }

        private static IEnumerable<TreeNode> CreateProjectNodes(FileInfo[] files, Func<FileSystemNode, bool> nodePredicate)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            return CreateProjectNodes(files.AsEnumerable(), nodePredicate);
        }

        private static IEnumerable<TreeNode> CreateProjectNodes(
            IEnumerable<FileInfo> files,
            Func<FileSystemNode, bool> predicate)
        {
            foreach (FileInfo fi in files)
            {
                ProjectNode projectNode = null;
                TreeNode node = null;
                try
                {
                    projectNode = new ProjectNode(fi);
                }
                catch (Exception ex) when (ex is InvalidOperationException
                    || ex is System.Xml.XmlException
                    || Executor.IsFileSystemException(ex))
                {
                    node = new InvalidProjectNode(fi.FullName);
                }

                if (node != null)
                {
                    yield return node;
                }
                else if (projectNode != null && predicate(projectNode))
                {
                    foreach (TreeNode inputNode in CreateInputNodes(projectNode.Project))
                        projectNode.Nodes.Add(inputNode);

                    yield return projectNode;
                }
            }
        }

        internal static IEnumerable<TreeNode> CreateInputNodes(Project project)
        {
            foreach (TreeNode node in CreateInputNodes(project.FileInputs))
                yield return node;

            foreach (Input input in project.Inputs)
                yield return new InputNode(input);
        }

        internal static IEnumerable<TreeNode> CreateInputNodes(IEnumerable<Input> inputs)
        {
            foreach (Input input in inputs.Where(f => f.Name != null))
            {
                TreeNode node = null;
                try
                {
                    var info = new FileInfo(input.Name);
                    node = new FileInputNode(info, input);
                }
                catch (Exception ex) when (Executor.IsFileSystemException(ex))
                {
                    node = new InvalidInputNode(input.Name, input);
                }

                if (node != null)
                    yield return node;
            }
        }
    }
}
