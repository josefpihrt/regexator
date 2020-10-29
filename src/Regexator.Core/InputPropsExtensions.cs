// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regexator
{
    [DebuggerStepThrough]
    public static class InputPropsExtensions
    {
        public static InputProps Union(this InputProps props, InputProps value)
        {
            return props | value;
        }

        public static InputProps Except(this InputProps props, InputProps value)
        {
            return props & ~value;
        }

        public static InputProps Intersect(this InputProps props, InputProps value)
        {
            return props & value;
        }

        public static bool Contains(this InputProps props, InputProps value)
        {
            return props.Intersect(value) == value;
        }

        public static bool ContainsAny(this InputProps props, InputProps value)
        {
            return props.Intersect(value) != InputProps.None;
        }

        public static bool Any(this InputProps props)
        {
            return props != InputProps.None;
        }

        public static IEnumerable<InputProps> ToValues(this InputProps props)
        {
            return Values.Where(f => ((props & f) == f));
        }

        private static InputProps[] Values
        {
            get
            {
                return _values
                    ?? (_values = Enum.GetValues(typeof(InputProps))
                        .Cast<InputProps>()
                        .Where(f => f != InputProps.None)
                        .ToArray());
            }
        }

        private static InputProps[] _values;
    }
}
