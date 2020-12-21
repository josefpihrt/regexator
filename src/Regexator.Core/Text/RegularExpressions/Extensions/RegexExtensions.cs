// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public static class RegexExtensions
    {
        public static MatchItem MatchItem(this Regex regex, string input)
        {
            return new MatchData(regex, input).MatchItem();
        }

        public static MatchItem MatchItem(this Regex regex, string input, int limit)
        {
            return new MatchData(regex, input, limit).MatchItem();
        }

        public static MatchItemCollection MatchItems(this Regex regex, string input)
        {
            return new MatchData(regex, input).Items;
        }

        public static MatchItemCollection MatchItems(this Regex regex, string input, int limit)
        {
            return new MatchData(regex, input, limit).Items;
        }

        public static ReplaceItemCollection ReplaceItems(this Regex regex, string input)
        {
            return new ReplaceData(regex, input).Items;
        }

        public static ReplaceItemCollection ReplaceItems(this Regex regex, string input, string replacement)
        {
            return new ReplaceData(regex, input, replacement).Items;
        }

        public static ReplaceItemCollection ReplaceItems(
            this Regex regex,
            string input,
            string replacement,
            ReplacementSettings settings)
        {
            return new ReplaceData(regex, input, replacement, settings).Items;
        }

        public static SplitItemCollection SplitItems(this Regex regex, string input)
        {
            return new SplitData(regex, input).Items;
        }

        public static SplitItemCollection SplitItems(this Regex regex, string input, int limit)
        {
            return new SplitData(regex, input, limit).Items;
        }

        public static IEnumerable<string> EnumerateSplit(this Regex regex, string input)
        {
            return RegexSplit.EnumerateValues(regex, input);
        }

        public static IEnumerable<string> EnumerateSplit(this Regex regex, string input, SplitOptions splitOptions)
        {
            return RegexSplit.EnumerateValues(regex, input, splitOptions);
        }
    }
}
