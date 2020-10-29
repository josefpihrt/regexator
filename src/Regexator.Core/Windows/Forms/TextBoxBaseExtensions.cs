// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Text;

namespace Regexator.Windows.Forms
{
    public static class TextBoxBaseExtensions
    {
        public static void ExtendSelection(this TextBoxBase box, int backward, int forward)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (backward < 0)
                throw new ArgumentOutOfRangeException(nameof(backward));

            if (forward < 0)
                throw new ArgumentOutOfRangeException(nameof(forward));

            box.Select(box.SelectionStart - backward, box.SelectionLength + backward + forward);
        }

        public static bool TrimSelectionMultiline(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return TextBoxUtility.ProcessSelection(box, f => f.TrimMultiline());
        }

        public static bool TrimSelectionStartMultiline(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return TextBoxUtility.ProcessSelection(box, f => f.TrimStartMultiline());
        }

        public static bool TrimSelectionEndMultiline(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return TextBoxUtility.ProcessSelection(box, f => f.TrimEndMultiline());
        }

        public static bool SelectionToSingleline(this TextBoxBase box, bool unindent)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return TextBoxUtility.ProcessSelection(
                box,
                f => Regex.Replace(f, (unindent) ? @"\r?\n(?!\z)[\s-[\r\n]]*" : @"\n(?!\z)", ""));
        }

        public static void Select(this TextBoxBase box, TextSpan textSpan)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (textSpan == null)
                throw new ArgumentNullException(nameof(textSpan));

            box.Select(textSpan.Index, textSpan.Length);
        }

        public static void Select(this TextBoxBase box, int index)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.Select(index, 0);
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Cr")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lf")]
        public static string GetTextCrLf(this TextBoxBase box)
        {
            return GetText(box, NewLineMode.CrLf);
        }

        public static string GetText(this TextBoxBase box, NewLineMode mode)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (mode == NewLineMode.CrLf && box.TextLength > 0)
                return string.Join("\r\n", box.Lines);

            return box.Text;
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Cr")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lf")]
        public static string GetSelectedTextCrLf(this TextBoxBase box)
        {
            return GetSelectedText(box, NewLineMode.CrLf);
        }

        public static string GetSelectedText(this TextBoxBase box, NewLineMode mode)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string s = box.SelectedText;

            if (mode == NewLineMode.CrLf && s.Length > 0)
                return s.EnsureCarriageReturnLinefeed();

            return s;
        }

        public static void Reselect(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.Select(box.SelectionStart, box.SelectionLength);
        }

        public static bool SelectionContains(this TextBoxBase box, int index)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.SelectionLength == 0)
                return box.SelectionStart == index;

            return index >= box.SelectionStart && index < box.SelectionStart + box.SelectionLength;
        }

        public static void SelectBeginning(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.Select(0, 0);
        }

        public static void SelectEnd(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.Select(box.TextLength, 0);
        }

        public static void SetAllText(this TextBoxBase box, string value)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.SelectAll();
            box.SelectedText = value;
            box.SelectBeginning();
        }

        public static void RemoveAllText(this TextBoxBase box)
        {
            SetAllText(box, "");
        }

        public static int GetFirstCharIndexFromLineModified(this TextBoxBase box, int lineIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.Lines.Take(lineIndex).Sum(f => f.Length) + lineIndex;
        }

        public static int GetLastCharIndexFromLine(this TextBoxBase box, int lineIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetFirstCharIndexFromLineModified(lineIndex) + box.Lines[lineIndex].Length;
        }

        public static void SelectLines(this TextBoxBase box, int startLine, int endLine)
        {
            SelectLines(box, startLine, endLine, false);
        }

        public static void SelectLines(this TextBoxBase box, int startLine, int endLine, bool includeNewLine)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string[] lines = box.Lines;
            if (lines.Length > 0)
            {
                int index = box.GetFirstCharIndexFromLineModified(startLine);

                if (includeNewLine && lines.Length > 1 && endLine < lines.Length - 1)
                {
                    box.Select(index, box.GetFirstCharIndexFromLineModified(endLine + 1) - index);
                }
                else
                {
                    box.Select(index, box.GetLastCharIndexFromLine(endLine) - index);
                }
            }
        }

        public static void SelectLineStart(this TextBoxBase box, int lineIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.Select(box.GetFirstCharIndexFromLineModified(lineIndex), 0);
        }

        public static void SelectLineEnd(this TextBoxBase box, int lineIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.Select(box.GetLastCharIndexFromLine(lineIndex), 0);
        }

        public static void SelectLine(this TextBoxBase box, int lineIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            SelectLine(box, lineIndex, false);
        }

        public static void SelectLine(this TextBoxBase box, int lineIndex, bool includeNewLine)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            box.SelectLines(lineIndex, lineIndex, includeNewLine);
        }

        public static bool IsMultilineSelection(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.SelectedText.IndexOf('\n') != -1;
        }

        public static bool IsLineSelected(this TextBoxBase box, int lineIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            int index = box.GetFirstCharIndexFromLineModified(lineIndex);
            return box.SelectionStart <= index
                && (box.SelectionStart + box.SelectionLength) >= index + box.Lines[lineIndex].Length;
        }

        public static int GetCharIndexFromMousePosition(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetCharIndexFromPosition(box.PointToClient(Control.MousePosition));
        }

        public static IEnumerable<string> EnumerateLines(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return EnumerateLines();

            IEnumerable<string> EnumerateLines()
            {
                string[] lines = box.Lines;

                if (lines == null)
                {
                    yield return "";
                    yield break;
                }

                for (int i = 0; i < lines.Length; i++)
                    yield return lines[i];
            }
        }

        public static IEnumerable<string> EnumerateSelectedLines(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return EnumerateSelectedLines();

            IEnumerable<string> EnumerateSelectedLines()
            {
                string[] lines = box.Lines;

                if (lines == null)
                {
                    yield return "";
                    yield break;
                }

                int startLine = box.GetLineFromCharIndexModified(box.SelectionStart);
                int endLine = box.GetLineFromCharIndexModified(box.SelectionStart + box.SelectionLength);

                if (endLine > startLine && box.SelectionEndsAtBeginningOfLine())
                    endLine--;

                for (int i = startLine; i <= endLine; i++)
                    yield return lines[i];
            }
        }

        public static int GetLineFromCharIndexModified(this TextBoxBase box, int index)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string[] lines = box.Lines;

            if (lines.Length == 0)
                return 0;

            if (index <= 0)
                return 0;

            if (index >= box.TextLength)
                return lines.Length - 1;

            if ((index / box.TextLength) < 0.5)
            {
                int len = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    len += lines[i].Length;
                    if (len >= index)
                        return i;

                    len++;
                }
            }
            else
            {
                int len = box.TextLength;
                for (int i = (lines.Length - 1); i >= 0; i--)
                {
                    len -= lines[i].Length;
                    if (index >= len)
                        return i;

                    len--;
                }
            }

            return 0;
        }

        public static int SelectEntireLines(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string[] lines = box.Lines;
            if (lines.Length > 0)
            {
                int selectionEnd = box.SelectionStart + box.SelectionLength;
                int startLine = box.GetLineFromCharIndexModified(box.SelectionStart);
                int index = box.GetFirstCharIndexFromLineModified(startLine);
                int endLine = box.GetLineFromCharIndexModified(selectionEnd);

                if (box.GetFirstCharIndexFromLineModified(endLine) == selectionEnd && endLine > startLine)
                    endLine--;

                box.Select(index, box.GetLastCharIndexFromLine(endLine) - index);

                return endLine - startLine;
            }

            return 0;
        }

        public static int GetSelectionStartLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetLineFromCharIndexModified(box.SelectionStart);
        }

        public static int GetSelectionEndLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetLineFromCharIndexModified(box.SelectionStart + box.SelectionLength);
        }

        public static int GetLineStartIndex(this TextBoxBase box, int charIndex)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetFirstCharIndexFromLineModified(box.GetLineFromCharIndexModified(charIndex));
        }

        public static string GetCurrentLineText(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.TextLength > 0)
            {
                int index = box.GetLineFromCharIndexModified(box.SelectionStart);
                return box.Lines[index];
            }

            return "";
        }

        public static int GetCurrentLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetLineFromCharIndexModified(box.SelectionStart);
        }

        public static int GetStartLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetLineFromCharIndexModified(box.SelectionStart);
        }

        public static int GetEndLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetLineFromCharIndexModified(box.SelectionStart + box.SelectionLength);
        }

        public static int GetFirstCharIndexOfCurrentLineModified(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.GetFirstCharIndexFromLineModified(box.GetLineFromCharIndexModified(box.SelectionStart));
        }

        public static bool SelectionEndsAtBeginningOfLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            int index = box.SelectionStart + box.SelectionLength;
            return index == box.GetLineStartIndex(index);
        }

        public static bool SelectionStartsAtBeginningOfLine(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            return box.SelectionStart == box.GetLineStartIndex(box.SelectionStart);
        }

        public static void SelectCurrentLine(this TextBoxBase box)
        {
            SelectCurrentLine(box, true);
        }

        public static void SelectCurrentLine(this TextBoxBase box, bool includeNewLine)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            if (box.TextLength > 0)
                box.SelectLine(box.GetCurrentLine(), includeNewLine);
        }

        public static bool MoveSelectionUp(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string[] lines = box.Lines;
            int sl = box.GetSelectionStartLine();
            if (lines.Length > 1 && sl > 0)
            {
                int el = box.GetSelectionEndLine();
                if (box.IsMultilineSelection() && box.SelectionEndsAtBeginningOfLine())
                    el--;

                TextSpan span = new TextSpan(box.SelectionStart, box.SelectionLength).Offset(-lines[sl - 1].Length - 1);
                if (span.Length > 0 && box.IsLineSelected(lines.Length - 1))
                    span = span.Extend(1);

                box.SelectLines(sl - 1, el, true);
                box.SelectedText = string.Concat(GetMoveUpText(lines, sl, el));
                box.Select(span);
                return true;
            }

            return false;
        }

        private static IEnumerable<string> GetMoveUpText(string[] lines, int sl, int el)
        {
            for (int i = sl; i <= el; i++)
                yield return lines[i] + "\n";

            yield return lines[sl - 1];

            if (el < lines.Length - 1)
                yield return "\n";
        }

        public static bool MoveSelectionDown(this TextBoxBase box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            string[] lines = box.Lines;
            int el = box.GetSelectionEndLine();
            if (box.IsMultilineSelection() && box.SelectionEndsAtBeginningOfLine())
                el--;

            if (lines.Length > 1 && el < (lines.Length - 1))
            {
                int sl = box.GetSelectionStartLine();
                TextSpan span = new TextSpan(box.SelectionStart, box.SelectionLength).Offset(lines[el + 1].Length + 1);
                box.SelectLines(sl, el + 1, true);
                box.SelectedText = string.Concat(GetMoveDownText(lines, sl, el));
                box.Select(span);
                return true;
            }

            return false;
        }

        private static IEnumerable<string> GetMoveDownText(string[] lines, int sl, int el)
        {
            yield return lines[el + 1] + "\n";
            for (int i = sl; i <= el; i++)
            {
                yield return lines[i];

                if (i < el || el < lines.Length - 2)
                    yield return "\n";
            }
        }
    }
}
