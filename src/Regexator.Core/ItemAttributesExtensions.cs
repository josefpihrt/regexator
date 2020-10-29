// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Regexator
{
    [DebuggerStepThrough]
    public static class ItemAttributesExtensions
    {
        public static ItemAttributes Union(this ItemAttributes props, ItemAttributes value)
        {
            return props | value;
        }

        public static ItemAttributes Except(this ItemAttributes props, ItemAttributes value)
        {
            return props & ~value;
        }

        public static ItemAttributes Intersect(this ItemAttributes props, ItemAttributes value)
        {
            return props & value;
        }

        public static bool Contains(this ItemAttributes props, ItemAttributes value)
        {
            return props.Intersect(value) == value;
        }

        public static bool ContainsAny(this ItemAttributes props, ItemAttributes value)
        {
            return props.Intersect(value) != ItemAttributes.None;
        }

        public static bool Any(this ItemAttributes props)
        {
            return props != ItemAttributes.None;
        }
    }
}
