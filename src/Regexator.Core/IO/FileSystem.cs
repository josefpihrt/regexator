// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace Regexator.IO
{
    public static class FileSystem
    {
        public static IEnumerable<FileInfo> EnumerateFileInfos(DirectoryInfo directoryInfo)
        {
            return EnumerateFileInfos(directoryInfo, "*");
        }

        public static IEnumerable<FileInfo> EnumerateFileInfos(DirectoryInfo directoryInfo, string filePattern)
        {
            return EnumerateFileInfos(directoryInfo, filePattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<FileInfo> EnumerateFileInfos(DirectoryInfo directoryInfo, SearchOption option)
        {
            return EnumerateFileInfos(directoryInfo, "*", option);
        }

        public static IEnumerable<FileInfo> EnumerateFileInfos(
            DirectoryInfo directoryInfo,
            string filePattern,
            SearchOption option)
        {
            return EnumerateFileInfos(directoryInfo, filePattern, option, "*");
        }

        public static IEnumerable<FileInfo> EnumerateFileInfos(
            DirectoryInfo directoryInfo,
            string filePattern,
            SearchOption option,
            string directoryPattern)
        {
            if (directoryInfo == null)
                throw new ArgumentNullException("root");

            return EnumerateFileInfos();

            IEnumerable<FileInfo> EnumerateFileInfos()
            {
                var stack = new Stack<DirectoryInfo>();
                stack.Push(directoryInfo);

                while (stack.Count > 0)
                {
                    DirectoryInfo dirInfo = stack.Pop();

                    if (!Directory.Exists(dirInfo.FullName))
                        continue;

                    IEnumerator<FileInfo> enumerator = null;

                    try
                    {
                        enumerator = dirInfo.EnumerateFiles(filePattern, SearchOption.TopDirectoryOnly).GetEnumerator();
                    }
                    catch (Exception ex) when (IsFileSystemException(ex))
                    {
                    }

                    if (enumerator != null)
                    {
                        using (enumerator)
                        {
                            while (enumerator.MoveNext())
                                yield return enumerator.Current;
                        }
                    }

                    if (option == SearchOption.AllDirectories)
                    {
                        try
                        {
                            foreach (DirectoryInfo dirPath in dirInfo.EnumerateDirectories(
                                directoryPattern,
                                SearchOption.TopDirectoryOnly))
                            {
                                stack.Push(dirPath);
                            }
                        }
                        catch (Exception ex) when (IsFileSystemException(ex))
                        {
                        }
                    }
                }
            }
        }

        private static bool IsFileSystemException(Exception ex)
        {
            return ex is IOException
                || ex is ArgumentException
                || ex is UnauthorizedAccessException
                || ex is SecurityException
                || ex is NotSupportedException;
        }
    }
}
