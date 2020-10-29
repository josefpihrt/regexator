// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class DataGridViewExtensions
    {
        public static bool IsContinuousSelection(this DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (dgv.SelectedRows.Count > 1)
            {
                int[] rows = dgv.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(f => f.Index)
                    .OrderBy(f => f)
                    .ToArray();
                return (rows[0] + rows.Length - 1) == rows[rows.Length - 1];
            }

            return true;
        }

        public static void EnsureRowDisplayed(this DataGridView dgv, int rowIndex)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            int cnt = dgv.DisplayedRowCount(false);
            int index = dgv.FirstDisplayedScrollingRowIndex;

            if (index == -1 || cnt == 0 || rowIndex < index)
            {
                index = rowIndex;
            }
            else if (rowIndex > index + cnt - 1)
            {
                index = rowIndex - (cnt - 1);
            }

            if (index != dgv.FirstDisplayedScrollingRowIndex && index != -1)
            {
                DataGridViewRow row = dgv.Rows.GetFirstVisibleNotFrozen(index);

                if (row != null)
                    dgv.FirstDisplayedScrollingRowIndex = row.Index;
            }
        }

        public static void EnsureColumnDisplayed(this DataGridView dgv, int columnIndex)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            int cnt = dgv.DisplayedColumnCount(false);
            int index = dgv.FirstDisplayedScrollingColumnIndex;

            if (index == -1 || cnt == 0 || columnIndex < index)
            {
                index = columnIndex;
            }
            else if (columnIndex > index + cnt - 1)
            {
                index = columnIndex - (cnt - 1);
            }

            if (index != dgv.FirstDisplayedScrollingColumnIndex && index != -1)
            {
                DataGridViewColumn col = dgv.Columns.GetFirstVisibleNotFrozen(index);

                if (col != null)
                    dgv.Columns.GetFirstVisibleNotFrozen(index);
            }
        }

        public static void EnsureCellDisplayed(this DataGridView dgv, DataGridViewCell cell)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            if (!cell.DataGridView.Equals(dgv))
                throw new InvalidOperationException();

            dgv.EnsureRowDisplayed(cell.OwningRow.Index);
            dgv.EnsureColumnDisplayed(cell.OwningColumn.Index);
        }

        public static int SelectNextRow(this DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (dgv.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgv.SelectedRows[dgv.SelectedRows.Count - 1];
                if (row.Index < dgv.Rows.Count - 1)
                {
                    dgv.ClearSelection();
                    dgv.Rows[row.Index + 1].Selected = true;
                }

                return row.Index;
            }

            return -1;
        }

        public static int SelectPreviousRow(this DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (dgv.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgv.SelectedRows[0];
                if (row.Index > 0)
                {
                    dgv.ClearSelection();
                    dgv.Rows[row.Index - 1].Selected = true;
                }

                return row.Index;
            }

            return -1;
        }

        public static bool IsFirstRowSelected(this DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            return dgv.SelectedRows
                .Cast<DataGridViewRow>()
                .Any(f => f.Index == 0);
        }

        public static bool IsLastRowSelected(this DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (dgv.Rows.Count > 0)
            {
                int index = dgv.RowCount - 1;
                return dgv.SelectedRows
                    .Cast<DataGridViewColumn>()
                    .Any(f => f.Index == index);
            }

            return false;
        }

        public static void TrySetCurrentCell(this DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (dgv.Rows.Count > 0)
            {
                foreach (DataGridViewColumn col in dgv.Columns.Cast<DataGridViewColumn>().Where(f => f.Visible)
                    .OrderBy(f => f.DisplayIndex))
                {
                    foreach (DataGridViewRow row in dgv.Rows.Cast<DataGridViewRow>().Where(f => f.Visible))
                    {
                        dgv.CurrentCell = dgv[col.Index, row.Index];
                        return;
                    }
                }
            }
        }
    }
}
