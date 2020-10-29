// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public class RegexOptionsDataGridView : ExtendedDataGridView
    {
        public RegexOptionsDataGridView()
        {
            Dock = DockStyle.Fill;
            MultiSelect = true;
            RowsDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            RowsDefaultCellStyle.SelectionForeColor = DefaultCellStyle.ForeColor;
            AlternatingRowsDefaultCellStyle = null;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            ColumnHeadersVisible = false;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            BackgroundColor = SystemColors.Window;
            AutoGenerateColumns = false;
            Columns.Add(new DataGridViewCheckBoxColumn() { Name = "Enabled", DataPropertyName = "Enabled", Frozen = true });
            Columns.Add(new DataGridViewTextBoxColumn() { Name = "Text", DataPropertyName = "Text" });
            _hotkeyNumberColumn = new DataGridViewTextBoxColumn() { Name = "RowNumber", ReadOnly = true };
            _hotkeyNumberColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Columns.Add(_hotkeyNumberColumn);
            _descriptionColumn = new DataGridViewTextBoxColumn() { Name = "Description", DataPropertyName = "Description" };
            Columns.Add(_descriptionColumn);
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Enter)
                return true;

            return base.ProcessDataGridViewKey(e);
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                if (Columns[e.ColumnIndex].Name.Equals("Enabled"))
                {
                    DataGridViewRow row = Rows[e.RowIndex];
                    var item = (RegexOptionsItem)row.DataBoundItem;
                    row.DefaultCellStyle.Font = new Font(Font, (item.Enabled) ? FontStyle.Bold : FontStyle.Regular);
                }
                else if (Columns[e.ColumnIndex].Name.Equals("RowNumber"))
                {
                    e.Value = (e.RowIndex + 1).ToString(CultureInfo.CurrentCulture);
                    e.FormattingApplied = true;
                }
            }

            base.OnCellFormatting(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
                _fSpaceUp = true;

            base.OnKeyUp(e);
            _fSpaceUp = false;
        }

        protected override void OnCurrentCellDirtyStateChanged(EventArgs e)
        {
            if (CurrentCell?.OwningColumn.DataPropertyName == "Enabled")
            {
                if (_fSpaceUp)
                {
                    CancelEdit();
                }
                else
                {
                    CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }

            base.OnCurrentCellDirtyStateChanged(e);
        }

        public IEnumerable<RegexOptionsItem> EnumerateSelectedItems()
        {
            return SelectedRows
                .Cast<DataGridViewRow>()
                .Select(f => f.DataBoundItem)
                .Cast<RegexOptionsItem>();
        }

        protected override bool ShowFocusCues
        {
            get { return false; }
        }

        public bool HotkeyNumberColumnVisible
        {
            get { return _hotkeyNumberColumn.Visible; }
            set { _hotkeyNumberColumn.Visible = value; }
        }

        public bool DescriptionColumnVisible
        {
            get { return _descriptionColumn.Visible; }
            set { _descriptionColumn.Visible = value; }
        }

        private bool _fSpaceUp;
        private readonly DataGridViewColumn _hotkeyNumberColumn;
        private readonly DataGridViewColumn _descriptionColumn;
    }
}
