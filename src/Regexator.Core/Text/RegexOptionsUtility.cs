// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Regexator.Collections.Generic;

namespace Regexator.Text
{
    [DebuggerStepThrough]
    public static class RegexOptionsUtility
    {
        public static RegexOptions Union(this RegexOptions options, RegexOptions value)
        {
            return options | value;
        }

        public static RegexOptions Except(this RegexOptions options, RegexOptions value)
        {
            return options & ~value;
        }

        public static RegexOptions Intersect(this RegexOptions options, RegexOptions value)
        {
            return options & value;
        }

        public static bool Contains(this RegexOptions options, RegexOptions value)
        {
            return options.Intersect(value) == value;
        }

        public static bool ContainsAny(this RegexOptions options, RegexOptions value)
        {
            return options.Intersect(value) != RegexOptions.None;
        }

        public static bool Any(this RegexOptions options)
        {
            return options != RegexOptions.None;
        }

        public static IEnumerable<RegexOptions> ToValues(this RegexOptions options)
        {
            return Values.Where(f => ((options & f) == f));
        }

        public static IEnumerable<string> ToNames(this RegexOptions options)
        {
            return options.ToValues().Select(f => f.ToString());
        }

        public static RegexOptions GetValue(this IEnumerable<RegexOptions> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var result = RegexOptions.None;

            foreach (RegexOptions value in values)
                result |= value;

            return result;
        }

        public static string Format(RegexOptions value)
        {
            return Format(value, ", ");
        }

        public static string Format(RegexOptions value, string separator)
        {
            return Format(value, separator, false);
        }

        public static string Format(RegexOptions value, string separator, bool insertSpace)
        {
            return string.Join(
                separator,
                ((value == RegexOptions.None)
                        ? new[] { RegexOptions.None }
                        : value.ToValues())
                    .Select(f => (insertSpace)
                        ? TextUtility.SplitCamelCase(f)
                        : f.ToString())
                    .OrderBy(f => f));
        }

        public static readonly ReadOnlyCollection<RegexOptions> Values = Enum.GetValues(typeof(RegexOptions))
            .Cast<RegexOptions>()
            .Where(f => f != RegexOptions.None)
            .OrderBy(f => f.ToString())
            .ToReadOnly();

        public static readonly RegexOptions Value = Values.GetValue();
    }
}
