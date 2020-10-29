// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.FileSystem
{
    internal sealed class ClipboardItem
    {
        private ClipboardItem(Guid guid, IClipboardNode node, ClipboardMode mode)
        {
            Guid = guid;
            Node = node;
            Mode = mode;
        }

        public static ClipboardItem Create(IClipboardNode node, ClipboardMode mode)
        {
            return new ClipboardItem(Guid.NewGuid(), node, mode);
        }

        public void Paste(FileSystemManager manager)
        {
            Node.Paste(manager, Mode);
        }

        public bool CanBePasted(TreeNode target)
        {
            return Node.CanBePasted(target, Mode);
        }

        public Guid Guid { get; }

        public IClipboardNode Node { get; }

        public ClipboardMode Mode { get; }
    }
}
