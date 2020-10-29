// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text.RegularExpressions
{
    public class ReplaceResult
    {
        internal ReplaceResult(string value, int index, ReplaceItem replaceItem)
        {
            Value = value;
            Index = index;
            ReplaceItem = replaceItem;
        }

        public override string ToString() => Value;

        public string Value { get; }

        public int Index { get; }

        public int Length => Value.Length;

        public int EndIndex => Index + Value.Length;

        public ReplaceItem ReplaceItem { get; }
    }
}
