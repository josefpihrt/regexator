// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;
using System.IO;

namespace Regexator.FileSystem
{
    public class ProjectCommand : FileSystemCommand
    {
        internal ProjectCommand(FileInfo info, FileSystemManager manager)
            : base(info)
        {
            _info = info;
            _manager = manager;
        }

        public override void Execute()
        {
            if (!_manager.IsCurrentProjectNode(_info))
                _manager.LoadProject(_info);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ProjectCommand other))
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

        private readonly FileInfo _info;
        private readonly FileSystemManager _manager;
        private static readonly Bitmap _image = Resources.IcoCode.ToBitmap();
    }
}
