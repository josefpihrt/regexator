// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regexator
{
    [DebuggerStepThrough]
    public static class ContainerPropsExtensions
    {
        public static ContainerProps Union(this ContainerProps props, ContainerProps value)
        {
            return props | value;
        }

        public static ContainerProps Except(this ContainerProps props, ContainerProps value)
        {
            return props & ~value;
        }

        public static ContainerProps Intersect(this ContainerProps props, ContainerProps value)
        {
            return props & value;
        }

        public static bool Contains(this ContainerProps props, ContainerProps value)
        {
            return props.Intersect(value) == value;
        }

        public static bool Any(this ContainerProps props)
        {
            return props != ContainerProps.None;
        }

        public static IEnumerable<ContainerProps> ToValues(this ContainerProps props)
        {
            return Values.Where(f => ((props & f) == f));
        }

        private static ContainerProps[] Values
        {
            get
            {
                return _values
                    ?? (_values = Enum.GetValues(typeof(ContainerProps))
                        .Cast<ContainerProps>()
                        .Where(f => f != ContainerProps.None)
                        .ToArray());
            }
        }

        private static ContainerProps[] _values;
    }
}
