// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public sealed class ReplaceBuilder : RegexBuilder
    {
        public ReplaceBuilder(ReplaceData data)
            : this(data, new OutputSettings())
        {
        }

        public ReplaceBuilder(ReplaceData data, OutputSettings settings)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            using (var processor = new ReplaceOutputProcessor())
            {
                _blocks = processor.CreateBlocks(this);
                Blocks = new ReadOnlyCollection<RegexBlock>(_blocks);
                _text = processor.ToString();
                SymbolSpans = processor.SymbolSpans;
            }
        }

        public override string GetText()
        {
            return _text;
        }

        public override DataTable GetTable()
        {
            var tbl = new DataTable() { Locale = CultureInfo.CurrentUICulture };
            tbl.Columns.Add(Symbols.DefaultNumber, typeof(int));
            bool info = Settings.AddInfo;
            if (info)
            {
                tbl.Columns.Add(Captions.DefaultMatchChar.ToString() + Captions.DefaultIndexChar.ToString(), typeof(int));
                tbl.Columns.Add(Captions.DefaultMatchChar.ToString() + Captions.DefaultLengthChar.ToString(), typeof(int));
            }

            tbl.Columns.Add(Captions.DefaultMatch, typeof(string));
            if (info)
            {
                tbl.Columns.Add(Captions.DefaultResultChar.ToString() + Captions.DefaultIndexChar.ToString(), typeof(int));
                tbl.Columns.Add(Captions.DefaultResultChar.ToString() + Captions.DefaultLengthChar.ToString(), typeof(int));
            }

            tbl.Columns.Add(Captions.DefaultResult, typeof(string));
            foreach (ReplaceBlock block in _blocks)
            {
                if (info)
                {
                    tbl.Rows.Add(
                        block.ReplaceItem.ItemIndex,
                        block.Match.Index,
                        block.Match.Length,
                        new FormattedValue(block.Match.Value, Settings),
                        block.Result.Index,
                        block.Result.Length,
                        new FormattedValue(block.Result.Value, Settings)
                    );
                }
                else
                {
                    tbl.Rows.Add(
                        block.ReplaceItem.ItemIndex,
                        new FormattedValue(block.Match.Value, Settings),
                        new FormattedValue(block.Result.Value, Settings)
                    );
                }
            }

            return tbl;
        }

        public override OutputSettings Settings { get; }

        public override int Count
        {
            get { return ReplaceItems.Count; }
        }

        public override LimitState LimitState
        {
            get { return Data.LimitState; }
        }

        public ReadOnlyCollection<TextSpan> SymbolSpans { get; }

        public override ReadOnlyCollection<RegexBlock> Blocks { get; }

        public ReplaceData Data { get; }

        public ReplaceItemCollection ReplaceItems
        {
            get { return Data.Items; }
        }

        public override EvaluationMode Mode
        {
            get { return EvaluationMode.Replace; }
        }

        public override string Input
        {
            get { return Data.Input; }
        }

        private readonly string _text;
        private readonly ReplaceBlock[] _blocks;
    }
}
