// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using Regexator.Windows.Forms;

namespace Regexator.Snippets
{
    public class SenseDataGridView : ExtendedDataGridView
    {
        protected override void OnSelectionChanged(EventArgs e)
        {
            if (SelectedRows.Count > 0)
                this.EnsureRowDisplayed(SelectedRows[0].Index);

            base.OnSelectionChanged(e);
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                return true;

            return base.ProcessDataGridViewKey(e);
        }
    }
}
