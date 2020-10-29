// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class DataGridViewColumnExtensions
    {
        public static bool IsFullyDisplayed(this DataGridViewBand column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            if (column.DataGridView != null)
            {
                return column.DataGridView.GetColumnDisplayRectangle(column.Index, false).Width
                    == column.DataGridView.GetColumnDisplayRectangle(column.Index, true).Width;
            }

            return false;
        }
    }
}
