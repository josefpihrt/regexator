// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using Regexator.Text;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public class FindToolStrip : ExtendedToolStrip
    {
        private readonly ToolStripButton _btnMatchCase;
        private readonly ToolStripButton _btnMatchWholeWord;
        private readonly ToolStripButton _btnPattern;
        private readonly ToolStripButton _btnInput;
        private readonly ToolStripButton _btnReplacement;
        private readonly ToolStripButton _btnOutput;

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public FindToolStrip()
        {
            FindButton = new ToolStripButton(null, Resources.IcoArrowRightSmall.ToBitmap())
            {
                ToolTipText = Resources.FindAll
            };
            StopFindButton = new ToolStripButton(null, Resources.IcoClose.ToBitmap())
            {
                ToolTipText = Resources.StopFind,
                Enabled = false
            };

            _btnMatchCase = new ToolStripButton(null, Resources.IcoMatchCase.ToBitmap())
            {
                CheckOnClick = true,
                ToolTipText = Resources.MatchCase + " " + Resources.AltC.AddParentheses()
            };
            _btnMatchWholeWord = new ToolStripButton(null, Resources.IcoMatchWhole.ToBitmap())
            {
                CheckOnClick = true,
                ToolTipText = Resources.MatchWholeWord + " " + Resources.AltW.AddParentheses()
            };

            _btnPattern = new ToolStripButton("P")
            {
                CheckOnClick = true,
                Checked = true,
                ToolTipText = Resources.SearchPattern
            };
            _btnInput = new ToolStripButton("I")
            {
                CheckOnClick = true,
                Checked = true,
                ToolTipText = Resources.SearchInput
            };
            _btnReplacement = new ToolStripButton("R")
            {
                CheckOnClick = true,
                Checked = true,
                ToolTipText = Resources.SearchReplacement
            };
            _btnOutput = new ToolStripButton("O")
            {
                CheckOnClick = true,
                Checked = true,
                ToolTipText = Resources.SearchOutput
            };

            SearchBox = new ToolStripComboBox() { Width = 100, ToolTipText = Resources.SearchTerm };
            SearchBox.KeyDown += (f, f2) => SearchBox_KeyDown(f, f2);
            SearchBox.DropDown += (object sender, EventArgs e) =>
            {
                SearchBox.Items.Clear();
                SearchBox.Items.AddRange(EnumerateSearchPhrases().ToArray());
            };

            Items.Add(SearchBox);
            Items.Add(FindButton);
            Items.Add(StopFindButton);
            Items.Add(_btnMatchCase);
            Items.Add(_btnMatchWholeWord);
            Items.Add(_btnPattern);
            Items.Add(_btnInput);
            Items.Add(_btnReplacement);
            Items.Add(_btnOutput);

            History = new Dictionary<string, DateTime>();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.A:
                                {
                                    SearchBox.SelectAll();
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
                                    MatchCase = !MatchCase;
                                    return true;
                                }
                            case Keys.W:
                                {
                                    MatchWholeWord = !MatchWholeWord;
                                    return true;
                                }
                        }

                        break;
                    }
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        public IEnumerable<string> EnumerateSearchPhrases()
        {
            return History
                .OrderByDescending(f => f.Value)
                .Select(f => f.Key)
                .Take(20);
        }

        public bool MatchCase
        {
            get { return _btnMatchCase.Checked; }
            set { _btnMatchCase.Checked = value; }
        }

        public bool MatchWholeWord
        {
            get { return _btnMatchWholeWord.Checked; }
            set { _btnMatchWholeWord.Checked = value; }
        }

        public string SearchPhrase
        {
            get { return SearchBox.Text; }
        }

        public bool SearchPattern
        {
            get { return _btnPattern.Checked; }
            set { _btnPattern.Checked = value; }
        }

        public bool SearchInput
        {
            get { return _btnInput.Checked; }
            set { _btnInput.Checked = value; }
        }

        public bool SearchReplacement
        {
            get { return _btnReplacement.Checked; }
            set { _btnReplacement.Checked = value; }
        }

        public bool SearchOutput
        {
            get { return _btnOutput.Checked; }
            set { _btnOutput.Checked = value; }
        }

        public ToolStripComboBox SearchBox { get; }

        public Dictionary<string, DateTime> History { get; }

        public SearchOptions Options
        {
            get
            {
                var options = SearchOptions.None;

                if (MatchCase)
                    options |= SearchOptions.MatchCase;

                if (MatchWholeWord)
                    options |= SearchOptions.MatchWholeWord;

                return options;
            }
        }

        public ToolStripButton FindButton { get; }

        public ToolStripButton StopFindButton { get; }
    }
}
