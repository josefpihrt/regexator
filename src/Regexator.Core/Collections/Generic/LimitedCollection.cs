// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Regexator.Collections.Generic
{
    public sealed class LimitedCollection<T> : Collection<T>
    {
        [DebuggerStepThrough]
        public LimitedCollection(int maxCount)
        {
            MaxCount = maxCount;
        }

        [DebuggerStepThrough]
        public LimitedCollection(IList<T> list, int maxCount)
            : base(list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > maxCount)
                throw new ArgumentOutOfRangeException(nameof(maxCount));

            MaxCount = maxCount;
        }

        protected override void InsertItem(int index, T item)
        {
            if (index == 0)
            {
                if (Count == MaxCount)
                    RemoveAt(0);

                base.InsertItem(index, item);
            }
            else
            {
                base.InsertItem(index, item);

                if (Count > MaxCount)
                    RemoveAt(0);
            }
        }

        public int MaxCount
        {
            get { return _maxCount; }
            private set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");

                _maxCount = value;
            }
        }

        private int _maxCount;
    }
}
