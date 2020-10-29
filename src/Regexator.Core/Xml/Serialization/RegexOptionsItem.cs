// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace Regexator.Xml.Serialization
{
    public class RegexOptionsItem
    {
        public static Regexator.RegexOptionsItem FromSerializable(RegexOptionsItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new Regexator.RegexOptionsItem(item.ParseOptions(), item.Description, item.Visible);
        }

        public RegexOptions ParseOptions()
        {
            if (Enum.TryParse(Value, true, out RegexOptions value))
                return value;

            return RegexOptions.None;
        }

        public string Value { get; set; }
        public string Description { get; set; }
        public bool Visible { get; set; }
    }
}
