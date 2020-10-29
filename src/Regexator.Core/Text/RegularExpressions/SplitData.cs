// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public class SplitData
    {
        private SplitItemCollection _items;

        public SplitData(Regex regex, string input)
            : this(regex, input, MatchData.InfiniteLimit)
        {
        }

        public SplitData(Regex regex, string input, int limit)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Limit = limit;
            GroupInfos = new GroupInfoCollection(regex);
        }

        private IEnumerable<Match> EnumerateMatches(Match match)
        {
            int count = Limit;
            count--;

            if (Regex.RightToLeft)
            {
                var matches = new List<Match>();
                while (match.Success)
                {
                    matches.Add(match);

                    count--;
                    if (count == 0)
                    {
                        LimitState = LimitState.Limited;
                        break;
                    }

                    match = match.NextMatch();
                }

                for (int i = (matches.Count - 1); i >= 0; i--)
                    yield return matches[i];
            }
            else
            {
                while (match.Success)
                {
                    yield return match;

                    count--;
                    if (count == 0)
                    {
                        LimitState = LimitState.Limited;
                        yield break;
                    }

                    match = match.NextMatch();
                }
            }
        }

        private IEnumerable<SplitItem> EnumerateItems()
        {
            if (Limit == 1)
            {
                yield return new MatchSplitItem(Input);
                yield break;
            }

            Match firstMatch = Regex.Match(Input);
            if (!firstMatch.Success)
            {
                yield return new MatchSplitItem(Input);
                yield break;
            }

            int prevIndex = 0;
            int itemIndex = 0;
            int splitIndex = 0;

            foreach (Match match in EnumerateMatches(firstMatch))
            {
                yield return new MatchSplitItem(
                    Input.Substring(prevIndex, match.Index - prevIndex),
                    prevIndex,
                    itemIndex,
                    splitIndex);
                itemIndex++;
                splitIndex++;
                prevIndex = match.Index + match.Length;

                if (Regex.RightToLeft)
                {
                    for (int i = (GroupInfos.Count - 1); i >= 0; i--)
                    {
                        if (GroupInfos[i].Index != 0)
                        {
                            Group group = match.Groups[GroupInfos[i].Index];
                            if (group.Success)
                            {
                                yield return new GroupSplitItem(group, GroupInfos[i], itemIndex);
                                GroupSplitItemCount++;
                                itemIndex++;
                            }
                        }
                    }
                }
                else
                {
                    foreach (GroupInfo info in GroupInfos)
                    {
                        if (info.Index != 0)
                        {
                            Group group = match.Groups[info.Index];
                            if (group.Success)
                            {
                                yield return new GroupSplitItem(group, info, itemIndex);
                                GroupSplitItemCount++;
                                itemIndex++;
                            }
                        }
                    }
                }
            }

            yield return new MatchSplitItem(
                Input.Substring(prevIndex, Input.Length - prevIndex),
                prevIndex,
                itemIndex,
                splitIndex);
        }

        public SplitItemCollection Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new SplitItemCollection(EnumerateItems().ToArray(), GroupInfos);
#if DEBUG
                    string[] splits = (Limit == MatchData.InfiniteLimit)
                        ? Regex.Split(Input)
                        : Regex.Split(Input, Limit);

                    System.Diagnostics.Debug
                        .Assert(splits.Length == _items.Count, _items.Count.ToString() + " " + splits.Length.ToString());

                    for (int i = 0; i < _items.Count; i++)
                    {
                        if (_items[i].Value != splits[i])
                        {
                            System.Diagnostics.Debug.WriteLine(i);
                            System.Diagnostics.Debug.WriteLine(splits[i]);
                            System.Diagnostics.Debug.WriteLine(_items[i].Value);
                            System.Diagnostics.Debug.Fail("");
                        }
                    }
#endif
                }

                return _items;
            }
        }

        public ReadOnlyCollection<GroupInfo> SuccessGroups => Items.SuccessGroups;

        public ReadOnlyCollection<GroupInfo> UnsuccessGroups => Items.UnsuccessGroups;

        public Regex Regex { get; }

        public GroupInfoCollection GroupInfos { get; }

        public string Input { get; }

        public int Limit { get; }

        public int GroupSplitItemCount { get; private set; }

        public LimitState LimitState { get; private set; }
    }
}
