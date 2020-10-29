// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Text;
using Regexator.Windows.Forms;

namespace Regexator.Snippets
{
    internal sealed class SnippetInsertProcessor
    {
        private SnippetInsertProcessor()
        {
        }

        public static void InsertText(RichTextBox rtb, string text)
        {
            var processor = new SnippetInsertProcessor();
            processor.Insert(rtb, text);
        }

        public void Insert(RichTextBox rtb, string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            _text = text.RemoveCarriageReturn();
            _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
            Initialize();
            GetSelectionIndent();
            UnindentSelection();
            IndentSelection();
            InsertSelection();
            IndentText();
            RemoveEndToken();
            InsertText();
            SelectEndTokenIndex();
        }

        private void Initialize()
        {
            if (_rtb.SelectionLength > 0 && _rtb.SelectionEndsAtBeginningOfLine())
                _rtb.SelectionLength--;

            _selectionStart = _rtb.SelectionStart;
            _selectionStartsAtBeginningOfLine = _rtb.SelectionStartsAtBeginningOfLine();
            _selection = _rtb.SelectedText;
        }

        private void GetSelectionIndent()
        {
            if (_selectionStartsAtBeginningOfLine)
            {
                _selectionIndent = Regex.Match(_selection, @"^\ *").Value;
            }
            else
            {
                int startIndex = _rtb.GetLineStartIndex(_selectionStart);
                string input = _rtb.Text.Substring(startIndex, _selectionStart - startIndex);
                _selectionIndent = Regex.Match(input, @"^\ *$").Value;
            }
        }

        private void UnindentSelection()
        {
            string len = _selectionIndent.Length.ToString(CultureInfo.InvariantCulture);
            _selection = (_selectionStartsAtBeginningOfLine)
                ? Regex.Replace(_selection, @"^\ {0," + len + "}", "", RegexOptions.Multiline)
                : Regex.Replace(_selection, @"\n\ {0," + len + "}", "\n");
        }

        private void IndentSelection()
        {
            _selection = _selection.AddIndent(GetSelectedTokenIndent(), false);
        }

        private string GetSelectedTokenIndent()
        {
            return Regex.Match(
                _text,
                @"^\ *(?=" + Regex.Escape(SnippetLiteral.SelectedLiteral.Token) + ")",
                RegexOptions.Multiline).Value;
        }

        private void InsertSelection()
        {
            _text = SnippetLiteral.SelectedLiteral.ReplaceToken(_text, _selection);
        }

        private void IndentText()
        {
            _text = _text.AddIndent(_selectionIndent, _selectionStartsAtBeginningOfLine);
        }

        private void RemoveEndToken()
        {
            _endTokenIndex = SnippetLiteral.EndLiteral.FindToken(_text);

            if (_endTokenIndex != -1)
                _text = SnippetLiteral.EndLiteral.RemoveToken(_text);
        }

        private void InsertText()
        {
            _rtb.SelectedText = _text;
        }

        private void SelectEndTokenIndex()
        {
            if (_endTokenIndex != -1)
                _rtb.Select(_selectionStart + _endTokenIndex, 0);
        }

        private string _text;
        private string _selection;
        private string _selectionIndent;
        private int _selectionStart;
        private bool _selectionStartsAtBeginningOfLine;
        private int _endTokenIndex;
        private RichTextBox _rtb;
    }
}
