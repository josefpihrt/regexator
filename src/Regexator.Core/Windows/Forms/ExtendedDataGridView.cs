// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ExtendedDataGridView : DataGridView
    {
        public ExtendedDataGridView()
        {
            DoubleBuffered = true;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            StandardTab = true;
            AllowUserToResizeRows = false;
            BackgroundColor = SystemColors.Control;
            EnableHeadersVisualStyles = false;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShowCellToolTips = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Button == MouseButtons.Right)
            {
                HitTestInfo info = HitTest(e.X, e.Y);

                if (info.Type == DataGridViewHitTestType.Cell
                    && !Rows[info.RowIndex].Selected)
                {
                    ClearSelection();
                    DataGridViewRow row = Rows[info.RowIndex];
                    row.Selected = true;
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (row.Cells[i].Visible)
                        {
                            CurrentCell = row.Cells[i];
                            break;
                        }
                    }
                }
            }

            base.OnMouseDown(e);
        }
    }
}
