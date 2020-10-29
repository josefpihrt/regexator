// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public class GroupInfoCollection : ReadOnlyCollection<GroupInfo>
    {
        private readonly Dictionary<string, GroupInfo> _names;
        private readonly Dictionary<int, GroupInfo> _indexes;

        public GroupInfoCollection(Regex regex)
            : base(CreateGroupInfos(regex))
        {
            _names = ToNameDictionary();
            _indexes = ToIndexDictionary();
        }

        private static GroupInfo[] CreateGroupInfos(Regex regex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            string[] names = regex.GetGroupNames();
            var infos = new GroupInfo[names.Length];

            for (int i = 0; i < names.Length; i++)
                infos[i] = new GroupInfo(i, names[i]);

            return infos;
        }

        public bool Contains(string groupName)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            return _names.ContainsKey(groupName);
        }

        public bool Contains(int groupIndex)
        {
            if (groupIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(groupIndex));

            return _indexes.ContainsKey(groupIndex);
        }

        internal Dictionary<string, GroupInfo> ToNameDictionary() => Items.ToDictionary(f => f.Name, f => f);

        internal Dictionary<int, GroupInfo> ToIndexDictionary() => Items.ToDictionary(f => f.Index, f => f);

        public GroupInfo this[string groupName]
        {
            get
            {
                if (groupName == null)
                    throw new ArgumentNullException(nameof(groupName));

                try
                {
                    return _names[groupName];
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentOutOfRangeException(nameof(groupName));
                }
            }
        }
    }
}
