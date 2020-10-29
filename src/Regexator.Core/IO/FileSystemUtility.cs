// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Security;
using Regexator.Text;

namespace Regexator.IO
{
    public static class FileSystemUtility
    {
        public static string MakeRelativePath(string path1, string path2)
        {
            if (!string.IsNullOrEmpty(path1) && !string.IsNullOrEmpty(path2))
            {
                return Uri.UnescapeDataString(
                    new Uri(path1).MakeRelativeUri(new Uri(path2)).ToString()
                )
                    .Replace('/', Path.DirectorySeparatorChar);
            }

            return path2;
        }

        public static bool IsValidPath(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                return !PatternLibrary.InvalidPathChars.IsMatch(input);

            return false;
        }

        public static bool IsValidFileName(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                return !PatternLibrary.InvalidFileNameChars.IsMatch(input);

            return false;
        }

        public static bool IsFileSystemException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            return
                ex is IOException
                    || ex is ArgumentException
                    || ex is UnauthorizedAccessException
                    || ex is SecurityException
                    || ex is NotSupportedException;
        }
    }
}
