// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;
using System.IO;

namespace Regexator.FileSystem
{
    public class DirectoryCommand : FileSystemCommand
    {
        internal DirectoryCommand(DirectoryInfo info, FileSystemManager manager)
            : base(info)
        {
            _info = info;
            _manager = manager;
        }

        public override void Execute()
        {
            _manager.LoadDirectory(_info);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DirectoryCommand other))
                return false;

            return FileSystemUtility.PathEquals(_info, other._info);
        }

        public override int GetHashCode()
        {
            return _info.GetHashCode();
        }

        public override Bitmap Image
        {
            get { return _image; }
        }

        private readonly DirectoryInfo _info;
        private readonly FileSystemManager _manager;
        private static readonly Bitmap _image = Resources.IcoFolder.ToBitmap();
    }
}
