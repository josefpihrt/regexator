// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Regexator.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static bool CountExceeds<T>(this IEnumerable<T> collection, int value)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (value == 0)
                return false;

            int cnt = 0;
            using (IEnumerator<T> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    cnt++;
                    if (cnt == value)
                        return en.MoveNext();
                }
            }

            return false;
        }

        public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T> collection) where T : class
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return collection.Where(f => f != null);
        }

        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return Array.AsReadOnly(collection.ToArray());
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (T item in collection)
                action(item);
        }

        public static IEnumerable<T> ToClones<T>(this IEnumerable<T> collection) where T : ICloneable
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return collection.Cast<ICloneable>().Select(f => f.Clone()).Cast<T>();
        }

        public static StringCollection ToStringCollection<T>(this IEnumerable<T> collection)
        {
            return ToStringCollection(collection, f => f.ToString());
        }

        public static StringCollection ToStringCollection<T>(this IEnumerable<T> collection, Func<T, string> selector)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var items = new StringCollection();
            items.AddRange(collection.Select(selector).ToArray());
            return items;
        }

        public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return Interleave();

            IEnumerable<T> Interleave()
            {
                using (IEnumerator<T> enumerator1 = first.GetEnumerator())
                using (IEnumerator<T> enumerator2 = second.GetEnumerator())
                {
                    while (enumerator1.MoveNext())
                    {
                        yield return enumerator1.Current;

                        if (enumerator2.MoveNext())
                            yield return enumerator2.Current;
                    }
                }
            }
        }

        public static double WeightedAverage<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double> value,
            Func<TSource, double> weight)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (weight == null)
                throw new ArgumentNullException(nameof(weight));

            double x = source.Sum(f => value(f) * weight(f));
            double y = source.Sum(f => weight(f));
            return x / y;
        }

        public static double Median(this IEnumerable<double> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!source.Any())
                throw new InvalidOperationException();

            double[] sorted = source.OrderBy(f => f).ToArray();
            int cnt = sorted.Length;
            int index = cnt / 2;

            if (cnt % 2 == 0)
                return (sorted[index] + sorted[index - 1]) / (double)2;

            return sorted[index];
        }

        public static double Median(this IEnumerable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!source.Any())
                throw new InvalidOperationException();

            int[] sorted = source.OrderBy(f => f).ToArray();
            int cnt = sorted.Length;
            int index = cnt / 2;

            if (cnt % 2 == 0)
                return (sorted[index] + sorted[index - 1]) / (double)2;

            return sorted[index];
        }

        public static bool SequenceEqualUnordered<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return SequenceEqualUnordered(first, second, EqualityComparer<T>.Default);
        }

        public static bool SequenceEqualUnordered<T>(
            this IEnumerable<T> first,
            IEnumerable<T> second,
            IEqualityComparer<T> comparer)
        {
            int cnt1 = (first is ICollection<T> coll1) ? coll1.Count : first.Count();
            int cnt2 = (second is ICollection<T> coll2) ? coll2.Count : second.Count();

            if (cnt1 == cnt2)
                return first.Intersect(second, comparer).Count() == cnt1;

            return false;
        }

        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int start, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Skip(start).Take(count);
        }

        public static IEnumerable<TResult[]> ToBatches<TResult>(this IEnumerable<TResult> source, int size)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size));

            return ToBatches();

            IEnumerable<TResult[]> ToBatches()
            {
                using (IEnumerator<TResult> enumerator = source.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var batch = new TResult[size];
                        batch[0] = enumerator.Current;

                        for (int i = 1; i < size && enumerator.MoveNext(); i++)
                            batch[i] = enumerator.Current;

                        yield return batch;
                    }
                }
            }
        }
    }
}
