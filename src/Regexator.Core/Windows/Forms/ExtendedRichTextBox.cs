// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using Regexator.Drawing;
using Regexator.Text;

namespace Regexator.Windows.Forms
{
    public class ExtendedRichTextBox : RichTextBox
    {
        private readonly Color _initBackColor;
        private int _spaceCount;
#if DEBUG
        private Rectangle _mouseDownRectangle;
        private static TextBoxTextSpan _dragSpan;
#endif
        public ExtendedRichTextBox()
        {
            BoldFont = new Font(Font, FontStyle.Bold);
            _initBackColor = BackColor;
            WordWrap = false;
            AcceptsTab = true;
            SpaceCount = 4;
            PlainTextOnly = true;
            EnableAutoDragDrop = false;
            AllowDrop = true;
        }

        public virtual void PasteText()
        {
            if (!ReadOnly)
            {
                if (PlainTextOnly)
                {
                    string s = GetClipboardText();
                    if (s != null)
                        SelectedText = s;
                }
                else
                {
                    ProcessClipboardText();
                    Paste();
                }
            }
        }

        private string GetClipboardText()
        {
            string s = null;

            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                s = Clipboard.GetText(TextDataFormat.UnicodeText);
            }
            else if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                s = Clipboard.GetText(TextDataFormat.Text);
            }

            if (s != null && NoTab)
                return TextUtility.Untabify(s, SpaceCount, SelectionStartFromBeginningOfLine);

