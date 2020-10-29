// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.Snippets
{
    public partial class CodeSense<T> : UserControl where T : SenseItem
    {
        public CodeSense()
        {
            InitializeComponent();
            Sorter = new SenseItemSorter();
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

            _dgv = new SenseDataGridView()
            {
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            _dgv.AlternatingRowsDefaultCellStyle = _dgv.DefaultCellStyle;
            _dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            _dgv.AllowUserToAddRows = false;
            _dgv.AllowUserToDeleteRows = false;
            _dgv.ColumnHeadersVisible = false;
            _dgv.RowHeadersVisible = false;
            _dgv.ReadOnly = true;
            _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgv.ScrollBars = ScrollBars.Vertical;
            _dgv.BackgroundColor = SystemColors.Window;
            _dgv.MultiSelect = false;
            _dgv.AutoGenerateColumns = false;
            _dgv.RowTemplate.Height = _dgv.DefaultCellStyle.Font.Height + 2;
            _dgv.Location = new Point(0, 20);
            _dgv.Dock = DockStyle.Fill;
            _dgv.Name = "_dgv";
            _dgv.Size = new Size(350, 230);
            _dgv.StandardTab = true;
            _dgv.TabIndex = 1;

            var col = new DataGridViewImageColumn(true)
            {
                Name = "Icon",
                DataPropertyName = "Icon",
                Icon = Resources.IcoSnippet,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader
            };

            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            _dgv.Columns.Add(col);
            _dgv.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = "Text",
                    DataPropertyName = "Text",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
            _dgv.MouseDoubleClick += (f, f2) => DataGridView_MouseDoubleClick(f, f2);
            _dgv.DataBindingComplete += (f, f2) => DataGridView_DataBindingComplete(f, f2);
            _dgv.CellContextMenuStripNeeded += (f, f2) => DataGridView_CellContextMenuStripNeeded(f, f2);
            _dgv.KeyDown += (f, f2) => DataGridView_KeyDown(f, f2);
            _dgv.CellFormatting += (f, f2) => DataGridView_CellFormatting(f, f2);
            _favoriteFont = new Font(_dgv.DefaultCellStyle.Font, FontStyle.Bold);

            _pnl = new Panel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            _pnl.Controls.Add(_dgv);
            Controls.Add(_pnl);
            Controls.Add(_tbx);
            ResumeLayout(true);
        }

        public void Show(T[] items)
        {
            Show(items, "");
        }

        public void Show(IEnumerable<T> items, string searchText)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = items
                .Where(f => f.Visible)
                .ToList();

            CreateItems();
            _tbx.Text = searchText;

            if (string.IsNullOrEmpty(searchText))
                LoadItems();

            Visible = true;
            Select();
            ActiveControl = _tbx;
            _tbx.Select(_tbx.Text.Length, 0);
        }

        private void CreateItems()
        {
            IEnumerable<T> items = _items.Where(f => f.Visible);

            if (Sorter != null)
                items = items.OrderBy(f => f, Sorter);

            _items2 = items.ToList();
        }

        private void LoadItems()
        {
            _dgv.DataSource = null;
            _dgv.DataSource = (string.IsNullOrEmpty(_tbx.Text))
                ? _items2.Where(f => f.IsMatch(_tbx.Text)).ToList()
                : _items.Where(f => f.IsMatch(_tbx.Text)).ToList();
        }

        private void TerminateCancel()
        {
            OnTerminate(EventArgs.Empty);
        }

        private void TerminateSuccess()
        {
            TerminateSuccess(false);
        }

        private void TerminateSuccess(bool checkExtendedKey)
        {
            SelectedItem = _dgv.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(f => f.DataBoundItem)
                .Cast<T>()
                .FirstOrDefault();

            if (SelectedItem?.IsExtensible == true && checkExtendedKey && (ModifierKeys & Keys.Shift) == Keys.Shift)
                SelectedItem.UseExtended = true;

            Success = true;
            OnTerminate(EventArgs.Empty);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                if (e.KeyCode == Keys.Down)
                {
                    _dgv.SelectNextRow();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    _dgv.SelectPreviousRow();
                    e.SuppressKeyPress = true;
                }
            }

            ProcessKeyDown(e);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            LoadItems();
        }

        private void DataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_dgv.SelectedRows.Count > 0)
                TerminateSuccess(true);
        }

        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var items = (List<T>)_dgv.DataSource;
            int index = items.FindIndex(f => f.Text.StartsWith(_tbx.Text, StringComparison.CurrentCultureIgnoreCase));
            if (index != -1)
            {
                _dgv.ClearSelection();
                _dgv.Rows[index].Selected = true;
                _dgv.FirstDisplayedScrollingRowIndex = Math.Max(index - (_dgv.DisplayedRowCount(false) / 2), 0);
            }
        }

        private void DataGridView_CellContextMenuStripNeeded(
            object sender,
            DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            var cms = new ContextMenuStrip();
            var item = (SenseItem)_dgv.Rows[e.RowIndex].DataBoundItem;
            var tsi = new ToolStripMenuItem(Resources.Favorite)
            {
                CheckOnClick = true,
                Tag = item,
                Checked = item.Favorite,
                ShortcutKeyDisplayString = Resources.CtrlD
            };
            tsi.CheckedChanged += (f, f2) => FavoriteItem_CheckedChanged(f, f2);
            cms.Items.Add(tsi);
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add(new ToolStripMenuItem(Resources.Insert, null, (f, f2) => InsertItem_Click(f, f2))
                {
                    ShortcutKeyDisplayString = Resources.Tab
                });
            if (item.IsExtensible)
            {
                var tsiInsertExtended = new ToolStripMenuItem(
                    Resources.InsertExtended + " " + item.ExtendedKind.GetDescription().AddParentheses())
                {
                    ShortcutKeyDisplayString = Resources.ShiftTab
                };

                tsiInsertExtended.Click += delegate
                {
                    item.UseExtended = true;
                    TerminateSuccess();
                };

                cms.Items.Add(tsiInsertExtended);
            }

            e.ContextMenuStrip = cms;
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Up && _dgv.IsFirstRowSelected())
            {
                ActiveControl = _tbx;
            }

            ProcessKeyDown(e);
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex != -1 && ((T)_dgv.Rows[e.RowIndex].DataBoundItem).Favorite)
                e.CellStyle.Font = _favoriteFont;
        }

        private void FavoriteItem_CheckedChanged(object sender, EventArgs e)
        {
            var tsi = (ToolStripMenuItem)sender;
            var item = (SenseItem)tsi.Tag;
            item.Favorite = tsi.Checked;
            CreateItems();
        }

        private void InsertItem_Click(object sender, EventArgs e)
        {
            TerminateSuccess();
        }

        private void ProcessKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    TerminateSuccess(true);
                    e.SuppressKeyPress = true;
                }
                else
                {
                    TerminateSuccess(true);
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                TerminateSuccess(true);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                TerminateCancel();
                e.SuppressKeyPress = true;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
            {
                if (_dgv.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = _dgv.SelectedRows[0];
                    var item = (SenseItem)row.DataBoundItem;
                    item.Favorite = !item.Favorite;
                    _dgv.InvalidateRow(row.Index);
                    CreateItems();
                }

                e.SuppressKeyPress = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                SelectedItem = null;
                Success = false;
            }

            base.OnVisibleChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            _tbx.Font = Font;
            _dgv.DefaultCellStyle.Font = Font;
            _dgv.RowTemplate.Height = _dgv.DefaultCellStyle.Font.Height + 2;
            _favoriteFont = new Font(_dgv.DefaultCellStyle.Font, FontStyle.Bold);
            base.OnFontChanged(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            _tbx.BackColor = BackColor;
            _dgv.DefaultCellStyle.BackColor = BackColor;
            base.OnBackColorChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            _tbx.ForeColor = ForeColor;
            _dgv.DefaultCellStyle.ForeColor = ForeColor;
            base.OnForeColorChanged(e);
        }

        protected virtual void OnTerminate(EventArgs e)
        {
            Terminate?.Invoke(this, e);
        }

        protected override Size DefaultMaximumSize
        {
            get { return new Size(300, 250); }
        }

        public bool Success { get; private set; }
        public T SelectedItem { get; private set; }
        public IComparer<SenseItem> Sorter { get; set; }

        private readonly SenseDataGridView _dgv;
        private readonly SenseTextBox _tbx;
        private readonly Panel _pnl;

        private List<T> _items;
        private List<T> _items2;
        private Font _favoriteFont;

        public event EventHandler Terminate;
    }
}
