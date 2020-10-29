// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace Regexator.Snippets
{
    public class ExtendedSnippetInfo
    {
        public ExtendedSnippetInfo(string code, RegexOptions options, SnippetCodeKind kind)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Options = options;
            Kind = kind;
        }

        public string Code { get; }

        public SnippetCodeKind Kind { get; }

        public RegexOptions Options { get; }
    }
}
