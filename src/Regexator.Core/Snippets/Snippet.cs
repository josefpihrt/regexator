// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Regexator.Snippets
{
    public class Snippet
    {
        [DebuggerStepThrough]
        public Snippet(string code)
            : this(code, null)
        {
        }

        public Snippet(string code, IList<SnippetLiteral> literals)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            CleanCode = SnippetLiteral.RemoveReservedLiterals(code);
            Literals = new ReadOnlyCollection<SnippetLiteral>(literals ?? new SnippetLiteral[] { });
            IsMultiline = code.IsMultiline();
            Title = "";
            Keywords = new Collection<string>();
            Version = new Version();
            FormatVersion = new Version();
        }

        public string Title
        {
            get { return _title; }
            set { _title = value ?? ""; }
        }

        public Version Version
        {
            get { return _version; }
            set { _version = value ?? throw new ArgumentNullException("value"); }
        }

        public Version FormatVersion
        {
            get { return _formatVersion; }
            set { _formatVersion = value ?? throw new ArgumentNullException("value"); }
        }

        public bool IsSurround
        {
            get { return (SnippetKinds & SnippetKinds.Surround) == SnippetKinds.Surround; }
        }

        public string Code { get; }
        public string CleanCode { get; }
        public bool IsMultiline { get; }
        public string Description { get; set; }
        public string Author { get; set; }
        public Uri HelpUrl { get; set; }
        public Collection<string> Keywords { get; }
        public ReadOnlyCollection<SnippetLiteral> Literals { get; }
        public SnippetKinds SnippetKinds { get; set; }

        private string _title;
        private Version _version;
        private Version _formatVersion;
    }
}
