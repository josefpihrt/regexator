// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Regexator.Text.RegularExpressions
{
    [Serializable]
    public class GroupInfoSorter : IComparer<GroupInfo>
    {
        public GroupInfoSorter()
            : this(GroupSettings.DefaultSortProperty, GroupSettings.DefaultSortDirection)
        {
        }

        public GroupInfoSorter(GroupSortProperty sortPropertyName, ListSortDirection sortDirection)
        {
            SortPropertyName = sortPropertyName;
            SortDirection = sortDirection;
        }

        public int Compare(GroupInfo x, GroupInfo y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            int value = (SortPropertyName == GroupSortProperty.Name)
                ? string.Compare(x.Name, y.Name, StringComparison.CurrentCulture)
                : x.Index.CompareTo(y.Index);

            return (SortDirection == ListSortDirection.Ascending) ? value : -value;
        }

        public GroupSortProperty SortPropertyName { get; }

        public ListSortDirection SortDirection { get; }
    }
}
