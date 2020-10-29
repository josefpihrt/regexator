// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Regexator.IO;

namespace Regexator.FileSystem
{
    //TODO: FileSystemSearchSettings
    public class FileSystemSearchSettings
    {
        public SearchMode SearchMode { get; set; }
        public SearchOption SearchOption { get; set; }
        public FileNamePart FileNamePart { get; set; }
    }
}
