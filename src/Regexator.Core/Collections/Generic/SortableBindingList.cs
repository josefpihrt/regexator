// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Regexator.Collections.Generic
{
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection;
        private PropertyDescriptor _sortProperty;

        public SortableBindingList()
        {
        }

        public SortableBindingList(IList<T> list)
            : base(list)
        {
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection sortDirection)
        {
            _sortProperty = prop;
            _sortDirection = sortDirection;

            if (prop == null)
                throw new ArgumentNullException(nameof(prop));

            if (!(Items is List<T> items))
            {
                RemoveSortCore();
                return;
            }

            items.Sort((f, f2) => Compare(f, f2));

            _isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private int Compare(T first, T second)
        {
            int result = CompareInternal(first, second);

            return (_sortDirection == ListSortDirection.Ascending) ? result : -result;
        }

        private int CompareInternal(T first, T second)
        {
            object x = (first != null) ? _sortProperty.GetValue(first) : null;
            object y = (second != null) ? _sortProperty.GetValue(second) : null;

            if (ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return (y != null) ? -1 : 0;

            if (y == null)
                return 1;

            if (x is IComparable comparable)
                return comparable.CompareTo(y);

            string sx = x.ToString();
            string sy = y.ToString();

            if (sx == null)
                return (sy != null) ? -1 : 0;

            if (sy == null)
                return 1;

            return string.Compare(sx, sy, StringComparison.CurrentCulture);
        }

        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
            _isSorted = false;
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }
    }
}
