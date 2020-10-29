// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Regexator.Snippets;

namespace Regexator.Xml.Serialization.Snippets
{
    public class SnippetInfo
    {
        public SnippetInfo(string filePath, Regexator.Snippets.RegexSnippet snippet)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
        }

        public SnippetInfo(SnippetErrorInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            FilePath = info.FullName;
            ErrorInfo = info;
        }

        public string FilePath { get; }

        public Regexator.Snippets.RegexSnippet Snippet { get; }

        public SnippetErrorInfo ErrorInfo { get; }

        public bool Success
        {
            get { return Snippet != null; }
        }
    }
}
