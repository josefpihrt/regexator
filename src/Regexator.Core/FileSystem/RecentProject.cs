// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

namespace Regexator.FileSystem
{
    public sealed class RecentProject : RecentItem
    {
        public RecentProject(FileInfo info)
            : base(info)
        {
            _info = info;
        }

        public override string Text
        {
            get { return Path.Combine(_info.DirectoryName, Path.GetFileNameWithoutExtension(_info.Name)); }
        }

        public override ItemKind Kind
        {
            get { return ItemKind.Project; }
        }

        private readonly FileInfo _info;
    }
}
