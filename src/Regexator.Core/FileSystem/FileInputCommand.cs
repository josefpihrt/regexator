// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;
using System.IO;

namespace Regexator.FileSystem
{
    public class FileInputCommand : FileSystemCommand
    {
        internal FileInputCommand(FileInfo projectInfo, FileInfo inputInfo, FileSystemManager manager)
            : base(projectInfo)
        {
            _projectInfo = projectInfo;
            _inputInfo = inputInfo;
            _manager = manager;
        }

        public override void Execute()
        {
            if (CheckCurrent())
                _manager.LoadProject(_projectInfo, _inputInfo);
        }

        private bool CheckCurrent()
        {
            if (_manager.IsCurrentProjectNode(_projectInfo))
            {
                return !(_manager.CurrentInputNode is FileInputNode node)
                    || !FileSystemUtility.PathEquals(node.FullName, _inputInfo.FullName);
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileInputCommand other))
                return false;

            return FileSystemUtility.PathEquals(_projectInfo, other._projectInfo)
                && FileSystemUtility.PathEquals(_inputInfo, other._inputInfo);
        }

        public override int GetHashCode()
        {
            return _projectInfo.GetHashCode() ^ _inputInfo.GetHashCode();
        }

        public override string Name
        {
            get { return base.Name + "  " + CompactName; }
        }

        public string CompactName
        {
            get
            {
                if (_compactName == null)
                {
                    string s = _inputInfo.Name;
                    _compactName = (s.Length > MaxNameLength)
                        ? s.Substring(0, MaxNameLength) + Resources.EllipsisStr
                        : s;
                }

                return _compactName;
            }
        }

        public override Bitmap Image
        {
            get { return _image; }
        }

        private readonly FileInfo _projectInfo;
        private readonly FileInfo _inputInfo;
        private readonly FileSystemManager _manager;
        private string _compactName;

        private static readonly Bitmap _image = Resources.IcoFile.ToBitmap();

        private const int MaxNameLength = 32;
    }
}
