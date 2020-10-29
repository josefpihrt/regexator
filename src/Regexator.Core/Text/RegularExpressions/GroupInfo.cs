// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Regexator.Text.RegularExpressions
{
    [DebuggerDisplay("Index: {Index}, Name: {Name}")]
    public class GroupInfo
    {
        internal GroupInfo(int index, string name)
        {
            Index = index;
            Name = name;
        }

        protected GroupInfo(GroupInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Index = info.Index;
            Name = info.Name;
        }

        public int Index { get; }

        public string Name { get; }

        public static GroupInfo Default { get; } = new GroupInfo(0, "0");
    }
}
