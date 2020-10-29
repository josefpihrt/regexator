// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Regexator.Text
{
    public class SearchDefinition
    {
        private readonly string[] _lines;

        public SearchDefinition(TextBoxBase box)
        {
            Box = box ?? throw new ArgumentNullException(nameof(box));
            Text = box.Text;
            _lines = box.Lines;
        }

        public virtual IEnumerable<SearchMatch> FindAll()
        {
            if (Condition && Text.Length > 0)
            {
                foreach (int index in TextUtility.IndexesOf(Text, SearchPhrase, SearchOptions))
                    yield return new SearchMatch(index, this);
            }
        }

        public int GetFirstCharIndexFromLine(int lineIndex)
        {
            return TextUtility.GetFirstCharIndexFromLine(_lines, lineIndex);
        }

        public string GetLine(int lineIndex)
        {
            return _lines[lineIndex];
        }

        public int GetLineFromCharIndex(int index)
        {
            if (_lines.Length == 0)
                return 0;

            if (index <= 0)
                return 0;

            if (index >= Text.Length)
                return _lines.Length - 1;

            if ((index / Text.Length) < 0.5)
            {
                int len = 0;
                for (int i = 0; i < _lines.Length; i++)
                {
                    len += _lines[i].Length;
                    if (len >= index)
                        return i;

                    len++;
                }
            }
            else
            {
                int len = Text.Length;
                for (int i = (_lines.Length - 1); i >= 0; i--)
                {
                    len -= _lines[i].Length;
                    if (index >= len)
                        return i;

                    len--;
                }
            }

            return 0;
        }

        public TextBoxBase Box { get; }
        public string Text { get; }

        public bool Condition { get; set; }
        public string SearchPhrase { get; set; }
        public SearchOptions SearchOptions { get; set; }
        public string Name { get; set; }
    }
}
