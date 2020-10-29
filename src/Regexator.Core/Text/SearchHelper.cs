// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Regexator.Text
{
    public static class SearchHelper
    {
        public static string GetInitialSearchPhrase(ITextContainer box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (!string.IsNullOrEmpty(box.SelectedText))
                return box.SelectedText.GetFirstLine();

            string s = box.Text;

            if (s.Length == 0)
                return "";

            if (s.Length == 1)
            {
                if (!char.IsWhiteSpace(s[0]))
                    return s;
            }
            else
            {
                int start = 0;
                int end = 0;
                int i = box.SelectionStart;
                if (i == 0 || (i < s.Length && char.IsWhiteSpace(s[i - 1]) && !char.IsWhiteSpace(s[i])))
                {
                    while (i < s.Length && char.IsWhiteSpace(s[i]))
                        i++;

                    start = i;
                    end = FindBoundary(s, i);
                }
                else if (i == s.Length || char.IsWhiteSpace(s[i]))
                {
                    while (i > 0 && char.IsWhiteSpace(s[i - 1]))
                        i--;

                    end = i;
                    i--;
                    start = FindBoundaryBackward(s, i);
                }
                else
                {
                    start = FindBoundaryBackward(s, i);
                    end = FindBoundary(s, i);
                }

                return s.Substring(start, end - start);
            }

            return "";
        }

        private static int FindBoundary(string s, int start)
        {
            int i = start;
            if (char.IsLetterOrDigit(s[i]))
            {
                while (i < s.Length && char.IsLetterOrDigit(s[i]))
                    i++;
            }
            else
            {
                while (i < s.Length && !char.IsWhiteSpace(s[i]) && !char.IsLetterOrDigit(s[i]))
                    i++;
            }

            return i;
        }

        private static int FindBoundaryBackward(string s, int start)
        {
            int i = start;
            if (char.IsLetterOrDigit(s[i]))
            {
                while (i > 0 && char.IsLetterOrDigit(s[i - 1]))
                    i--;
            }
            else
            {
                while (i > 0 && !char.IsWhiteSpace(s[i - 1]) && !char.IsLetterOrDigit(s[i - 1]))
                    i--;
            }

            return i;
        }
    }
}
