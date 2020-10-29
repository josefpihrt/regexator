// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public sealed class SplitBuilder : RegexBuilder
    {
        private readonly SplitBlock[] _blocks;

        public SplitBuilder(SplitData data)
            : this(data, new OutputSettings())
        {
        }

        public SplitBuilder(SplitData data, OutputSettings settings)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            var builder = new SplitTextBuilder(Data.Items, Settings);
            _blocks = builder.GetBlocks();
            Blocks = new ReadOnlyCollection<RegexBlock>(_blocks);
        }

        public override string GetText()
        {
            return string.Join("\n", _blocks.Select(f => f.Text));
        }

        public override DataTable GetTable()
        {
            var tbl = new DataTable() { Locale = CultureInfo.CurrentUICulture };
            tbl.Columns.Add(Symbols.DefaultNumber, typeof(int));
            bool info = Settings.AddInfo;
            if (info)
            {
                tbl.Columns.Add(Settings.Captions.SplitChar + "/" + Settings.Captions.GroupChar, typeof(string));
                tbl.Columns.Add("N", typeof(string));
                tbl.Columns.Add(Captions.DefaultIndexChar.ToString(), typeof(int));
                tbl.Columns.Add(Captions.DefaultLengthChar.ToString(), typeof(int));
            }

            tbl.Columns.Add(Captions.DefaultValue, typeof(string));
            foreach (SplitBlock block in _blocks)
            {
                if (info)
                {
                    tbl.Rows.Add(
                        block.MatchItemIndex,
                        (block.SplitItem.Kind == SplitItemKind.Group)
                            ? Settings.Captions.GroupChar
                            : Settings.Captions.SplitChar,
                        block.SplitItem.Name,
                        block.ValueIndex,
                        block.ValueLength,
                        block.FormattedValue);
                }
                else
                {
                    tbl.Rows.Add(block.MatchItemIndex, block.FormattedValue);
                }
            }

            return tbl;
        }

        public override OutputSettings Settings { get; }

        public override int Count
        {
            get { return Data.Items.Count; }
        }

        public override LimitState LimitState
        {
            get { return Data.LimitState; }
        }

        public override ReadOnlyCollection<RegexBlock> Blocks { get; }

        public SplitData Data { get; }

        public override EvaluationMode Mode
        {
            get { return EvaluationMode.Split; }
        }

        public override string Input
        {
            get { return Data.Input; }
        }
    }
}