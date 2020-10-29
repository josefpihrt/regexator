// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Regexator.Xml.Serialization.Projects.VersionLessThan1
{
    public class GroupSettings
    {
        public static GroupSettings ToSerializable(global::Regexator.Text.RegularExpressions.GroupSettings item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new GroupSettings()
            {
                IgnoreList = item.IgnoredGroups.OrderBy(f => f).ToArray()
            };
        }

        public static global::Regexator.Text.RegularExpressions.GroupSettings FromSerializable(GroupSettings item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return (item.IgnoreList != null)
                ? new global::Regexator.Text.RegularExpressions.GroupSettings(item.IgnoreList)
                : new global::Regexator.Text.RegularExpressions.GroupSettings();
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [XmlArrayItem("Name")]
        public string[] IgnoreList { get; set; }

        public string SortPropertyName { get; set; }
        public string SortDirection { get; set; }
    }
}
