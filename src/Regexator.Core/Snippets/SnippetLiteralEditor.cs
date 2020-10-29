// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.Snippets
{
    public partial class SnippetLiteralEditor : UserControl
    {
        public SnippetLiteralEditor()
        {
            InitializeComponent();
            Visible = false;
            TabStop = false;
            SuspendLayout();

            _tbx = new SenseTextBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Top,
                Location = new Point(0, 0),
                Name = "_tbx",
                Size = new Size(350, 20),
                TabIndex = 0
            };

            _tbx.KeyDown += (f, f2) => TextBox_KeyDown(f, f2);
            _tbx.TextChanged += (f, f2) => TextBox_TextChanged(f, f2);

            _rtb = new RichTextBox()
            {
                WordWrap = false,
                DetectUrls = false,
                ReadOnly = true,
                BackColor = SystemColors.Window,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Location = new Point(0, 20),
                Name = "_rtb",
                Size = new Size(350, 130),
                TabIndex = 0,
                Text = ""
            };

            _rtb.GotFocus += (f, f2) => RichTextBox_GotFocus(f, f2);

            _pnl = new Panel()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };

            _tlt = new ToolTip();

            _pnl.Controls.Add(_rtb);
            Controls.Add(_pnl);
            Controls.Add(_tbx);
            ResumeLayout(true);
        }

        public void Show(RegexSnippet snippet, string text, string selectedText)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            _origText = text.RemoveCarriageReturn();
            Snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
            _selectedText = ProcessSelectedText((selectedText ?? "").RemoveCarriageReturn());
            _enumerator = snippet.Literals.GetEnumerator();
            _enumeratorInitialized = false;
            _dicLiterals = snippet.Literals.ToDictionary(f => f.Token, f => new LiteralInfo(f));
            Visible = true;
            Select();
            _tbx.Select();
            ProcessNextLiteral();
        }

        private void ProcessNextLiteral()
        {
            if (_enumeratorInitialized)
                _dicLiterals[_enumerator.Current.Token].Value = _tbx.Text;

            _enumeratorInitialized = true;
            if (_enumerator.MoveNext())
            {
                bool flg = (_tbx.Text == (_enumerator.Current.DefaultValue ?? ""));
                _tbx.Text = _enumerator.Current.DefaultValue;
                _tlt.SetToolTip(_rtb, _enumerator.Current.Description);

                if (flg)
                    HighlightLiteral();

                _tbx.SelectAll();
            }
            else
            {
                TerminateSuccess();
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            HighlightLiteral();
        }

        private void HighlightLiteral()
        {
            SnippetLiteral literal = _enumerator.Current;
            if (literal != null)
            {
                _rtb.BeginUpdate();
                _rtb.SelectAll();
                _rtb.SelectionBackColor = _rtb.BackColor;
                _rtb.SelectionFont = _rtb.Font;
                _dicLiterals[_enumerator.Current.Token].Value = _tbx.Text;
                _offset = 0;

                foreach (LiteralInfo info in _dicLiterals.Values)
                    info.Indexes.Clear();

                _rtb.Text = Regex.Replace(_origText, "%[a-z_0-9]+%", f => ReplaceLiteral(f), RegexOptions.IgnoreCase);
                foreach (LiteralInfo info in _dicLiterals.Values)
                {
                    foreach (int index in info.Indexes)
                    {
                        _rtb.Select(index, info.Value.Length);
                        if (info.Literal.Id == SnippetLiteral.SelectedLiteral.Id)
                        {
                            _rtb.SelectionFont = new Font(_rtb.Font, FontStyle.Italic);
                        }
                        else
                        {
                            _rtb.SelectionFont = new Font(_rtb.Font, _rtb.Font.Style | FontStyle.Underline);

                            if (info.Literal.Id == _enumerator.Current.Id)
                                _rtb.SelectionFont = new Font(_rtb.Font, FontStyle.Bold | FontStyle.Underline);
                        }
                    }
                }

                _rtb.EndUpdate();
                _rtb.SelectBeginning();
            }
        }

        private string ReplaceLiteral(Match match)
        {
            if (!_dicLiterals.TryGetValue(match.Value, out LiteralInfo info)
                && match.Value == SnippetLiteral.SelectedLiteral.Token)
            {
                if (string.IsNullOrEmpty(_selectedText))
                    return "";

                info = new LiteralInfo(SnippetLiteral.SelectedLiteral, _selectedText);
                _dicLiterals.Add(SnippetLiteral.SelectedLiteral.Token, info);
            }

            if (info != null)
            {
                info.Indexes.Add(match.Index + _offset);
                _offset += info.Value.Length - match.Value.Length;
                return info.Value;
            }

            return "";
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                if (e.KeyCode == Keys.Tab)
                {
                    ProcessNextLiteral();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    ProcessNextLiteral();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    TerminateCancel();
                }
            }
        }

        private void RichTextBox_GotFocus(object sender, EventArgs e)
        {
            _tbx.Select();
        }

        private void TerminateCancel()
        {
            OnTerminate(EventArgs.Empty);
        }

        private void TerminateSuccess()
        {
            SnippetText = _origText;
            foreach (var item in Snippet.Literals.Join(
                _dicLiterals.Values,
                f => f.Id,
                g => g.Literal.Id,
                (f, g) => new { Literal = f, Replacement = g.Value }))
            {
                SnippetText = SnippetText.Replace(item.Literal.Token, item.Replacement);
            }

            Success = true;
            OnTerminate(EventArgs.Empty);
        }

        private static string ProcessSelectedText(string s)
        {
            const int maxLength = 20;
            const string suffix = "...";
            if (s != null)
            {
                int index = s.Substring(0, Math.Min(s.Length, maxLength)).IndexOfAny(new[] { '\r', '\n' });

                if (index != -1)
                    return s.Substring(0, index) + suffix;

                if (s.Length > 20)
                    return s.Substring(0, maxLength) + suffix;

                return s;
            }

            return null;
        }

        protected virtual void OnTerminate(EventArgs e)
        {
            Terminate?.Invoke(this, e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
                Success = false;

            base.OnVisibleChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            _tbx.Font = Font;
            _rtb.Font = Font;
            base.OnFontChanged(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            _tbx.BackColor = BackColor;
            _rtb.BackColor = BackColor;
            base.OnBackColorChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            _tbx.ForeColor = ForeColor;
            _rtb.ForeColor = ForeColor;
            base.OnForeColorChanged(e);
        }

        protected override Size DefaultMaximumSize
        {
            get { return new Size(300, 150); }
        }

        public bool Success { get; private set; }
        public string SnippetText { get; private set; }
        public RegexSnippet Snippet { get; private set; }

        private readonly SenseTextBox _tbx;
        private readonly RichTextBox _rtb;
        private readonly ToolTip _tlt;
        private readonly Panel _pnl;
        private IEnumerator<SnippetLiteral> _enumerator;
        private Dictionary<string, LiteralInfo> _dicLiterals;
        private int _offset;
        private string _origText;
        private string _selectedText;
        private bool _enumeratorInitialized;

        public event EventHandler Terminate;
    }
}
