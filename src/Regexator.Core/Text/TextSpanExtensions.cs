// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Regexator.Text
{
    public static class TextSpanExtensions
    {
        public static TextSpan Combine(this IEnumerable<TextSpan> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            int index = items.Min(f => f.Index);
            return new TextSpan(index, items.Max(f => f.EndIndex) - index);
        }

        public static IEnumerable<TextSpan> Offset(this IEnumerable<TextSpan> items, int indexOffset)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (indexOffset != 0)
                return items.Select(f => f.Offset(indexOffset));

            return items;
        }
    }
}
