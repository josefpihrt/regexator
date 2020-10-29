// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;
using System.IO;

namespace Regexator.FileSystem
{
    public abstract class FileSystemCommand : ICommand
    {
        protected FileSystemCommand(FileSystemInfo info)
        {
            FileSystemInfo = info;
        }

        public abstract void Execute();

        public abstract Bitmap Image { get; }

        public FileSystemInfo FileSystemInfo { get; }

        public string FullName
        {
            get { return FileSystemInfo.FullName; }
        }

        public virtual string Name
        {
            get { return FileSystemUtility.GetCompactPath(FileSystemInfo.FullName); }
        }

        public virtual bool IsToolTipTextNeeded
        {
            get { return FileSystemInfo.FullName.Length > FileSystemUtility.MaxPathLength; }
        }
    }
}
