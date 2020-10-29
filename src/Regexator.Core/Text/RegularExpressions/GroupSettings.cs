// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Regexator.Text.RegularExpressions
{
    [Serializable]
    public class GroupSettings : ICloneable
    {
        private readonly HashSet<string> _ignoredSet;

        public GroupSettings()
            : this(DefaultSortProperty, DefaultSortDirection)
        {
        }

        public GroupSettings(IList<string> ignoredGroups)
            : this(DefaultSortProperty, DefaultSortDirection, ignoredGroups)
        {
        }

        public GroupSettings(GroupSortProperty sortPropertyName, ListSortDirection sortDirection)
            : this(sortPropertyName, sortDirection, new string[] { })
        {
        }

        public GroupSettings(
            GroupSortProperty sortPropertyName,
            ListSortDirection sortDirection,
            IList<string> ignoredGroups)
        {
            if (ignoredGroups == null)
                throw new ArgumentNullException(nameof(ignoredGroups));

            SortProperty = sortPropertyName;
            SortDirection = sortDirection;
            IgnoredGroups = new ReadOnlyCollection<string>(ignoredGroups);
            _ignoredSet = new HashSet<string>(ignoredGroups);
            IsZeroIgnored = IgnoredGroups.Contains("0");
            Sorter = new GroupInfoSorter(SortProperty, SortDirection);
        }

        public static bool HasDefaultValues(GroupSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return settings.IgnoredGroups.Count == 0
                && settings.SortProperty == DefaultSortProperty
                && settings.SortDirection == DefaultSortDirection;
        }

        public bool IsIgnored(GroupInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return IsIgnored(info.Name);
        }

        public bool IsIgnored(string groupName) => _ignoredSet.Contains(groupName);

        public object Clone() => new GroupSettings(SortProperty, SortDirection, IgnoredGroups.ToArray());

        public bool IsZeroIgnored { get; }

        public GroupInfoSorter Sorter { get; }

        public GroupSortProperty SortProperty { get; }

        public ListSortDirection SortDirection { get; }

        public ReadOnlyCollection<string> IgnoredGroups { get; }

        public static ListSortDirection DefaultSortDirection => ListSortDirection.Ascending;

        public static GroupSortProperty DefaultSortProperty => GroupSortProperty.Index;

        public static GroupSettings Default { get; } = new GroupSettings();
    }
}
