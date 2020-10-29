﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Snippets
{
    public class SnippetCodeSenseItem : SnippetSenseItem
    {
        public SnippetCodeSenseItem(RegexSnippet snippet)
            : base(snippet)
        {
        }

        public override string Text
        {
            get { return Snippet.CleanCodeSingleline; }
        }
    }
}
