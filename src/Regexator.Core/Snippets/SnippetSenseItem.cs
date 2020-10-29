// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;

namespace Regexator.Snippets
{
    public abstract class SnippetSenseItem : SenseItem
    {
        protected SnippetSenseItem(RegexSnippet snippet)
        {
            Snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
        }

        public override bool IsMatch(string text)
        {
            return Text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        public override bool Favorite
        {
            get { return Snippet.Favorite; }
            set { Snippet.Favorite = value; }
        }

        public override bool IsExtensible
        {
            get { return Snippet.IsExtensible; }
        }

        public override SnippetCodeKind ExtendedKind
        {
            get { return Snippet.ExtendedKind; }
        }

        public override bool Visible
        {
            get { return Snippet.Visible; }
        }

        public override Icon Icon
        {
            get { return (IsExtensible) ? Resources.IcoSnippetExtensible : Resources.IcoSnippet; }
        }

        public RegexSnippet Snippet { get; }
    }
}
