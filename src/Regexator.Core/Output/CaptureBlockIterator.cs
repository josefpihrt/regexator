// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal class CaptureBlockIterator : IEnumerable<CaptureBlock>
    {
        private readonly MatchTextBuilder _format;
        private int _startIndex;
        private bool _fMatch;
        private bool _fGroup;
        private CaptureBlock _matchBlock;
        private CaptureBlock _groupBlock;
        private CaptureInfoSelections _selections;
        private TextSpan _matchSpan;
        private TextSpan _groupSpan;

        public CaptureBlockIterator(MatchTextBuilder format)
        {
            _format = format;
        }

        public IEnumerator<CaptureBlock> GetEnumerator()
        {
            return _format.MatchItems.SelectMany(f => CreateBlocks(f)).GetEnumerator();
        }

        private IEnumerable<CaptureBlock> CreateBlocks(MatchItem matchItem)
        {
            _matchBlock = null;
            _matchSpan = null;

            if (_format.HasDefaultGroupSettings)
            {
                return matchItem.GroupItems.SelectMany(f => CreateBlocks(f));
            }
            else
            {
                return matchItem.EnumerateGroupItems(_format.GroupSettings).SelectMany(f => CreateBlocks(f));
            }
        }

        private IEnumerable<CaptureBlock> CreateBlocks(GroupItem groupItem)
        {
            _groupBlock = null;
            _groupSpan = null;
            return groupItem.CaptureItems.Select(f => CreateBlock(f));
        }

        private CaptureBlock CreateBlock(CaptureItem item)
        {
            CaptureItem = item;
            _selections = _format.GetInfoSelections(_startIndex);

            _fMatch = _format.Settings.OmitRepeatedInfo && _matchBlock != null;
            _fGroup = _format.Settings.OmitRepeatedInfo && _groupBlock != null;

            TextSpan matchSelection = (_fMatch) ? _matchSpan : _selections.Match;
            TextSpan groupSelection = (_fGroup) ? _groupSpan : _selections.Group;

            InfoSelections = new CaptureInfoSelections(
                (_fMatch) ? _matchSpan : matchSelection,
                (_fGroup) ? _groupSpan : groupSelection,
                _selections.Capture,
                _selections.Index,
                _selections.Length);

            var block = new CaptureBlock(this);
            if (_matchBlock == null)
            {
                _matchBlock = block;
                _matchSpan = matchSelection;
            }

            if (_groupBlock == null)
            {
                _groupBlock = block;
                _groupSpan = groupSelection;
            }

            CaptureId++;
            _startIndex = block.EndIndex + 1;
            return block;
        }

        public string GetInfoText()
        {
            return _format.GetInfo(CaptureItem, _fMatch, _fGroup);
        }

        public CaptureInfoSelections InfoSelections { get; private set; }

        public CaptureItem CaptureItem { get; private set; }

        public int CaptureId { get; private set; }

        public int StartIndex
        {
            get { return _startIndex; }
        }

        public OutputSettings Settings
        {
            get { return _format.Settings; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
