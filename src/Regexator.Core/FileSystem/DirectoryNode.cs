// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public sealed class DirectoryNode : FileSystemNode, INode, IClipboardNode
    {
        public DirectoryNode(DirectoryInfo info)
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
        }

        public override void Rename(string name)
        {
            if (!FileSystemUtility.IsValidFileName(name))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.FolderNameHasInvalidFormatMsg, name));
            }

            string newPath = Path.Combine(DirectoryName, name);
            if (!FileSystemUtility.PathEquals(FullName, newPath, false))
            {
                DirectoryInfo.MoveTo(newPath);
                UpdateProperties();
                var trv = TreeView as FileSystemTreeView;
                trv?.BeginInvoke(new Action(() => trv.SortIfRequired(this)));
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
                    DirectoryNode node = CopyTo(rootNode);
                    if (node != null)
                        manager.SelectedNode = node;
                }
            }
        }

        internal DirectoryNode CopyTo(RootNode rootNode)
        {
            string dirPath = Path.Combine(rootNode.FullName, FileName);

            if (Equals(DirectoryName, rootNode.FullName))
                dirPath = FileSystemUtility.GetDirectoryPathCopy(dirPath);

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(FullName, dirPath);
            return rootNode.FindDirectoryNode(dirPath) ?? rootNode.AddDirectoryNode(Path.GetFileName(dirPath));
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

        public override RootNode GetRootNode()
        {
            return Parent as RootNode;
        }

        public override DirectoryInfo DirectoryInfo { get; }

        public override DirectoryInfo ParentDirectory
        {
            get { return DirectoryInfo.Parent; }
        }

        public override NodeKind Kind
        {
            get { return NodeKind.Directory; }
        }
    }
}