            return s;
        }

        private void ProcessClipboardText()
        {
            if (NoTab)
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                {
                    ProcessClipboardText(TextDataFormat.UnicodeText);
                }
                else if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    ProcessClipboardText(TextDataFormat.Text);
                }
            }
        }

        private void ProcessClipboardText(TextDataFormat format)
        {
            string s = Clipboard.GetText(format);
            s = TextUtility.Untabify(s, SpaceCount, SelectionStartFromBeginningOfLine);
            Clipboard.SetText(s, format);
        }

        new public void AppendText(string text)
        {
            if (NoTab)
            {
                int index = 0;
                string[] lines = Lines;

                if (lines.Length > 0)
                    index = lines[lines.Length - 1].Length;

                _ = TextUtility.Untabify(text, SpaceCount, index);
            }
            else
            {
                base.AppendText(text);
            }
        }

        public virtual void PrintText()
        {
            PrintUtility.Print(this);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.KeyChar == '\t' && NoTab)
            {
                int cnt = SpaceCount - (SelectionStartFromBeginningOfLine % SpaceCount);
                SelectedText = new string(' ', cnt);
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (AcceptsTab && !ReadOnly)
            {
                Keys modifiers = keyData & Keys.Modifiers;
                Keys keyCode = keyData & Keys.KeyCode;

                if (modifiers == Keys.None && keyCode == Keys.Tab && this.IsMultilineSelection())
                {
                    MultilineIndent();
                    return true;
                }
                else if (modifiers == Keys.Shift && keyCode == Keys.Tab)
                {
                    if (this.IsMultilineSelection())
                    {
                        MultilineUnindent();
                    }
                    else
                    {
                        SinglelineUnindent();
                    }

                    return true;
                }
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        private void MultilineIndent()
        {
            int sl = SelectionStartLine;
            int el = SelectionEndLine;

            if (el > 0 && (this.GetFirstCharIndexFromLineModified(el) == SelectionEnd))
                el--;

            this.SelectLines(sl, el);
            string s = TextUtility.AddIndent(SelectedText, (NoTab) ? new string(' ', SpaceCount) : "\t");

            if (!string.IsNullOrEmpty(s))
                SelectedText = s;

            this.SelectLines(sl, el, true);
        }

        private void MultilineUnindent()
        {
            int sl = SelectionStartLine;
            int el = SelectionEndLine;

            if (el > 0 && (this.GetFirstCharIndexFromLineModified(el) == SelectionEnd))
                el--;

            this.SelectLines(sl, el);
            string s = TextUtility.RemoveIndent(SelectedText, SpaceCount);

            if (!string.IsNullOrEmpty(s))
                SelectedText = s;

            this.SelectLines(sl, el, true);
        }

        private void SinglelineUnindent()
        {
            string[] lines = Lines;
            if (lines.Length > 0)
            {
                int selStart = SelectionStartFromBeginningOfLine;
                var indenter = new Indenter(SpaceCount);
                int index = indenter.GetLastIndent(lines[SelectionStartLine], selStart);
                if (index != selStart)
                {
                    index += this.GetLineStartIndex(SelectionStart);
                    int selLength = SelectionLength;

                    this.BeginUpdate();
                    Select(index, SelectionStart - index);
                    SelectedText = "";
                    Select(index, selLength);
                    this.EndUpdate();
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.SuppressKeyPress)
            {
                switch (e.Modifiers)
                {
                    case Keys.Control:
                        {
                            switch (e.KeyCode)
                            {
                                case Keys.C:
                                    {
                                        ProcessCopyCurrentLine();
                                        break;
                                    }
                                case Keys.E:

                                case Keys.I:

                                case Keys.L:

                                case Keys.R:
                                    {
                                        e.SuppressKeyPress = true;
                                        break;
                                    }
                                case Keys.V:
                                    {
                                        if (!ReadOnly)
                                        {
                                            PasteText();
                                            e.Handled = true;
                                            e.SuppressKeyPress = true;
                                        }

                                        break;
                                    }
                                case Keys.X:
                                    {
                                        ProcessCutCurrentLine();
                                        break;
                                    }
                                case Keys.P:
                                    {
                                        PrintText();
                                        e.SuppressKeyPress = true;
                                        break;
                                    }
                                case Keys.D1:

                                case Keys.D2:

                                case Keys.D5:
                                    {
                                        e.SuppressKeyPress = true;
                                        break;
                                    }
                            }

                            break;
                        }
                    case Keys.Shift:
                        {
                            switch (e.KeyCode)
                            {
                                case Keys.Insert:
                                    {
                                        if (!ReadOnly)
                                        {
                                            PasteText();
                                            e.Handled = true;
                                            e.SuppressKeyPress = true;
                                        }

                                        break;
                                    }
                            }

                            break;
                        }
                    case (Keys.Control | Keys.Shift):
                        {
                            switch (e.KeyCode)
                            {
                                case Keys.I:
                                    {
                                        e.SuppressKeyPress = true;
                                        break;
                                    }
                            }

                            break;
                        }
                    case (Keys.Control | Keys.Alt):
                        {
                            if (e.KeyValue == 48 && !e.SuppressKeyPress)
                            {
                                this.ResetZoom();
                                e.SuppressKeyPress = true;
                            }

                            break;
                        }
                }
            }

            base.OnKeyDown(e);
        }

        protected virtual void ProcessCopyCurrentLine()
        {
            if (TextLength > 0 && SelectionLength == 0)
            {
                this.BeginUpdate();
                TextBoxUtility.CopyCurrentLine(this);
                this.EndUpdate();
            }
        }

        protected virtual void ProcessCutCurrentLine()
        {
            if (TextLength > 0 && SelectionLength == 0)
            {
                this.BeginUpdate();
                TextBoxUtility.CutCurrentLine(this);
                this.EndUpdate();
            }
        }

        protected override void OnReadOnlyChanged(EventArgs e)
        {
            if (!_initBackColor.IsEmpty)
                BackColor = _initBackColor;

            base.OnReadOnlyChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            BoldFont = new Font(Font, FontStyle.Bold);
            base.OnFontChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Right)
            {
                int index = GetCharIndexFromPosition(e.Location);
                if (!this.SelectionContains(index))
                {
                    Point pt = GetPositionFromCharIndex(TextLength);

                    if (e.Location.X >= pt.X && e.Location.Y >= pt.Y)
                    {
                        Select(TextLength, 0);
                    }
                    else
                    {
                        Select(index, 0);
                    }
                }
            }
#if DEBUG
            else if (e.Button == MouseButtons.Left)
            {
                if (SelectionLength > 0 && this.SelectionContains(GetCharIndexFromPosition(e.Location)))
                {
                    Size dragSize = SystemInformation.DragSize;
                    _mouseDownRectangle = new Rectangle(
                        new Point(e.X - (int)(dragSize.Width / 2), e.Y - (int)(dragSize.Height / 2)),
                        dragSize);
                }
                else
                {
                    _mouseDownRectangle = Rectangle.Empty;
                }
            }
#endif            
            base.OnMouseDown(e);
        }

#if DEBUG
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Left
                && _mouseDownRectangle != Rectangle.Empty
                && !_mouseDownRectangle.Contains(e.X, e.Y))
            {
                _dragSpan = new TextBoxTextSpan(this);
                DoDragDrop(SelectedText, DragDropEffects.Move);
                _dragSpan = null;
            }

            base.OnMouseMove(e);
        }
