// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;

namespace Regexator.IO
{
    public class SearchResultSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            var second = y as SearchResult;

            if (!(x is SearchResult first))
            {
                if (second != null)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            if (second == null)
            {
                if (first != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            return string.Compare(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
