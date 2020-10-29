// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regexator.Text.RegularExpressions
{
    public class SplitItemCollection : ReadOnlyCollection<SplitItem>
    {
        private ReadOnlyCollection<GroupInfo> _unsuccessGroups;
        private ReadOnlyCollection<GroupInfo> _successGroups;

        internal SplitItemCollection(IList<SplitItem> list, GroupInfoCollection groupInfos)
            : base(list)
        {
            GroupInfos = groupInfos;
        }

        public ReadOnlyCollection<GroupInfo> SuccessGroups
        {
            get
            {
                return _successGroups
                    ?? (_successGroups = this
                        .Where(f => f.Kind == SplitItemKind.Group)
                        .Select(f => f.GroupInfo)
                        .Distinct(new GroupInfoIndexEqualityComparer())
                        .ToList()
                        .AsReadOnly());
            }
        }

        public ReadOnlyCollection<GroupInfo> UnsuccessGroups
        {
            get
            {
                return _unsuccessGroups
                    ?? (_unsuccessGroups = GroupInfos
                        .Except(SuccessGroups, new GroupInfoIndexEqualityComparer())
                        .ToList()
                        .AsReadOnly());
            }
        }

        public GroupInfoCollection GroupInfos { get; }
    }
}
