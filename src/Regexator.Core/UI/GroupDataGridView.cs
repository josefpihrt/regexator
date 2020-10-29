// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Regexator.Collections.Generic;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public class GroupDataGridView : ExtendedDataGridView
    {
        public GroupDataGridView()
        {
            Dock = DockStyle.Fill;
            MultiSelect = true;
            RowsDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            RowsDefaultCellStyle.SelectionForeColor = DefaultCellStyle.ForeColor;
            AlternatingRowsDefaultCellStyle = null;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            ColumnHeadersVisible = true;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            BackgroundColor = SystemColors.Window;
            AutoGenerateColumns = false;
            AddColumns();
        }

        private void AddColumns()
        {
            _enabledColumn = new DataGridViewCheckBoxColumn()
            {
                Name = "Enabled",
                DataPropertyName = "Enabled",
                HeaderText = "",
                Frozen = true,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            var indexColumn = new DataGridViewTextBoxColumn()
            {
                Name = "Index",
                DataPropertyName = "Index",
                HeaderText = Resources.NumberStr
            };
            indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            Columns.Add(_enabledColumn);
            Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = "Name",
                    DataPropertyName = "Name",
                    HeaderText = Resources.Name.ToLower(CultureInfo.CurrentCulture)
                });
            Columns.Add(indexColumn);
        }

        public GroupInfoItem FindGroupInfo(int groupIndex)
        {
            return Rows
                .Cast<DataGridViewRow>()
                .Select(f => f.DataBoundItem)
                .Cast<GroupInfoItem>()
                .FirstOrDefault(f => f.Index == groupIndex);
        }

        private void SetFont(DataGridViewRow row)
        {
            SetFont(row, IsBold((GroupInfoItem)row.DataBoundItem));
        }

        private void SetFont(DataGridViewRow row, bool bold)
        {
            Font font = row.DefaultCellStyle.Font;
            if (font == null || (((font.Style & FontStyle.Bold) == FontStyle.Bold) != bold))
                row.DefaultCellStyle.Font = new Font(Font, (bold) ? FontStyle.Bold : FontStyle.Regular);
        }

        private bool IsBold(GroupInfoItem info)
        {
            switch (Mode)
            {
                case EvaluationMode.Match:
                    return info.Enabled;
                case EvaluationMode.Split:
                    return info.Index != 0;
                case EvaluationMode.Replace:
                    return info.Index == 0;
            }

            return false;
        }

        public IEnumerable<GroupInfoItem> SelectedGroupInfos()
        {
            return SelectedRows
                .Cast<DataGridViewRow>()
                .Select(f => f.DataBoundItem)
                .Cast<GroupInfoItem>();
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Enter)
                return true;

            return base.ProcessDataGridViewKey(e);
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            CurrentCell = null;
            _horizontalScrollingOffset = HorizontalScrollingOffset;
            base.OnColumnHeaderMouseClick(e);
        }

        protected override void OnSorted(EventArgs e)
        {
            HorizontalScrollingOffset = _horizontalScrollingOffset;
            base.OnSorted(e);
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (Columns[e.ColumnIndex].Name.Equals("Enabled"))
            {
                DataGridViewRow row = Rows[e.RowIndex];
                SetFont(row, ((GroupInfoItem)row.DataBoundItem).Enabled);
            }

            base.OnCellFormatting(e);
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                SetFont(Rows[i]);

            base.OnRowsAdded(e);
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

        protected override bool ShowFocusCues
        {
            get { return false; }
        }

        public GroupInfoItem SelectedGroupInfo
        {
            get { return SelectedGroupInfos().FirstOrDefault(); }
        }

        public EvaluationMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    _enabledColumn.Visible = (Mode == EvaluationMode.Match);
                    Rows.Cast<DataGridViewRow>().ForEach(f => SetFont(f));
                }
            }
        }

        private EvaluationMode _mode;
        private DataGridViewColumn _enabledColumn;
        private bool _fSpaceUp;
        private int _horizontalScrollingOffset;
    }
}
