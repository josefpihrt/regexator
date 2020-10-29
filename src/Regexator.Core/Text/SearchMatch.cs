// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Windows.Forms;

namespace Regexator.Text
{
    public sealed class SearchMatch
    {
        public SearchMatch(int index, SearchDefinition definition)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Index = index;
            SearchDefinition = definition ?? throw new ArgumentNullException(nameof(definition));
            LineIndex = definition.GetLineFromCharIndex(index);
        }

        public string GetText()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0}({1}):{2}",
                SearchDefinition.Name,
                LineIndex.ToString(CultureInfo.InvariantCulture),
                SearchDefinition.GetLine(LineIndex));
        }

        public bool IsContained(int length)
        {
            int lineStartIndex = SearchDefinition.GetFirstCharIndexFromLine(LineIndex);
            return Index >= lineStartIndex && Index < (lineStartIndex + length);
        }

        public int Index { get; }
        public SearchDefinition SearchDefinition { get; }
        public int LineIndex { get; }

        public int Length
        {
            get { return SearchDefinition.SearchPhrase.Length; }
        }

        public TextBoxBase Box
        {
            get { return SearchDefinition.Box; }
        }
    }
}