#endif

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            ProcessDragEvent(drgevent);
            base.OnDragEnter(drgevent);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            ProcessDragEvent(drgevent);
            base.OnDragOver(drgevent);
        }

        private void ProcessDragEvent(DragEventArgs e)
        {
            var effects = DragDropEffects.None;
            var flg = true;
#if DEBUG
            flg = _dragSpan == null
                || !ReferenceEquals(this, _dragSpan.TextBox)
                || !this.SelectionContains(GetCharIndexFromPosition(PointToClient(new Point(e.X, e.Y))));
#endif
            if (flg)
            {
                if (e.Data.GetDataPresent(DataFormats.UnicodeText, true) || e.Data.GetDataPresent(DataFormats.Text, true))
                {
                    effects = DragDropEffects.Move;
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    effects = DragDropEffects.Copy;
                }
            }

            e.Effect = effects;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent == null)
                throw new ArgumentNullException(nameof(drgevent));

            string s = GetText(drgevent);
            if (s != null)
            {
#if DEBUG
                if (_dragSpan != null)
                {
                    TextSpan span = new TextBoxTextSpan(this);
                    _dragSpan.Select();
                    _dragSpan.TextBox.SelectedText = "";

                    if (ReferenceEquals(this, _dragSpan.TextBox) && span.Index > _dragSpan.EndIndex)
                        span = span.Offset(-_dragSpan.Length);

                    Select(span.Index, span.Length);
                }
#endif
                if (NoTab)
                    s = TextUtility.Untabify(s, SpaceCount, SelectionStartFromBeginningOfLine);

                SelectedText = s;
            }

            base.OnDragDrop(drgevent);
        }

        private static string GetText(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
                return e.Data.GetData(DataFormats.UnicodeText, true).ToString();

            if (e.Data.GetDataPresent(DataFormats.Text, true))
                return e.Data.GetData(DataFormats.Text, true).ToString();

            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            {
                return string.Join("\n", paths);
            }

            return null;
        }

        public int SpaceCount
        {
            get { return _spaceCount; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");

                _spaceCount = value;
            }
        }

        new public string Text
        {
            get { return base.Text; }
            set
            {
                if (NoTab && value != null)
                {
                    base.Text = TextUtility.Untabify(value, SpaceCount);
                }
                else
                {
                    base.Text = value;
                }
            }
        }

        new public string SelectedText
        {
            get { return base.SelectedText; }
            set
            {
                if (NoTab && value != null)
                {
                    base.SelectedText = TextUtility.Untabify(value, SpaceCount, SelectionStart);
                }
                else
                {
                    base.SelectedText = value;
                }
            }
        }

        private int SelectionStartFromBeginningOfLine
        {
            get { return SelectionStart - this.GetFirstCharIndexFromLineModified(SelectionStartLine); }
        }

        public int SelectionStartLine
        {
            get { return this.GetLineFromCharIndexModified(SelectionStart); }
        }

        public int SelectionEndLine
        {
            get { return this.GetLineFromCharIndexModified(SelectionEnd); }
        }

        public int SelectionEnd
        {
            get { return SelectionStart + SelectionLength; }
        }

        public Font BoldFont { get; private set; }

        public int CurrentLine
        {
            get { return this.GetCurrentLine(); }
        }

        public bool NoTab { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PlainText")]
        public bool PlainTextOnly { get; set; }
    }
}
