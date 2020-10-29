// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public sealed class MatchBuilder : RegexBuilder
    {
        private readonly CaptureBlock[] _blocks;

        public MatchBuilder(MatchData data)
            : this(data, new OutputSettings())
        {
        }

        public MatchBuilder(MatchData data, OutputSettings settings)
            : this(data, settings, GroupSettings.Default)
        {
        }

        public MatchBuilder(MatchData data, GroupSettings groupSettings)
            : this(data, new OutputSettings(), groupSettings)
        {
        }

        public MatchBuilder(MatchData data, OutputSettings settings, GroupSettings groupSettings)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            GroupSettings = groupSettings ?? throw new ArgumentNullException(nameof(groupSettings));
            IgnoredGroups = data.GroupInfos.Where(f => groupSettings.IsIgnored(f.Name)).ToList().AsReadOnly();

            var builder = new MatchTextBuilder(MatchItems, Settings, GroupSettings);
            _blocks = new CaptureBlockIterator(builder).ToArray();
            InfoLength = builder.InfoLength;

            Blocks = new ReadOnlyCollection<RegexBlock>(_blocks);
            CaptureBlocks = Array.AsReadOnly(_blocks);
        }

        public override string GetText()
        {
            return string.Join(Settings.NewLine, _blocks.Select(f => f.Text));
        }

        public override DataTable GetTable()
        {
            return MatchTableBuilder.CreateTable(_blocks, Settings.AddInfo);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public DataTable GetGroupTable()
        {
            var builder = new MatchTableBuilder(MatchItems, Settings, GroupSettings);
            return builder.CreateGroupTable();
        }

        public override OutputSettings Settings { get; }

        public override int Count
        {
            get { return Data.MatchCount; }
        }

        public override LimitState LimitState
        {
            get { return Data.LimitState; }
        }

        public override ReadOnlyCollection<RegexBlock> Blocks { get; }

        public ReadOnlyCollection<CaptureBlock> CaptureBlocks { get; }

        public MatchItemCollection MatchItems
        {
            get { return Data.Items; }
        }

        public override int CaptureCount
        {
            get { return Data.CaptureCount; }
        }

        public MatchData Data { get; }

        public override EvaluationMode Mode
        {
            get { return EvaluationMode.Match; }
        }

        public override string Input
        {
            get { return Data.Input; }
        }

        public GroupSettings GroupSettings { get; }

        public ReadOnlyCollection<GroupInfo> IgnoredGroups { get; }

        public override int InfoLength { get; }
    }
}
