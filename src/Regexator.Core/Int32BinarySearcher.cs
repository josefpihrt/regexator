// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Regexator
{
    public class Int32BinarySearcher : BinarySearcher<int>
    {
        public Int32BinarySearcher()
        {
        }

        public Int32BinarySearcher(IEnumerable<int> items)
            : base(items)
        {
        }

        protected override int DefaultValue
        {
            get { return -1; }
        }

        public static readonly Int32BinarySearcher Empty = new Int32BinarySearcher();
    }
}
