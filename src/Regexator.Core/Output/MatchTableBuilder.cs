// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal class MatchTableBuilder
    {
        private readonly MatchItemCollection _items;
        private readonly OutputSettings _settings;
        private readonly GroupSettings _groupSettings;
        private readonly ReadOnlyCollection<GroupInfo> _filteredGroups;
        private readonly bool _hasDefaultGroupSetting;

        public MatchTableBuilder(MatchItemCollection items)
            : this(items, new OutputSettings())
        {
        }

        public MatchTableBuilder(MatchItemCollection items, OutputSettings settings)
            : this(items, settings, new GroupSettings())
        {
        }

        public MatchTableBuilder(MatchItemCollection items, GroupSettings groupSettings)
            : this(items, new OutputSettings(), groupSettings)
        {
        }

        public MatchTableBuilder(MatchItemCollection items, OutputSettings settings, GroupSettings groupSettings)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _groupSettings = groupSettings ?? throw new ArgumentNullException(nameof(groupSettings));
            _filteredGroups = items.GroupInfos
                .Where(f => !groupSettings.IsIgnored(f.Name))
                .OrderBy(f => f, groupSettings.Sorter)
                .ToList()
                .AsReadOnly();
            _hasDefaultGroupSetting = GroupSettings.HasDefaultValues(groupSettings);
        }

        private static DataTable CreateTable(
            Func<IEnumerable<DataColumn>> columnFactory,
            Func<IEnumerable<object[]>> rowFactory)
        {
            var tbl = new DataTable() { Locale = CultureInfo.CurrentUICulture };
            tbl.Columns.AddRange(columnFactory().ToArray());

            foreach (object[] row in rowFactory())
                tbl.Rows.Add(row);

            return tbl;
        }

        public static DataTable CreateTable(IEnumerable<CaptureBlock> blocks, bool addInfo)
        {
            return CreateTable(() => CreateTableColumns(addInfo), () => CreateTableRows(blocks, addInfo));
        }

        private static IEnumerable<DataColumn> CreateTableColumns(bool info)
        {
            yield return new DataColumn(Symbols.DefaultNumber, typeof(int));
            if (info)
            {
                yield return new DataColumn(Captions.DefaultMatchChar.ToString(), typeof(int));
                yield return new DataColumn(Captions.DefaultGroupChar.ToString(), typeof(string));
                yield return new DataColumn(Captions.DefaultCaptureChar.ToString(), typeof(int));
                yield return new DataColumn(Captions.DefaultIndexChar.ToString(), typeof(int));
                yield return new DataColumn(Captions.DefaultLengthChar.ToString(), typeof(int));
            }

            yield return new DataColumn(Captions.DefaultValue, typeof(string));
        }

        private static IEnumerable<object[]> CreateTableRows(IEnumerable<CaptureBlock> blocks, bool info)
        {
            int i = 0;
            foreach (CaptureBlock block in blocks)
            {
                yield return CreateTableRow(block, i, info).ToArray();
                i++;
            }
        }

        private static IEnumerable<object> CreateTableRow(CaptureBlock block, int index, bool info)
        {
            yield return index;
            if (info)
            {
                yield return block.MatchItem.ItemIndex;
                yield return block.GroupName;
                yield return block.CaptureItem.ItemIndex;
                yield return block.CaptureItem.Index;
                yield return block.CaptureItem.Length;
            }

            yield return block.FormattedValue;
        }

        public DataTable CreateGroupTable()
        {
            return CreateTable(() => CreateGroupTableColumns(), () => CreateGroupTableRows());
        }

        private IEnumerable<DataColumn> CreateGroupTableColumns()
        {
            yield return new DataColumn(Symbols.DefaultNumber, typeof(int));

            foreach (DataColumn column in _filteredGroups.Select(f => new DataColumn(f.Name, typeof(string))))
                yield return column;

            if (_settings.AddInfo)
            {
                foreach (DataColumn column in _filteredGroups.ExceptZero()
                    .Select(f => new DataColumn(GetGroupColumnName(f))))
                {
                    yield return column;
                }
            }
        }

        private string GetGroupColumnName(GroupInfo info)
        {
            return info.Name +
                " " +
                _settings.Captions.Captures.ToLower(CultureInfo.CurrentCulture) +
                ((_items.ToGroupItems(info.Index).Any(f => f.CaptureCount != 1)) ? "*" : "");
        }

        private IEnumerable<object[]> CreateGroupTableRows()
        {
            return _items.Select(item => CreateGroupTableRow(item).ToArray());
        }

        private IEnumerable<object> CreateGroupTableRow(MatchItem item)
        {
            yield return item.ItemIndex;

            foreach (object value in GetGroupItems(item).Select(
                f => (f.Success) ? new FormattedValue(f.Value, _settings) : (object)DBNull.Value))
            {
                yield return value;
            }

            if (_settings.AddInfo)
            {
                foreach (int value in GetGroupItems(item).Where(f => !f.IsDefaultGroup).Select(g => g.CaptureCount))
                    yield return value;
            }
        }

        private IEnumerable<GroupItem> GetGroupItems(MatchItem item)
        {
            return (_hasDefaultGroupSetting) ? item.GroupItems : item.EnumerateGroupItems(_groupSettings);
        }
    }
}
