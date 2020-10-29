// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Regexator.IO
{
    public sealed class SearchResultEqualityComparer : EqualityComparer<SearchResult>
    {
        public override bool Equals(SearchResult x, SearchResult y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(SearchResult obj)
        {
            if (obj == null || obj.FullName == null)
                return 0;

            return obj.FullName.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
        }
    }
}
