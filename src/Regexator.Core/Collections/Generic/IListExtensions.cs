// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Regexator.Collections.Generic
{
    public static class IListExtensions
    {
        public static IEnumerable<T> EnumerateFrom<T>(this IList<T> list, int startIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return EnumerateFrom();

            IEnumerable<T> EnumerateFrom()
            {
                for (int i = startIndex; i < list.Count; i++)
                    yield return list[i];
            }
        }

        public static IEnumerable<T> ReversedFrom<T>(this IList<T> list, int startIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return ReversedFrom();

            IEnumerable<T> ReversedFrom()
            {
                for (int i = startIndex; i >= 0; i--)
                    yield return list[i];
            }
        }

        public static IEnumerable<T> Reversed<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return ReversedFrom(list, list.Count - 1);
        }

        public static IEnumerable<TResult> ToArray<TSource, TResult>(
            this IList<TSource> list,
            Func<TSource, TResult> selector)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var results = new TResult[list.Count];

            for (int i = 0; i < list.Count; i++)
                results[i] = selector(list[i]);

            return results;
        }

        public static IEnumerable<TResult> ToList<TSource, TResult>(
            this IList<TSource> list,
            Func<TSource, TResult> selector)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var results = new List<TResult>(list.Count);

            for (int i = 0; i < list.Count; i++)
                results[i] = selector(list[i]);

            return results;
        }
    }
}
