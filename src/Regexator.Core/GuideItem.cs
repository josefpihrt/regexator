// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Regexator.Text;

namespace Regexator
{
    [DebuggerDisplay("{Text}")]
    public class GuideItem
    {
        private string _categoryText;

        public GuideItem(string text, RegexCategory category, string description)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Category = category;
            Description = description ?? "";
        }

        public string Text { get; }

        public RegexCategory Category { get; }

        public string Description { get; }

        public string CategoryText
        {
            get { return _categoryText ?? (_categoryText = TextUtility.SplitCamelCase(Category)); }
        }
    }
}