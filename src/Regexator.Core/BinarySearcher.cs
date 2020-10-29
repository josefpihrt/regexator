// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Regexator
{
    public class BinarySearcher<T> where T : IComparable<T>
    {
        private readonly List<T> _items;
        private readonly IComparer<T> _comparer;

        public BinarySearcher()
            : this(Enumerable.Empty<T>())
        {
        }

        public BinarySearcher(IEnumerable<T> items)
            : this(items, Comparer<T>.Default)
        {
        }

        public BinarySearcher(IEnumerable<T> items, IComparer<T> comparer)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = new List<T>(items);
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public int FindIndex(T item)
        {
            if (_items.Count > 0)
            {
                int result = _items.BinarySearch(item, _comparer);
                if (result < 0)
                {
                    if (result == -1)
                        return ~result;

                    return ~result - 1;
                }

                return result;
            }

            return -1;
        }

        public T Find(T item)
        {
            int index = FindIndex(item);

            if (index != -1)
                return _items[index];

            return DefaultValue;
        }

        public IndexSearcherResult<T> FindItems(T item)
        {
            int i = FindIndex(item);
            if (i != -1)
            {
                return new IndexSearcherResult<T>(
                    _items[i],
                    (i > 0) ? _items[i - 1] : DefaultValue,
                    (i < _items.Count - 1) ? _items[i + 1] : DefaultValue);
            }

            return null;
        }

        public T FirstItem
        {
            get { return (_items.Count > 0) ? _items[0] : DefaultValue; }
        }

        public T LastItem
        {
            get { return (_items.Count > 0) ? _items[_items.Count - 1] : DefaultValue; }
        }

        protected virtual T DefaultValue
        {
            get { return default; }
        }
    }
}
