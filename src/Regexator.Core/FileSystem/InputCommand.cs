// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;
using System.IO;

namespace Regexator.FileSystem
{
    public class InputCommand : FileSystemCommand
    {
        internal InputCommand(FileInfo projectInfo, Input input, FileSystemManager manager)
            : base(projectInfo)
        {
            _projectInfo = projectInfo;
            _input = input;
            _manager = manager;
        }

        public override void Execute()
        {
            if (CheckCurrent()
                && _manager.Save())
            {
                _manager.LoadProject(_projectInfo, _input.Name);
            }
        }

        private bool CheckCurrent()
        {
            if (_manager.IsCurrentProjectNode(_projectInfo))
            {
                return !(_manager.CurrentInputNode is InputNode node) || node.Input.Name != _input.Name;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is InputCommand other))
                return false;

            return FileSystemUtility.PathEquals(_projectInfo, other._projectInfo)
                && Input.NameEquals(_input.Name, other._input.Name);
        }

        public override int GetHashCode()
        {
            return _projectInfo.GetHashCode() ^ _input.Name.GetHashCode();
        }

        public override string Name
        {
            get { return base.Name + "  <" + CompactName + ">"; }
        }

        public override Bitmap Image
        {
            get { return _image; }
        }

        public string CompactName
        {
            get
            {
                if (_compactName == null)
                {
                    string s = _input.Name;
                    _compactName = (s.Length > MaxNameLength)
                        ? s.Substring(0, MaxNameLength) + Resources.EllipsisStr
                        : s;
                }

                return _compactName;
            }
        }

        public override bool IsToolTipTextNeeded
        {
            get
            {
                return _projectInfo.FullName.Length > FileSystemUtility.MaxPathLength
                    || _input.Name.Length > MaxNameLength;
            }
        }

        private readonly FileInfo _projectInfo;
        private readonly Input _input;
        private string _compactName;
        private readonly FileSystemManager _manager;
        private static readonly Bitmap _image = Resources.IcoInput.ToBitmap();

        private const int MaxNameLength = 32;
    }
}
