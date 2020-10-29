// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Regexator.IO
{
    public static class FileSystemInfoExtensions
    {
        public static bool ExistsNow(this FileSystemInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return (info.HasAttributes(FileAttributes.Directory))
                ? Directory.Exists(info.FullName)
                : File.Exists(info.FullName);
        }

        public static bool IsHidden(this FileSystemInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return info.HasAttributes(FileAttributes.Hidden);
        }

        public static bool IsDirectory(this FileSystemInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return info.HasAttributes(FileAttributes.Directory);
        }

        private static bool HasAttributes(this FileSystemInfo info, FileAttributes attributes)
        {
            return (info.Attributes & attributes) == attributes;
        }
    }
}
