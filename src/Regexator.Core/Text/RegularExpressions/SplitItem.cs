// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator.Text.RegularExpressions
{
    public abstract class SplitItem
    {
        protected SplitItem()
        {
        }

        public override string ToString() => Value;

        public abstract string Value { get; }

        public abstract int Index { get; }

        public abstract int Length { get; }

        public abstract int ItemIndex { get; }

        public abstract string Name { get; }

        public abstract SplitItemKind Kind { get; }

        public abstract string Key { get; }

        public virtual GroupInfo GroupInfo => null;
    }
}
