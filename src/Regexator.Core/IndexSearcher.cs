// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Regexator.Output
{
    public class IndexSearcher : BinarySearcher<IndexSearcherItem>
    {
        public IndexSearcher()
        {
        }

        public IndexSearcher(IEnumerable<IndexSearcherItem> items)
            : base(items)
        {
        }

        public static IndexSearcher CreateInputSearcher(IEnumerable<RegexBlock> blocks)
        {
            if (blocks == null)
                throw new ArgumentNullException(nameof(blocks));

            return CreateSearcher(blocks, f => f.InputSpan.Index);
        }

        public static IndexSearcher CreateOutputSearcher(IEnumerable<RegexBlock> blocks)
        {
            if (blocks == null)
                throw new ArgumentNullException(nameof(blocks));

            return CreateSearcher(blocks, f => f.StartIndex);
        }

        internal static IndexSearcher CreateSearcher(IEnumerable<RegexBlock> blocks, Func<RegexBlock, int> selector)
        {
            return new IndexSearcher(blocks
                .Select(f => new IndexSearcherItem(selector(f), f))
                .OrderBy(f => f.Index)
                .Distinct());
        }

        public IndexSearcherResult<IndexSearcherItem> FindItems(int index)
        {
            return FindItems(new IndexSearcherItem(index));
        }

        public IndexSearcherItem FindItem(int index)
        {
            return Find(new IndexSearcherItem(index));
        }

        public static readonly IndexSearcher Empty = new IndexSearcher();
    }
}
