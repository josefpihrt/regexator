// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Windows.Forms;

namespace Regexator.FileSystem
{
    internal interface IClipboardNode
    {
        void Paste(FileSystemManager manager, ClipboardMode mode);
        bool CanCut();
        bool CanCopy();
        bool CanBePasted(TreeNode target, ClipboardMode mode);
    }
}
