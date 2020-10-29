// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Regexator.Snippets
{
    public class SenseItemSorter : IComparer<SenseItem>
    {
        public int Compare(SenseItem x, SenseItem y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            if (x.Favorite && y.Favorite)
                return string.Compare(x.Text, y.Text, StringComparison.CurrentCulture);

            if (x.Favorite)
                return -1;

            if (y.Favorite)
                return 1;

            int value = x.Rank.CompareTo(y.Rank);
            if (value == 0)
                return string.Compare(x.Text, y.Text, StringComparison.CurrentCulture);

            return -value;
        }
    }
}
