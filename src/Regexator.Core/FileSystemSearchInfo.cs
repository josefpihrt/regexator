// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Regexator.IO;

namespace Regexator
{
    [Serializable]
    public class FileSystemSearchInfo : ICloneable
    {
        public FileSystemSearchInfo(
            SearchMode searchMode = SearchMode.FileName,
            SearchOption searchOption = SearchOption.AllDirectories,
            FileNamePart fileNamePart = FileNamePart.NameAndExtension)
        {
            SearchMode = searchMode;
            SearchOption = searchOption;
            FileNamePart = fileNamePart;
        }

        public static ContainerProps GetChangedProps(
            FileSystemSearchInfo first,
            FileSystemSearchInfo second,
            ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.FileSystemSearchMode) && first.SearchMode != second.SearchMode)
                value |= ContainerProps.FileSystemSearchMode;

            if (props.Contains(ContainerProps.FileSystemSearchOption) && first.SearchOption != second.SearchOption)
                value |= ContainerProps.FileSystemSearchOption;

            if (props.Contains(ContainerProps.FileSystemFileNamePart) && first.FileNamePart != second.FileNamePart)
                value |= ContainerProps.FileSystemFileNamePart;

            return value;
        }

        public object Clone()
        {
            return new FileSystemSearchInfo(SearchMode, SearchOption, FileNamePart);
        }

        public SearchMode SearchMode { get; set; }
        public SearchOption SearchOption { get; set; }
        public FileNamePart FileNamePart { get; set; }

        public static readonly ContainerProps AllProps = ContainerProps.FileSystemSearchMode
            | ContainerProps.FileSystemSearchOption
            | ContainerProps.FileSystemFileNamePart;
    }
}
