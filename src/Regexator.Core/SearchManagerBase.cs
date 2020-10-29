// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Regexator.Collections.Generic;

namespace Regexator
{
    public abstract class SearchManagerBase<T>
    {
        protected SearchManagerBase(T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            IgnoreCase = true;
            Items = Array.AsReadOnly(items);
            FilterValue = null;
        }

        public abstract bool Predicate(T item);

        public string FilterValue
        {
            get { return _filterValue; }
            set
            {
                _filterValue = value ?? "";
                IsFilterValueEmpty = string.IsNullOrEmpty(_filterValue);
                FilteredItems = (IsFilterValueEmpty) ? Items : Items.Where(f => Predicate(f)).ToReadOnly();
            }
        }

        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set
            {
                if (_ignoreCase != value)
                {
                    _ignoreCase = value;
                    Comparison = ((IgnoreCase)
                        ? StringComparison.CurrentCultureIgnoreCase
                        : StringComparison.CurrentCulture);
                }
            }
        }

        public StringComparison Comparison { get; private set; }
        public ReadOnlyCollection<T> Items { get; }
        public ReadOnlyCollection<T> FilteredItems { get; private set; }
        public bool IsFilterValueEmpty { get; private set; }

        private string _filterValue;
        private bool _ignoreCase;
    }
}
