// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.UI
{
    public class ErrorInfoDataGridView : ExtendedDataGridView
    {
        public ErrorInfoDataGridView()
        {
            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.None;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            ReadOnly = true;
            AutoGenerateColumns = true;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            RowHeadersVisible = false;
            BackgroundColor = SystemColors.Window;
            AlternatingRowsDefaultCellStyle = DefaultCellStyle;
        }

        protected override void OnCellContextMenuStripNeeded(DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.RowIndex != -1)
            {
                var item = (ErrorInfo)Rows[e.RowIndex].DataBoundItem;
                if (File.Exists(item.FullName))
                {
                    var cms = new ContextMenuStrip();
                    cms.Items.Add(new ToolStripMenuItem(
                        Resources.OpenInExplorer,
                        null,
                        (object sender, EventArgs e2) => FileSystem.FileSystemUtility.OpenPathInExplorer(item.FullName)));
                    e.ContextMenuStrip = cms;
                }
            }

            base.OnCellContextMenuStripNeeded(e);
        }
    }
}
