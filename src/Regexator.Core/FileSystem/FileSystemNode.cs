// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public abstract class FileSystemNode : TreeNode, INode
    {
        [DebuggerStepThrough]
        protected FileSystemNode(FileSystemInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public abstract NodeKind Kind { get; }
        public abstract DirectoryInfo ParentDirectory { get; }

        public abstract RootNode GetRootNode();

        public void SetImage(NodeImageIndex index)
        {
            FileSystemUtility.SetImage(this, index);
        }

        public virtual void Rename(string name)
        {
            throw new NotSupportedException();
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

        public FileSystemInfo Info { get; }

        public virtual FileInfo FileInfo
        {
            get { return null; }
        }

        public virtual DirectoryInfo DirectoryInfo
        {
            get { return null; }
        }

        public string FullName
        {
            get { return Info.FullName; }
        }

        public string DirectoryName
        {
            get { return ParentDirectory.FullName; }
        }

        public string FileName
        {
            get { return Info.Name; }
        }

        public string FileNameWithoutExtension
        {
            get { return Path.GetFileNameWithoutExtension(FileName); }
        }

        public string Extension
        {
            get { return Info.Extension; }
        }

        public string RootPath
        {
            get
            {
                RootNode node = GetRootNode();
                return node?.FullName;
            }
        }
    }
}
