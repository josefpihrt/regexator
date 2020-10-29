// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using Regexator.Text;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public class SearchToolStrip : ExtendedToolStrip
    {
        public SearchToolStrip(RichTextBox rtb)
            : this(rtb, true)
        {
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public SearchToolStrip(RichTextBox rtb, bool replaceEnabled)
        {
            RichTextBox = rtb ?? throw new ArgumentNullException(nameof(rtb));
            _wrapper = new RichTextBoxWrapper(rtb);
            ReplaceEnabled = replaceEnabled;
            Searcher = new RichTextBoxSearcher(rtb) { ReplaceEnabled = replaceEnabled };
            Searcher.MatchCaseChanged += (object sender, EventArgs e) => _btnMatchCase.Checked = Searcher.MatchCase;
            Searcher.MatchWholeWordChanged += (object sender, EventArgs e) => _btnMatchWholeWord.Checked = Searcher
                .MatchWholeWord;

            Visible = false;

            _btnMatchCase = new ToolStripButton(null, Resources.IcoMatchCase.ToBitmap())
            {
                CheckOnClick = true,
                ToolTipText = Resources.MatchCase + " " + Resources.AltC.AddParentheses()
            };
            _btnMatchCase.Click += (object sender, EventArgs e) => Searcher.ToggleMatchCase();

            _btnMatchWholeWord = new ToolStripButton(null, Resources.IcoMatchWhole.ToBitmap())
            {
                CheckOnClick = true,
                ToolTipText = Resources.MatchWholeWord + " " + Resources.AltW.AddParentheses()
            };
            _btnMatchWholeWord.Click += (object sender, EventArgs e) => Searcher.ToggleMatchWholeWord();

            _cmbFind = new ToolStripComboBox() { Width = 75, ToolTipText = Resources.SearchTerm };
            _cmbFind.TextChanged += (object sender, EventArgs e) => Searcher.SearchPhrase = _cmbFind.Text;
            _cmbFind.KeyDown += (f, f2) => SearchPhrase_KeyDown(f, f2);
            _cmbFind.DropDown += (object sender, EventArgs e) =>
            {
                _cmbFind.Items.Clear();
                _cmbFind.Items.AddRange(Searcher.EnumerateSearchPhrases().Take(10).ToArray());
            };

            _btnFindNext = new ToolStripButton(null, Resources.IcoArrowRightSmall.ToBitmap())
            {
                ToolTipText = Resources.FindNext + " " + Resources.F3.AddParentheses()
            };
            _btnFindNext.Click += (object sender, EventArgs e) => FindNext();

            if (replaceEnabled)
            {
                _cmbReplace = new ToolStripComboBox() { Width = 75, ToolTipText = Resources.ReplacementTerm };
                _cmbReplace.TextChanged += (object sender, EventArgs e) => Searcher.ReplacementPhrase = _cmbReplace.Text;
                _cmbReplace.KeyDown += (f, f2) => ReplacementPhrase_KeyDown(f, f2);
                _cmbReplace.DropDown += (object sender, EventArgs e) =>
                {
                    _cmbReplace.Items.Clear();
                    _cmbReplace.Items.AddRange(Searcher.EnumerateReplacePhrases().Take(10).ToArray());
                };

                _btnReplaceNext = new ToolStripButton(null, Resources.IcoReplace.ToBitmap())
                {
                    ToolTipText = Resources.ReplaceNext + " " + Resources.AltR
                };
                _btnReplaceNext.Click += (object sender, EventArgs e) => ReplaceNext();
            }

            _btnClose = new ToolStripButton(null, Resources.IcoClose.ToBitmap())
            {
                ToolTipText = Resources.Close,
                Alignment = ToolStripItemAlignment.Right
            };
            _btnClose.Click += (object sender, EventArgs e) => HideToolStrip();

            Items.Add(_btnClose);
            Items.Add(_btnMatchCase);
            Items.Add(_btnMatchWholeWord);
            Items.Add(_btnFindNext);
            Items.Add(_cmbFind);
            if (replaceEnabled)
            {
                Items.Add(_cmbReplace);
                Items.Add(_btnReplaceNext);
            }

            RichTextBox.KeyDown += (f, f2) => Box_KeyDown(f, f2);
        }

        private void FindNext()
        {
            switch (Searcher.FindNext())
            {
                case FindResult.NoSuccess:
                    {
                        MessageDialog.Warning(Resources.FollowingTextWasNotFoundMsg + " " + Searcher.SearchPhrase);
                        break;
                    }
                case FindResult.Finished:
                    {
                        MessageDialog.Info(Resources.FindReachedStartingPointMsg);
                        break;
                    }
            }
        }

        private void ReplaceNext()
        {
            switch (Searcher.ReplaceNext())
            {
                case FindResult.NoSuccess:
                    {
                        MessageDialog.Warning(Resources.FollowingTextWasNotFoundMsg + " " + Searcher.SearchPhrase);
                        break;
                    }
                case FindResult.Finished:
                    {
                        MessageDialog.Info(Resources.FindReachedStartingPointMsg);
                        break;
                    }
            }
        }

        private void SearchPhrase_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Enter:
                                {
                                    FindNext();
                                    break;
                                }
                            case Keys.Down:
                                {
                                    if (!_cmbFind.DroppedDown && Searcher.EnumerateSearchPhrases().Any())
                                        _cmbFind.DroppedDown = true;

                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.A:
                                {
                                    _cmbFind.SelectAll();
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ReplacementPhrase_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Enter:
                                {
                                    ReplaceNext();
                                    break;
                                }
                            case Keys.Down:
                                {
                                    if (!_cmbReplace.DroppedDown && Searcher.EnumerateReplacePhrases().Any())
                                        _cmbReplace.DroppedDown = true;

                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.A:
                                {
                                    _cmbReplace.SelectAll();
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            Keys modifiers = keyData & Keys.Modifiers;
            Keys keyCode = keyData & Keys.KeyCode;
            switch (modifiers)
            {
                case Keys.Alt:
                    {
                        switch (keyCode)
                        {
                            case Keys.C:
                                {
                                    Searcher.ToggleMatchCase();
                                    return true;
                                }
                            case Keys.R:
                                {
                                    if (Searcher.ReplaceEnabled)
                                        ReplaceNext();

                                    return true;
                                }
                            case Keys.W:
                                {
                                    Searcher.ToggleMatchWholeWord();
                                    return true;
                                }
                        }

                        break;
                    }
                case Keys.None:
                    {
                        switch (keyCode)
                        {
                            case Keys.Escape:
                                {
                                    if (!_cmbFind.DroppedDown && (!ReplaceEnabled || !_cmbReplace.DroppedDown))
                                    {
                                        HideToolStrip();
                                        return true;
                                    }

                                    break;
                                }
                            case Keys.F3:
                                {
                                    FindNext();
                                    return true;
                                }
                        }

                        break;
                    }
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        private void Box_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.None:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Escape:
                                {
                                    Hide();
                                    break;
                                }
                            case Keys.F3:
                                {
                                    FindNext();
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.F:
                                {
                                    ShowToolStrip();
                                    break;
                                }
                            case Keys.H:
                                {
                                    if (Searcher.ReplaceEnabled)
                                        ShowToolStrip();

                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void ShowToolStrip()
        {
            string s = SearchHelper.GetInitialSearchPhrase(_wrapper);

            if (!string.IsNullOrEmpty(s))
                _cmbFind.Text = s;

            Show();
            _cmbFind.Focus();
            _cmbFind.SelectAll();
        }

        private void HideToolStrip()
        {
            RichTextBox.Select();
            Hide();
        }

        public RichTextBox RichTextBox { get; }

        public bool ReplaceEnabled { get; }

        public RichTextBoxSearcher Searcher { get; }

        private readonly RichTextBoxWrapper _wrapper;

        private readonly ToolStripButton _btnMatchCase;
        private readonly ToolStripButton _btnMatchWholeWord;
        private readonly ToolStripComboBox _cmbFind;
        private readonly ToolStripButton _btnFindNext;
        private readonly ToolStripComboBox _cmbReplace;
        private readonly ToolStripButton _btnReplaceNext;
        private readonly ToolStripButton _btnClose;
    }
}
