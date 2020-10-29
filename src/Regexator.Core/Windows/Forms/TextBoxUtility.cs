// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Text;

namespace Regexator.Windows.Forms
{
    public static class TextBoxUtility
    {
        private static readonly Regex _emptyOrWhiteSpaceLine = PatternLibrary.EmptyOrWhiteSpaceLine.ToRegex();
        private static readonly Regex _emptyLine = PatternLibrary.EmptyLine.ToRegex();

        public static bool RemoveWhiteSpaceLinesFromSelection(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.SelectionLength > 0)
            {
                box.SelectEntireLines();
                return ProcessSelection(box, f => _emptyOrWhiteSpaceLine.Replace(f, ""));
            }

            return false;
        }

        public static bool RemoveEmptyLinesFromSelection(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.SelectionLength > 0)
            {
                box.SelectEntireLines();
                return ProcessSelection(box, f => _emptyLine.Replace(f, ""));
            }

            return false;
        }

        public static bool SelectionToLower(TextBoxBase box)
        {
            return SelectionToLower(box, CultureInfo.CurrentCulture);
        }

        public static bool SelectionToLower(TextBoxBase box, CultureInfo culture)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            return ProcessSelection(box, f => f.ToLower(culture));
        }

        public static bool SelectionToUpper(TextBoxBase box)
        {
            return SelectionToUpper(box, CultureInfo.CurrentCulture);
        }

        public static bool SelectionToUpper(TextBoxBase box, CultureInfo culture)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            return ProcessSelection(box, f => f.ToUpper(culture));
        }

        public static bool ProcessSelection(TextBoxBase box, Func<string, string> processor)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            string s = processor(box.SelectedText);
            if (box.SelectedText != s)
            {
                int index = box.SelectionStart;
                box.SelectedText = s;
                box.Select(index, s.Length);
                return true;
            }

            return false;
        }

        public static void CopyCurrentLine(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.TextLength > 0)
            {
                using (new SelectionMemento(box))
                {
                    box.SelectCurrentLine(true);
                    box.Copy();
                }
            }
        }

        public static void CutCurrentLine(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.TextLength > 0)
            {
                box.SelectCurrentLine(true);
                box.Cut();
            }
        }

        public static bool AddLineComment(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.Lines.Length > 0)
            {
                int startLine = box.GetStartLine();
                int cnt = box.SelectEntireLines();
                box.SelectedText = Regex.Replace(box.SelectedText, "^", "#", RegexOptions.Multiline);
                box.SelectLines(startLine, startLine + cnt);
                return true;
            }

            return false;
        }

        public static bool RemoveLineComment(TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.Lines.Length > 0)
            {
                int startLine = box.GetStartLine();
                int cnt = box.SelectEntireLines();
                string s = Regex.Replace(box.SelectedText, @"(?<=^[\s-[\r\n]]*)#", "", RegexOptions.Multiline);
                if (s != box.SelectedText)
                {
                    box.SelectedText = s;
                    box.SelectLines(startLine, startLine + cnt);
                    return true;
                }
            }

            return false;
        }
    }
}
