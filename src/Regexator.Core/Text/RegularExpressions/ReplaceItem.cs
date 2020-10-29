// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public sealed class ReplaceItem
    {
        internal ReplaceItem(Match match, string resultValue, int resultIndex, int itemIndex)
        {
            Match = match;
            ItemIndex = itemIndex;
            Key = itemIndex.ToString(CultureInfo.CurrentCulture);
            Result = new ReplaceResult(resultValue, resultIndex, this);
        }

        public override string ToString() => Result.ToString();

        public int MatchEndIndex => Match.Index + Match.Length;

        public Match Match { get; }

        public int ItemIndex { get; }

        public string Key { get; }

        public ReplaceResult Result { get; }
    }
}
