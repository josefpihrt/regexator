// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public sealed class FileInputNode : FileSystemNode, IInputContainer, INode, IClipboardNode
    {
        public FileInputNode(FileInfo info, Input input)
            : base(info)
        {
            FileInfo = info;
            Input = input ?? throw new ArgumentNullException(nameof(input));
            SetImage(NodeImageIndex);
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            Name = FullName;
            Text = FileName;
            ToolTipText = FullName;
        }

        public override void Rename(string name)
        {
            if (!FileSystemUtility.IsValidFileName(name))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.FileNameHasInvalidFormatMsg, name));
            }

            string oldPath = FullName;
            string newPath = GetNewFilePath(name);
            if (!FileSystemUtility.PathEquals(oldPath, newPath, false))
            {
                using (ProjectNode.OpenFile())
                {
                    FileInfo.MoveTo(newPath);

                    if (Project.InputNameKind == InputKind.File && FileSystemUtility.PathEquals(oldPath, Project.InputName))
                        Project.InputName = newPath;

                    Input.Name = newPath;
                    UpdateProperties();
                }

                var trv = TreeView as FileSystemTreeView;
                trv?.BeginInvoke(new Action(() =>
                {
                    Text = FileName;
                    trv.SortIfRequired(this);
                }));
            }
        }

        private string GetNewFilePath(string name)
        {
            string newName = Path.GetFileName(name);
            if (!Path.HasExtension(newName))
            {
                newName = Path.ChangeExtension(
                    newName,
                    (newName.LastOrDefault() == '.') ? null : Extension);
            }

            return Path.Combine(DirectoryName, newName);
        }

        public void Paste(FileSystemManager manager, ClipboardMode mode)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            ProjectNode projectNode = (manager.SelectedNode as ProjectNode) ?? (manager.SelectedNode.Parent as ProjectNode);
            if (projectNode != null)
            {
                switch (mode)
                {
                    case ClipboardMode.Cut:
                        {
                            if (MoveTo(projectNode))
                                manager.SelectedNode = this;

                            break;
                        }
                    case ClipboardMode.Copy:
                        {
                            FileInputNode node = CopyTo(projectNode);
                            if (node != null)
                                manager.SelectedNode = node;

                            break;
                        }
                }
            }
        }

        internal FileInputNode CopyTo(ProjectNode projectNode)
        {
            FileInputNode node = projectNode.FindFileInputNode(FullName);
            if (node != null)
            {
                MessageDialog.Warning(Resources.InputIsAlreadyPartOfProject);
            }
            else
            {
                using (projectNode.OpenFile())
                    return projectNode.AddFileInputNode(new FileInfo(FullName), (Input)Input.Clone());
            }

            return null;
        }

        internal bool MoveTo(ProjectNode projectNode)
        {
            if (projectNode.TreeView is FileSystemTreeView trv
                && ReferenceEquals(trv, TreeView))
            {
                if (trv.IsCurrentInputNode(this))
                {
                    MessageDialog.Warning(Resources.OpenInputCannotBeMovedMsg);
                }
                else if (projectNode.ContainsFileInputNode(FullName))
                {
                    MessageDialog.Warning(Resources.InputIsAlreadyPartOfProject);
                }
                else
                {
                    using (ProjectNode.OpenFile())
                    {
                        using (projectNode.OpenFile())
                        {
                            Project.FileInputs.Remove(Input);
                            projectNode.FileInputs.Add(Input);
                        }
                    }

                    Remove();
                    projectNode.Nodes.Add(this);
                    return true;
                }
            }

            return false;
        }

        public bool CanCut()
        {
            return true;
        }

        public bool CanCopy()
        {
            return true;
        }

        public bool CanBePasted(TreeNode target, ClipboardMode mode)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return target is ProjectNode;
        }

        public void LoadText()
        {
            Input.LoadText();
        }

        public void SaveText(string text)
        {
            Input.SaveText(text);
        }

        public override RootNode GetRootNode()
        {
            ProjectNode node = ProjectNode;
            return (node != null) ? node.Parent as RootNode : null;
        }

        public override FileInfo FileInfo { get; }

        public override DirectoryInfo ParentDirectory
        {
            get { return FileInfo.Directory; }
        }

        public ProjectNode ProjectNode
        {
            get { return Parent as ProjectNode; }
        }

        public string ProjectPath
        {
            get
            {
                ProjectNode projectNode = ProjectNode;
                return projectNode?.FullName;
            }
        }

        public Project Project
        {
            get
            {
                ProjectNode projectNode = ProjectNode;
                return projectNode?.Project;
            }
        }

        public override NodeKind Kind
        {
            get { return NodeKind.FileInput; }
        }

        public Input Input
        {
            get { return _input; }
            set { _input = value ?? throw new ArgumentNullException("value"); }
        }

        public NodeImageIndex NodeImageIndex
        {
            get { return NodeImageIndex.FileInput; }
        }

        public NodeImageIndex OpenNodeImageIndex
        {
            get { return NodeImageIndex.FileInputOpen; }
        }

        private Input _input;
    }
}
