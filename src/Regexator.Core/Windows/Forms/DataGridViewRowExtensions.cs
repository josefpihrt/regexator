// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class DataGridViewRowExtensions
    {
        public static bool IsFullyDisplayed(this DataGridViewBand row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (row.DataGridView != null)
            {
                return row.DataGridView.GetRowDisplayRectangle(row.Index, false).Height
                    == row.DataGridView.GetRowDisplayRectangle(row.Index, true).Height;
            }

            return false;
        }
    }
}
