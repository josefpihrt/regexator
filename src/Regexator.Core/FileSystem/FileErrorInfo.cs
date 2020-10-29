// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.FileSystem
{
    public class FileErrorInfo : ErrorInfo
    {
        public FileErrorInfo(string path, Exception exception)
            : base(path, exception)
        {
            Path = path;
        }

        public override string Name
        {
            get
            {
                if (!_fFileName)
                {
                    try
                    {
                        _fileName = System.IO.Path.GetFileName(Path);
                    }
                    catch (ArgumentException)
                    {
                        _fileName = Path;
                    }

                    _fFileName = true;
                }

                return _fileName;
            }
        }

        public override string FullName
        {
            get { return Path; }
        }

        public string Path { get; }

        private bool _fFileName;
        private string _fileName;
    }
}
