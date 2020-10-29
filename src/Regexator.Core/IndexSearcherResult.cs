// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Regexator
{
    public class IndexSearcherResult<T>
    {
        internal IndexSearcherResult(T current, T previous, T next)
        {
            Current = current;
            Previous = previous;
            Next = next;
        }

        public T Current { get; }

        public T Previous { get; }

        public T Next { get; }
    }
}
