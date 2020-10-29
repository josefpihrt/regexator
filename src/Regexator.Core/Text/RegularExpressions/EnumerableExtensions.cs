// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Match> ToMatches(this IEnumerable<MatchItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToMatches();

            IEnumerable<Match> ToMatches()
            {
                foreach (MatchItem item in items)
                    yield return item.Match;
            }
        }

        public static IEnumerable<Match> ToMatches(this IEnumerable<ReplaceItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToMatches();

            IEnumerable<Match> ToMatches()
            {
                foreach (ReplaceItem item in items)
                    yield return item.Match;
            }
        }

        public static IEnumerable<Group> ToGroups(this IEnumerable<MatchItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToGroups();

            IEnumerable<Group> ToGroups()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                        yield return item.GroupItems[i].Group;
                }
            }
        }

        public static IEnumerable<Group> ToGroups(this IEnumerable<GroupItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToGroups();

            IEnumerable<Group> ToGroups()
            {
                foreach (GroupItem item in items)
                    yield return item.Group;
            }
        }

        public static IEnumerable<GroupInfo> ToGroupInfos(this IEnumerable<GroupItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToGroupInfos();

            IEnumerable<GroupInfo> ToGroupInfos()
            {
                foreach (GroupItem item in items)
                    yield return item.GroupInfo;
            }
        }

        public static IEnumerable<Capture> ToCaptures(this IEnumerable<MatchItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToCaptures();

            IEnumerable<Capture> ToCaptures()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                    {
                        for (int j = 0; j < item.GroupItems[i].CaptureItems.Count; j++)
                            yield return item.GroupItems[i].CaptureItems[j].Capture;
                    }
                }
            }
        }

        public static IEnumerable<Capture> ToCaptures(this IEnumerable<GroupItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToCaptures();

            IEnumerable<Capture> ToCaptures()
            {
                foreach (GroupItem item in items)
                {
                    for (int i = 0; i < item.CaptureItems.Count; i++)
                        yield return item.CaptureItems[i].Capture;
                }
            }
        }

        public static IEnumerable<Capture> ToCaptures(this IEnumerable<CaptureItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToCaptures();

            IEnumerable<Capture> ToCaptures()
            {
                foreach (CaptureItem item in items)
                    yield return item.Capture;
            }
        }

        public static IEnumerable<GroupItem> ToGroupItems(this IEnumerable<MatchItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToGroupItems();

            IEnumerable<GroupItem> ToGroupItems()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                        yield return item.GroupItems[i];
                }
            }
        }

        public static IEnumerable<GroupItem> ToGroupItems(this IEnumerable<MatchItem> items, int groupIndex)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToGroupItems();

            IEnumerable<GroupItem> ToGroupItems()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                    {
                        if (item.GroupItems[i].GroupInfo.Index == groupIndex)
                        {
                            yield return item.GroupItems[i];
                            break;
                        }
                    }
                }
            }
        }

        public static IEnumerable<GroupItem> ToGroupItems(this IEnumerable<MatchItem> items, string groupName)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            return ToGroupItems();

            IEnumerable<GroupItem> ToGroupItems()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                    {
                        if (item.GroupItems[i].GroupInfo.Name == groupName)
                        {
                            yield return item.GroupItems[i];
                            break;
                        }
                    }
                }
            }
        }

        public static IEnumerable<GroupItem> ToSuccessGroupItems(this IEnumerable<MatchItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToSuccessGroupItems();

            IEnumerable<GroupItem> ToSuccessGroupItems()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                    {
                        if (item.GroupItems[i].Success)
                            yield return item.GroupItems[i];
                    }
                }
            }
        }

        internal static IEnumerable<GroupItem> ToGroupItems(
            this IEnumerable<MatchItem> items,
            Func<GroupItem, bool> predicate)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return ToGroupItems();

            IEnumerable<GroupItem> ToGroupItems()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                    {
                        if (predicate(item.GroupItems[i]))
                            yield return item.GroupItems[i];
                    }
                }
            }
        }

        public static IEnumerable<CaptureItem> ToCaptureItems(this IEnumerable<MatchItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToCaptureItems();

            IEnumerable<CaptureItem> ToCaptureItems()
            {
                foreach (MatchItem item in items)
                {
                    for (int i = 0; i < item.GroupItems.Count; i++)
                    {
                        GroupItem groupItem = item.GroupItems[i];

                        for (int j = 0; j < groupItem.CaptureItems.Count; j++)
                            yield return groupItem.CaptureItems[j];
                    }
                }
            }
        }

        public static IEnumerable<CaptureItem> ToCaptureItems(this IEnumerable<GroupItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToCaptureItems();

            IEnumerable<CaptureItem> ToCaptureItems()
            {
                foreach (GroupItem item in items)
                {
                    for (int i = 0; i < item.CaptureItems.Count; i++)
                        yield return item.CaptureItems[i];
                }
            }
        }

        public static IEnumerable<ReplaceResult> ToResults(this IEnumerable<ReplaceItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return ToResults();

            IEnumerable<ReplaceResult> ToResults()
            {
                foreach (ReplaceItem item in items)
                    yield return item.Result;
            }
        }

        public static IEnumerable<string> ToNames(this IEnumerable<GroupInfo> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return ToNames();

            IEnumerable<string> ToNames()
            {
                foreach (GroupInfo item in collection)
                    yield return item.Name;
            }
        }

        public static IEnumerable<int> ToIndexes(this IEnumerable<GroupInfo> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return ToIndexes();

            IEnumerable<int> ToIndexes()
            {
                foreach (GroupInfo item in collection)
                    yield return item.Index;
            }
        }

        public static IEnumerable<GroupInfo> ExceptZero(this IEnumerable<GroupInfo> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return ExceptZero();

            IEnumerable<GroupInfo> ExceptZero()
            {
                foreach (GroupInfo item in collection)
                {
                    if (item.Index != 0)
                        yield return item;
                }
            }
        }
    }
}
