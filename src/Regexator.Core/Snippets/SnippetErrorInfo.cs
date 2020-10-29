// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Regexator.FileSystem;

namespace Regexator.Snippets
{
    public class SnippetErrorInfo : FileErrorInfo
    {
        public SnippetErrorInfo(string path, Exception exception)
            : this(path, exception, null)
        {
        }

        public SnippetErrorInfo(string path, Exception exception, string fullName)
            : base(path, exception)
        {
            _snippetFullName = fullName;
        }

        public override string Comment
        {
            get { return _snippetFullName; }
        }

        private readonly string _snippetFullName;
    }
}
