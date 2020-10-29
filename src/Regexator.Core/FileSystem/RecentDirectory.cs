// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

namespace Regexator.FileSystem
{
    public sealed class RecentDirectory : RecentItem
    {
        public RecentDirectory(DirectoryInfo info)
            : base(info)
        {
        }

        public override ItemKind Kind
        {
            get { return ItemKind.Directory; }
        }
    }
}
