// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Regexator.Collections.Generic
{
    public static class ICollectionExtensions
    {
        public static void AddItems<TSource>(this ICollection<TSource> collection, IEnumerable<TSource> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (TSource item in items)
                collection.Add(item);
        }
    }
}
