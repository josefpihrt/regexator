// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Regexator.Collections.Generic;
using Regexator.Text;
using Regexator.Text.RegularExpressions;
using Pihrtsoft.Text.RegularExpressions.Linq;

namespace Regexator.UI
{
    public partial class CharacterAnalyzerForm : Form
    {
        private ContextMenuStrip _cms;
        private int _charCode;
        private bool _loading;

        public CharacterAnalyzerForm()
        {
            InitializeComponent();
            Text = "Character Analyzer";
            Icon = Resources.RegexatorIcon;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            _lblComment.Text = "";

            _rtbChar.AcceptsTab = false;
            _rtbCode.AcceptsTab = false;
            _rtbHex.AcceptsTab = false;

            _rtbChar.TextChanged += (f, f2) => LoadItems(f, f2);
            _rtbChar.SelectionChanged += (f, f2) => LoadItems(f, f2);
            _rtbCode.TextChanged += (f, f2) => LoadItems(f, f2);
            _rtbHex.TextChanged += (f, f2) => LoadItems(f, f2);

            _dgv.RowHeadersVisible = false;
            _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgv.BorderStyle = BorderStyle.None;
            _dgv.KeyDown += (f, f2) => DataGridView_KeyDown(f, f2);
            _dgv.CellContextMenuStripNeeded += (f, f2) => DataGridView_CellContextMenuStripNeeded(f, f2);

            _cbxIgnoreCase.CheckedChanged += (object sender, EventArgs e) => LoadItems();
            _cbxECMAScript.CheckedChanged += (object sender, EventArgs e) => LoadItems();
            _cbxCharGroup.CheckedChanged += (object sender, EventArgs e) => LoadItems();
        }

        protected override void OnLoad(EventArgs e)
        {
            _rtbChar.Select();
            base.OnLoad(e);
        }

        private void DataGridView_CellContextMenuStripNeeded(
            object sender,
            DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (_cms == null)
                {
                    _cms = new ContextMenuStrip();
                    _cms.Items.Add(new ToolStripMenuItem(
                        Resources.CopyPattern,
                        null,
                        (object sender2, EventArgs e2) => CopyPattern())
                    { ShortcutKeyDisplayString = Resources.CtrlC });
                }

                e.ContextMenuStrip = _cms;
            }
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.Control:
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.C:
                                {
                                    {
                                        if (CopyPattern())
                                            e.Handled = true;
                                    }

                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        private void LoadItems(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _loading = true;
                int charCode = -1;

                if (ReferenceEquals(sender, _rtbChar))
                {
                    charCode = LoadFromCharBox();
                }
                else if (ReferenceEquals(sender, _rtbCode))
                {
                    charCode = LoadFromCodeBox();
                }
                else if (ReferenceEquals(sender, _rtbHex))
                {
                    charCode = LoadFromHexBox();
                }

                if (charCode != _charCode)
                {
                    _charCode = charCode;
                    LoadItems();
                }

                _loading = false;
            }
        }

        private int LoadFromCharBox()
        {
            int charCode = -1;
            int index = GetCharIndex();
            if (index != -1)
            {
                charCode = (int)_rtbChar.Text[index];
                _rtbCode.Text = charCode.ToString(CultureInfo.InvariantCulture);
                _rtbHex.Text = charCode.ToString("X", CultureInfo.InvariantCulture);
            }
            else
            {
                _rtbCode.Clear();
                _rtbHex.Clear();
            }

            return charCode;
        }

        private int LoadFromCodeBox()
        {
            int charCode = -1;
            if (int.TryParse(_rtbCode.Text.Trim(), out int result) && result >= 0 && result <= 0xFFFF)
            {
                charCode = result;
                var ch = (char)charCode;
                _rtbChar.Text = (char.GetUnicodeCategory(ch) == UnicodeCategory.Control) ? "" : ch.ToString();
                _rtbHex.Text = charCode.ToString("X", CultureInfo.InvariantCulture);
            }
            else
            {
                _rtbChar.Clear();
                _rtbHex.Clear();
            }

            return charCode;
        }

        private int LoadFromHexBox()
        {
            int charCode = -1;
            if (int.TryParse(_rtbHex.Text.Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)
                && result >= 0
                && result <= 0xFFFF)
            {
                charCode = result;
                var ch = (char)charCode;
                _rtbChar.Text = (char.GetUnicodeCategory(ch) == UnicodeCategory.Control) ? "" : ch.ToString();
                _rtbCode.Text = charCode.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                _rtbChar.Clear();
                _rtbCode.Clear();
            }

            return charCode;
        }

        private void LoadItems()
        {
            _lblComment.Text = (_charCode >= 0 && _charCode <= 0xFF)
                ? TextUtility.SplitCamelCase((AsciiChar)_charCode)
                : "";

            _dgv.DataSource = (_charCode != -1)
                ? new SortableBindingList<CharMatchInfo>(
                    CharMatchInfo.Create((char)_charCode, _cbxCharGroup.Checked, Options).ToList())
                : null;
        }

        private int GetCharIndex()
        {
            if (_rtbChar.TextLength == 1)
            {
                return 0;
            }
            else if (_rtbChar.TextLength > 1 && _rtbChar.SelectionLength == 1)
            {
                return _rtbChar.SelectionStart;
            }

            return -1;
        }

        private bool CopyPattern()
        {
            if (_dgv.SelectedRows.Count == 1)
            {
                string pattern = ((CharMatchInfo)_dgv.SelectedRows[0].DataBoundItem).Pattern;
                if (!string.IsNullOrEmpty(pattern))
                {
                    try
                    {
                        Clipboard.SetText(pattern);
                        return true;
                    }
                    catch (ThreadStateException)
                    {
                    }
                    catch (ExternalException)
                    {
                    }
                }
            }

            return false;
        }

        public RegexOptions Options
        {
            get
            {
                var value = RegexOptions.None;

                if (_cbxIgnoreCase.Checked)
                    value |= RegexOptions.IgnoreCase;

                if (_cbxECMAScript.Checked)
                    value |= RegexOptions.ECMAScript;

                return value;
            }
        }
    }
}
