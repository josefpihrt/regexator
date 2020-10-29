// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class ListViewExtensions
    {
        public static void AutoResizeAllColumns(this ListView lsv)
        {
            if (lsv == null)
                throw new ArgumentNullException(nameof(lsv));

            if (lsv.Items.Count > 0)
            {
                lsv.BeginUpdate();

                foreach (ColumnHeader column in lsv.Columns)
                    column.Width = -1;

                lsv.EndUpdate();
            }
        }

        public static ListViewItem SelectedItem(this ListView lsv)
        {
            if (lsv == null)
                throw new ArgumentNullException(nameof(lsv));

            return (lsv.SelectedItems.Count > 0) ? lsv.SelectedItems[0] : null;
        }

        public static bool TrySelectFirstItem(this ListView lsv)
        {
            if (lsv == null)
                throw new ArgumentNullException(nameof(lsv));

            if (lsv.Items.Count > 0)
            {
                lsv.Items[0].Selected = true;
                return true;
            }

            return false;
        }
    }
}
