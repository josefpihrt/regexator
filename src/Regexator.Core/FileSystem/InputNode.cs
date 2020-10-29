// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public sealed class InputNode : TreeNode, IInputContainer, INode, IClipboardNode
    {
        internal InputNode(Input input)
        {
            Input = input;
            Text = input.Name;
            FileSystemUtility.SetImage(this, NodeImageIndex);
        }

        public void Rename(string name)
        {
            if (!FileSystemUtility.IsValidFileName(name))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.NameHasInvalidFormatMsg, name));
            }

            if (!string.Equals(Input.Name, name, StringComparison.CurrentCulture))
            {
                using (ProjectNode.OpenFile())
                {
                    string oldName = Input.Name;
                    Input.Name = name;

                    if (Project.InputNameKind == InputKind.Included && Input.NameEquals(oldName, Project.InputName))
                        Project.InputName = name;

                    Text = name;
                }

                var trv = TreeView as FileSystemTreeView;
                trv?.BeginInvoke(new Action(() => trv.SortIfRequired(this)));
            }
        }

        internal void RemoveNode()
        {
            if (TreeView is FileSystemTreeView trv)
            {
                trv.RemoveNode(this);
            }
            else
            {
                Remove();
            }
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
                            InputNode node = CopyTo(projectNode);
                            if (node != null)
                                manager.SelectedNode = node;

                            break;
                        }
                }
            }
        }

        internal InputNode CopyTo(ProjectNode projectNode)
        {
            InputNode node = null;
            string name = Input.Name;
            if (Equals(Project.FilePath, projectNode.FullName))
            {
                name = projectNode.Project.GetInputNameCopy(name);
            }
            else
            {
                node = projectNode.FindInputNode(name);
            }

            if (node == null || MessageDialog.Question(Resources.InputIsAlreadyPartOfProjectOverwiteMsg) == DialogResult.Yes)
            {
                using (projectNode.OpenFile())
                {
                    if (node != null)
                    {
                        projectNode.Inputs.Remove(node.Input);
                        node.RemoveNode();
                    }

                    var input = (Input)Input.Clone();
                    input.Name = name;

                    return projectNode.AddInputNode(input);
                }
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
                else
                {
                    InputNode node = projectNode.FindInputNode(Input.Name);
                    if (node == null
                        || MessageDialog.Question(Resources.InputIsAlreadyPartOfProjectOverwiteMsg) == DialogResult.Yes)
                    {
                        using (ProjectNode.OpenFile())
                        {
                            using (projectNode.OpenFile())
                            {
                                if (node != null)
                                {
                                    projectNode.Inputs.Remove(node.Input);
                                    node.RemoveNode();
                                }

                                Project.Inputs.Remove(Input);
                                projectNode.Inputs.Add(Input);
                            }
                        }

                        Remove();
                        projectNode.Nodes.Add(this);
                        return true;
                    }
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

        public RootNode GetRootNode()
        {
            ProjectNode node = ProjectNode;
            return (node != null) ? node.Parent as RootNode : null;
        }

        public string RootPath
        {
            get
            {
                RootNode node = GetRootNode();
                return node?.FullName;
            }
        }

        public ProjectNode ProjectNode
        {
            get { return Parent as ProjectNode; }
        }

        public Project Project
        {
            get { return ProjectNode.Project; }
        }

        public Input Input
        {
            get { return _input; }
            set { _input = value ?? throw new ArgumentNullException("value"); }
        }

        public NodeKind Kind
        {
            get { return NodeKind.Input; }
        }

        public string FullName
        {
            get { return Input.Name; }
        }

        public string FileName
        {
            get { return Input.Name; }
        }

        public NodeImageIndex NodeImageIndex
        {
            get { return NodeImageIndex.Input; }
        }

        public NodeImageIndex OpenNodeImageIndex
        {
            get { return NodeImageIndex.InputOpen; }
        }

        private Input _input;
    }
}