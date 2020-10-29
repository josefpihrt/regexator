// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Regexator.IO
{
    public class FileLock : IDisposable
    {
        private readonly FileStream _fileStream;
        private bool _disposed;

        public FileLock(string dirPath)
        {
            if (dirPath == null)
                throw new ArgumentNullException(nameof(dirPath));

            Guid = Guid.NewGuid();
            FilePath = Path.Combine(dirPath, Guid.ToString());
            _fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _fileStream?.Dispose();

                    File.Delete(FilePath);
                }

                _disposed = true;
            }
        }

        public string FilePath { get; }

        public Guid Guid { get; }
    }
}
