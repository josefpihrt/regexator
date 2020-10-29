// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Regexator.IO;

namespace Regexator.Xml.Serialization.Projects
{
    public class FileSystemSearchInfo
    {
        public static FileSystemSearchInfo ToSerializable(Regexator.FileSystemSearchInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new FileSystemSearchInfo()
            {
                SearchMode = item.SearchMode,
                SearchOption = item.SearchOption,
                FileNamePart = item.FileNamePart
            };
        }

        public static Regexator.FileSystemSearchInfo FromSerializable(FileSystemSearchInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.FileSystemSearchInfo(item.SearchMode, item.SearchOption, item.FileNamePart);
        }

        public SearchMode SearchMode { get; set; }
        public SearchOption SearchOption { get; set; }
        public FileNamePart FileNamePart { get; set; }
    }
}