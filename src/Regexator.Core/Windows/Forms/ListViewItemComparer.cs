// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ListViewItemComparer : IComparer
    {
        public ListViewItemComparer()
            : this(-1, ListSortDirection.Ascending)
        {
        }

        public ListViewItemComparer(int columnIndex, ListSortDirection sortDirection)
        {
            ColumnIndex = columnIndex;
            SortDirection = sortDirection;
        }

        public int Compare(object x, object y)
        {
            var i1 = (ListViewItem)x;
            var i2 = (ListViewItem)y;

            if (ColumnIndex > -1 && ColumnIndex < i1.SubItems.Count && ColumnIndex < i2.SubItems.Count)
                return CompareOverride(i1, i2);

            return 1;
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        protected virtual int CompareOverride(ListViewItem x, ListViewItem y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (SortDirection == ListSortDirection.Ascending)
            {
                return string.Compare(
                    x.SubItems[ColumnIndex].Text,
                    y.SubItems[ColumnIndex].Text,
                    StringComparison.CurrentCulture);
            }

            return -string.Compare(
                x.SubItems[ColumnIndex].Text,
                y.SubItems[ColumnIndex].Text,
                StringComparison.CurrentCulture);
        }

        public int ColumnIndex { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }
}