// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace Regexator.FileSystem
{
    public static class Executor
    {
        public static bool ProcessException(Exception ex)
        {
            return ProcessException(ex, ShowMessageDefault);
        }

        public static bool ProcessException(Exception ex, bool showMessage)
        {
            return ProcessException(ex, BasicExceptions, showMessage);
        }

        public static bool ProcessException(Exception ex, Exceptions exceptions)
        {
            return ProcessException(ex, exceptions, ShowMessageDefault);
        }

        public static bool ProcessException(Exception ex, Exceptions exceptions, bool showMessage)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            if (IsFileSystemException(ex, exceptions))
            {
                ShowMessageIf(showMessage, ex);
                return true;
            }

            return false;
        }

        public static bool IsFileSystemException(Exception ex)
        {
            return IsFileSystemException(ex, BasicExceptions);
        }

        public static bool IsFileSystemException(Exception ex, Exceptions exceptions)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            return
                (((exceptions & Exceptions.Argument) == Exceptions.Argument) && ex is ArgumentException)
                    || (((exceptions & Exceptions.InvalidOperation) == Exceptions.InvalidOperation)
                        && ex is InvalidOperationException)
                    || (((exceptions & Exceptions.IO) == Exceptions.IO) && ex is IOException)
                    || (((exceptions & Exceptions.NotSupported) == Exceptions.NotSupported) && ex is NotSupportedException)
                    || (((exceptions & Exceptions.Security) == Exceptions.Security)
                        && ex is System.Security.SecurityException)
                    || (((exceptions & Exceptions.UnauthorizedAccess) == Exceptions.UnauthorizedAccess)
                        && ex is UnauthorizedAccessException)
                    || (((exceptions & Exceptions.XmlException) == Exceptions.XmlException)
                        && ex is System.Xml.XmlException);
        }

        public static bool Execute(Action action)
        {
            return Execute(action, ShowMessageDefault);
        }

        public static bool Execute(Action action, bool showMessage)
        {
            return Execute(action, BasicExceptions, showMessage);
        }

        public static bool Execute(Action action, Exceptions exceptions)
        {
            return Execute(action, exceptions, ShowMessageDefault);
        }

        public static bool Execute(Action action, Exceptions exceptions, bool showMessage)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                action();
                return true;
            }
            catch (Exception ex) when (ProcessException(ex, exceptions, showMessage))
            {
                return false;
            }
        }

        internal static bool Delete(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ProcessException(ex))
            {
                return false;
            }
        }

        internal static void StartProcess(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex) when (ex is InvalidOperationException
                || ex is System.ComponentModel.Win32Exception
                || ex is ObjectDisposedException
                || ex is FileNotFoundException
                || ex is DirectoryNotFoundException)
            {
                MessageDialog.Warning(ex.GetBaseException().Message);
            }
        }

        public static DirectoryInfo CreateDirectory(string path)
        {
            return CreateDirectory(path, false);
        }

        public static DirectoryInfo CreateDirectory(string path, bool showMessage)
        {
            DirectoryInfo info = null;
            Execute(() => info = Directory.CreateDirectory(path), showMessage);
            return info;
        }

        public static FileInfo CreateFileInfo(string path)
        {
            return CreateFileInfo(path, false);
        }

        public static FileInfo CreateFileInfo(string path, bool showMessage)
        {
            return CreateFileSystemInfo(() => new FileInfo(path), showMessage) as FileInfo;
        }

        public static DirectoryInfo CreateDirectoryInfo(string path)
        {
            return CreateDirectoryInfo(path, false);
        }

        public static DirectoryInfo CreateDirectoryInfo(string path, bool showMessage)
        {
            return CreateFileSystemInfo(() => new DirectoryInfo(path), showMessage) as DirectoryInfo;
        }

        private static FileSystemInfo CreateFileSystemInfo(Func<FileSystemInfo> creator, bool showMessage)
        {
            try
            {
                return creator();
            }
            catch (Exception ex) when (IsFileSystemException(ex))
            {
                ShowMessageIf(showMessage, ex);
                return null;
            }
        }

        private static void ShowMessageIf(bool condition, Exception ex)
        {
            Debug.WriteLine(ex.CreateLog());
            if (condition)
                MessageDialog.Warning(ex.GetBaseException().Message);
        }

        private const bool ShowMessageDefault = true;

        public static readonly Exceptions BasicExceptions =
            Exceptions.Argument
                | Exceptions.IO
                | Exceptions.NotSupported
                | Exceptions.Security
                | Exceptions.UnauthorizedAccess;
    }
}
