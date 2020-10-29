// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal class SplitBlockIterator : IEnumerable<SplitBlock>
    {
        private readonly SplitTextBuilder _builder;
        private int _startIndex;

        public SplitBlockIterator(SplitTextBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerator<SplitBlock> GetEnumerator()
        {
            _startIndex = 0;
            return CreateBlocks().GetEnumerator();
        }

        private IEnumerable<SplitBlock> CreateBlocks()
        {
            foreach (SplitItem item in _builder.SplitItems)
            {
                var block = new SplitBlock(item, _startIndex, _builder);
                yield return block;
                _startIndex = block.EndIndex + 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
