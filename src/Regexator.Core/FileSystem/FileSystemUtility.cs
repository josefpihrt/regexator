// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using Regexator.Collections.Generic;

namespace Regexator.FileSystem
{
    public static class FileSystemUtility
    {
        internal static void SetImage(TreeNode node, NodeImageIndex index)
        {
            node.ImageIndex = (int)index;
            node.SelectedImageIndex = (int)index;
        }

        internal static bool TryCreateRelativePath(string projectPath, string inputPath, out string result)
        {
            result = "";
            try
            {
                string relPath = IO.FileSystemUtility.MakeRelativePath(projectPath, inputPath);
                if (!relPath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                {
                    result = relPath;
                    return true;
                }
            }
            catch (UriFormatException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return false;
        }

        internal static bool TryCreateAbsolutePath(string dirPath, string inputPath, out string result)
        {
            if (IO.FileSystemUtility.IsValidPath(dirPath) && IO.FileSystemUtility.IsValidPath(inputPath))
            {
                try
                {
                    result = (Path.IsPathRooted(inputPath)) ? inputPath : Path.GetFullPath(Path.Combine(dirPath, inputPath));
                    return true;
                }
                catch (Exception ex) when (Executor.IsFileSystemException(ex))
                {
                }
            }

            result = "";
            return false;
        }

        public static void DeleteDirectory(string dirPath)
        {
            DeleteDirectory(dirPath, false);
        }

        public static void DeleteDirectory(string dirPath, bool useRecycleBin)
        {
            if (dirPath == null)
                throw new ArgumentNullException(nameof(dirPath));

            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                dirPath,
                UIOption.OnlyErrorDialogs,
                (useRecycleBin) ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently,
                UICancelOption.ThrowException);
        }

        public static void DeleteFile(string filePath)
        {
            DeleteFile(filePath, false);
        }

        public static void DeleteFile(string filePath, bool useRecycleBin)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                filePath,
                UIOption.OnlyErrorDialogs,
                (useRecycleBin) ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently,
                UICancelOption.ThrowException);
        }

        internal static string GetCompactPath(string path)
        {
            return GetCompactPath(path, MaxPathLength);
        }

        internal static string GetCompactPath(string path, int maxLength)
        {
            if (path.Length > maxLength)
            {
                try
                {
                    var sb = new StringBuilder(maxLength + 1);
                    if (NativeMethods.PathCompactPathEx(sb, path, maxLength + 1, 0))
                        return sb.ToString();
                }
                catch (EntryPointNotFoundException)
                {
                }
                catch (DllNotFoundException)
                {
                }
            }

            return path;
        }

        public static bool PathEquals(FileSystemInfo info1, FileSystemInfo info2)
        {
            return PathEquals(info1, info2, true);
        }

        public static bool PathEquals(FileSystemInfo info1, FileSystemInfo info2, bool ignoreCase)
        {
            if (info1 == null)
                throw new ArgumentNullException(nameof(info1));

            if (info2 == null)
                throw new ArgumentNullException(nameof(info2));

            return PathEquals(info1.FullName, info2.FullName, ignoreCase);
        }

        public static bool PathEquals(string path1, string path2)
        {
            return PathEquals(path1, path2, true);
        }

        public static bool PathEquals(string path1, string path2, bool ignoreCase)
        {
            return string.Equals(path1, path2, (ignoreCase) ? ComparisonIgnoreCase : Comparison);
        }

        internal static string GetNewDirectoryName(string parentDirPath)
        {
            string dirPath = Path.Combine(parentDirPath, Resources.NewFolder);
            var generator = new SuffixGenerator();
            while (true)
            {
                string path = dirPath + generator.Suffix;

                if (!Directory.Exists(path))
                    return path;

                generator.Increment();
            }
        }

        internal static string GetDirectoryPathCopy(string dirPath)
        {
            var generator = new CopySuffixGenerator();
            while (true)
            {
                string path = dirPath + generator.Suffix;

                if (!Directory.Exists(path))
                    return path;

                generator.Increment();
            }
        }

        internal static string GetFilePathCopy(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            var generator = new CopySuffixGenerator();
            while (true)
            {
                string path = Path.Combine(dirPath, Path.ChangeExtension(fileName + generator.Suffix, extension));

                if (!File.Exists(path))
                    return path;

                generator.Increment();
            }
        }

        internal static FileStream CreateNewOrOverwrite(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (MessageDialog.Question(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.FileAlreadyExistsDoYouWantToOvervwriteItMsg,
                    filePath.Enclose("'"))) == DialogResult.Yes)
                {
                    return new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                }
            }
            else
            {
                return CreateNew(filePath);
            }

            return null;
        }

        internal static bool CheckFileExists(string filePath, bool useRecycleBin)
        {
            if (File.Exists(filePath))
            {
                if (MessageDialog.Question(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.FileAlreadyExistsDoYouWantToOvervwriteItMsg,
                    filePath.Enclose("'"))) == DialogResult.Yes)
                {
                    if (File.Exists(filePath))
                        DeleteFile(filePath, useRecycleBin);

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        internal static FileStream CreateNew(string filePath)
        {
            return new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        }

        public static bool IsValidFileName(string fileName)
        {
            return IO.FileSystemUtility.IsValidFileName(fileName);
        }

        public static void OpenPathInExplorer(FileSystemNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            OpenPathInExplorer(node.FullName);
        }

        internal static void OpenPathInExplorer(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (File.Exists(path) || Directory.Exists(path))
                Executor.StartProcess(() => Process.Start("explorer.exe", string.Concat("/select, \"", path, "\"")));
        }

        private static IEnumerable<Encoding> EnumerateCommonEncodings()
        {
            yield return Input.DefaultEncoding;
            yield return Encoding.GetEncoding("utf-16");
            yield return Encoding.GetEncoding("windows-1250");
            yield return Encoding.GetEncoding("windows-1252");
            yield return Encoding.GetEncoding("iso-8859-1");
            yield return Encoding.GetEncoding("iso-8859-2");
            yield return Encoding.GetEncoding("us-ascii");
        }

        public static StringComparison ComparisonIgnoreCase
        {
            get { return StringComparison.OrdinalIgnoreCase; }
        }

        public static StringComparison Comparison
        {
            get { return StringComparison.Ordinal; }
        }

        public static StringComparer ComparerIgnoreCase
        {
            get { return StringComparer.OrdinalIgnoreCase; }
        }

        public static ReadOnlyCollection<Encoding> CommonEncodings
        {
            get { return _commonEncodingsLazy.Value; }
        }

        private static readonly Lazy<ReadOnlyCollection<Encoding>> _commonEncodingsLazy
            = new Lazy<ReadOnlyCollection<Encoding>>(() => EnumerateCommonEncodings().ToReadOnly());

        internal const int MaxPathLength = 64;
    }
}
