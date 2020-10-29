// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Regexator.FileSystem
{
    internal sealed class FileConnection : IDisposable
    {
        private Project _project;
        private FileStream _stream;
        private bool _disposed;
        private static readonly object _syncRoot = new object();

        public FileConnection(ProjectNode node)
        {
            _project = node.Project;
            _stream = Open(node.FullName);
        }

        public FileConnection(Project project)
        {
            _project = project;
            _stream = Open(project.FilePath);
        }

        public static FileStream Open(string filePath)
        {
            return new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public void Close()
        {
            if (_stream != null)
            {
                lock (_syncRoot)
                {
                    if (_stream != null)
                    {
                        try
                        {
                            _project.Save(_stream);
                        }
                        finally
                        {
                            _stream.Dispose();
                            _stream = null;
                            _project = null;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Close();

                _disposed = true;
            }
        }
    }
}
