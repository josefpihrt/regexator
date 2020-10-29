// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    public sealed class ProjectNode : FileSystemNode, INode, IClipboardNode
    {
        public ProjectNode(FileInfo info)
            : base(info)
        {
            FileInfo = info;
            SetImage(NodeImageIndex.Project);
            UpdateProperties();
            Project = Xml.Serialization.Projects.ProjectSerializer.Default.Deserialize(FullName);
        }

        public ProjectNode(FileInfo info, Project project)
            : base(info)
        {
            FileInfo = info;
            SetImage(NodeImageIndex.Project);
            UpdateProperties();
            Project = project;
        }

        private void UpdateProperties()
        {
            Name = FullName;
            Text = FileNameWithoutExtension;
        }

        public void LoadInputNodes()
        {
            Nodes.Clear();
            foreach (TreeNode node in NodeFactory.CreateInputNodes(Project))
                Nodes.Add(node);
        }

        public FileInputNode AddFileInputNode(FileInfo info, Input input)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var node = new FileInputNode(info, input);
            RemoveInvalidInput(info.FullName);
            Nodes.Add(node);
            Project.FileInputs.Add(input);
            return node;
        }

        internal void AddFileInputNode(string filePath, Func<string, Input> inputFactory)
        {
            Input input = inputFactory(filePath);
            try
            {
                AddFileInputNode(new FileInfo(filePath), input);
            }
            catch (Exception ex) when (Executor.IsFileSystemException(ex))
            {
                Nodes.Add(new InvalidInputNode(filePath, input));
            }
        }

        internal InputNode AddInputNode(Input input)
        {
            var node = new InputNode(input);
            Nodes.Add(node);
            Project.Inputs.Add(input);
            return node;
        }

        private void RemoveInvalidInput(string path)
        {
            InvalidInputNode node = Nodes.OfType<InvalidInputNode>()
                .FirstOrDefault(f => FileSystemUtility.PathEquals(f.Path, path));
            node?.Remove();
        }

        public override void Rename(string name)
        {
            if (!FileSystemUtility.IsValidFileName(name))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.FileNameHasInvalidFormatMsg, name));
            }

            string newPath = Path.Combine(
                DirectoryName,
                Path.ChangeExtension(Path.GetFileName(name), FileSystemManager.ProjectExtension));
            if (!FileSystemUtility.PathEquals(FullName, newPath, false))
            {
                FileInfo.MoveTo(newPath);
                UpdateProperties();
                SaveProject();
                var trv = TreeView as FileSystemTreeView;
                trv?.BeginInvoke(new Action(() =>
                {
                    Text = FileNameWithoutExtension;
                    trv.SortIfRequired(this);
                }));
            }
        }

        public void Paste(FileSystemManager manager, ClipboardMode mode)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (mode == ClipboardMode.Copy)
            {
                RootNode rootNode = manager.SelectedRootNode;
                if (rootNode != null)
                {
                    ProjectNode node = CopyTo(rootNode);
                    if (node != null)
                        manager.SelectedNode = node;
                }
            }
        }

        internal ProjectNode CopyTo(RootNode rootNode)
        {
            string path = Path.Combine(rootNode.FullName, FileName);
            bool flg = FileSystemUtility.PathEquals(rootNode.FullName, DirectoryName);

            if (flg)
                path = FileSystemUtility.GetFilePathCopy(path);

            var fi = new FileInfo(path);
            FileStream fs = null;
            try
            {
                fs = (flg) ? FileSystemUtility.CreateNew(path) : FileSystemUtility.CreateNewOrOverwrite(path);
                if (fs != null)
                {
                    var project = (Project)Project.Clone();
                    project.FilePath = path;
                    project.Save(fs);
                    ProjectNode node = rootNode.FindProjectNode(path);

                    node?.RemoveNode();

                    ProjectNode projectNode = rootNode.AddProjectNode(fi, project);
                    projectNode.LoadInputNodes();
                    return projectNode;
                }
            }
            finally
            {
                fs?.Dispose();
            }

            return null;
        }

        public bool CanCut()
        {
            return false;
        }

        public bool CanCopy()
        {
            return true;
        }

        public bool CanBePasted(TreeNode target, ClipboardMode mode)
        {
            switch (mode)
            {
                case ClipboardMode.Cut:
                    return false;
                case ClipboardMode.Copy:
                    return target is RootNode;
            }

            return false;
        }

        public FileInputNode FindFileInputNode(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
                return EnumerateFileInputNodes().FirstOrDefault(f => FileSystemUtility.PathEquals(f.FullName, filePath));

            return null;
        }

        public InputNode FindInputNode(string name)
        {
            if (!string.IsNullOrEmpty(name))
                return EnumerateInputNodes().FirstOrDefault(f => Input.NameEquals(f.Input.Name, name));

            return null;
        }

        internal FileInputNode FindFileInputNodeFromInputPath()
        {
            if (Project.InputNameKind == InputKind.File)
                return FindFileInputNode(Project.InputName);

            return null;
        }

        internal InputNode FindInputNodeFromInputPath()
        {
            if (Project.InputNameKind == InputKind.Included)
                return FindInputNode(Project.InputName);

            return null;
        }

        public bool ContainsFileInputNode(string filePath)
        {
            return FindFileInputNode(filePath) != null;
        }

        public bool ContainsInputNode(string name)
        {
            return FindInputNode(name) != null;
        }

        public void SaveProject()
        {
            Project.Save(FullName);
        }

        internal FileConnection OpenFile()
        {
            return new FileConnection(this);
        }

        internal bool OpenFile(Action action)
        {
            return Executor.Execute(() =>
            {
                using (OpenFile())
                    action();
            });
        }

        internal TResult OpenFile<TResult>(Func<TResult> func)
        {
            var result = default(TResult);
            Executor.Execute(() =>
            {
                using (OpenFile())
                    result = func();
            });
            return result;
        }

        public InputNode[] GetInputNodes()
        {
            return EnumerateInputNodes().ToArray();
        }

        public IEnumerable<InputNode> EnumerateInputNodes()
        {
            return Nodes.OfType<InputNode>();
        }

        public FileInputNode[] GetFileInputNodes()
        {
            return EnumerateFileInputNodes().ToArray();
        }

        public IEnumerable<FileInputNode> EnumerateFileInputNodes()
        {
            return Nodes.OfType<FileInputNode>();
        }

        public IEnumerable<string> EnumerateFileInputPaths()
        {
            return EnumerateFileInputNodes().Select(f => f.FullName);
        }

        public override RootNode GetRootNode()
        {
            return Parent as RootNode;
        }

        public override DirectoryInfo ParentDirectory
        {
            get { return FileInfo.Directory; }
        }

        public override FileInfo FileInfo { get; }

        public override NodeKind Kind
        {
            get { return NodeKind.Project; }
        }

        public InputCollection FileInputs
        {
            get { return Project.FileInputs; }
        }

        public InputCollection Inputs
        {
            get { return Project.Inputs; }
        }

        public Project Project
        {
            get { return _project; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!ReferenceEquals(_project, value))
                    _project = value;
            }
        }

        private Project _project;
    }
}