// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Regexator.Text.RegularExpressions;
using Regexator.Collections.Generic;

namespace Regexator.Output
{
    public static class GroupSettingsHelper
    {
        public static ContainerProps GetChangedProps(GroupSettings first, GroupSettings second, ContainerProps props)
        {
            if (first == null || second == null || ReferenceEquals(first, second))
                return ContainerProps.None;

            var value = ContainerProps.None;

            if (props.Contains(ContainerProps.IgnoredGroups)
                && !first.IgnoredGroups.SequenceEqualUnordered(second.IgnoredGroups))
            {
                value |= ContainerProps.IgnoredGroups;
            }

            return value;
        }
    }
}
