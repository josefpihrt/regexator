// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ExtendedListView : ListView
    {
        public ExtendedListView()
        {
            View = View.Details;
            MultiSelect = false;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            DoubleBuffered = true;
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (CustomListViewItemSorter != null)
            {
                if (e.Column != CustomListViewItemSorter.ColumnIndex)
                {
                    CustomListViewItemSorter.ColumnIndex = e.Column;
                    CustomListViewItemSorter.SortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    CustomListViewItemSorter.SortDirection = (CustomListViewItemSorter
                        .SortDirection == ListSortDirection.Ascending)
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }

                Sort();
            }

            base.OnColumnClick(e);
        }

        public ListViewItemComparer CustomListViewItemSorter
        {
            get { return ListViewItemSorter as ListViewItemComparer; }
            set { ListViewItemSorter = value; }
        }
    }
}