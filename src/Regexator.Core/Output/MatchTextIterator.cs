// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    internal class MatchTextIterator : IEnumerable<string>
    {
        private readonly MatchTextBuilder _builder;
        private bool _fMatch;
        private bool _fGroup;

        public MatchTextIterator(MatchTextBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerator<string> GetEnumerator()
        {
            _fMatch = false;
            _fGroup = false;
            return _builder.MatchItems.SelectMany(f => GetText(f)).GetEnumerator();
        }

        private IEnumerable<string> GetText(MatchItem matchItem)
        {
            _fMatch = false;

            if (_builder.HasDefaultGroupSettings)
                return matchItem.GroupItems.SelectMany(f => GetText(f));

            return matchItem.EnumerateGroupItems(_builder.GroupSettings).SelectMany(f => GetText(f));
        }

        private IEnumerable<string> GetText(GroupItem groupItem)
        {
            _fGroup = false;

            return groupItem.CaptureItems.Select(f => GetText(f));
        }

        private string GetText(CaptureItem item)
        {
            string _info = _builder.GetInfo(item, _fMatch, _fGroup);
            _fMatch = true;
            _fGroup = true;

            using (var processor = new TextProcessor(item.Value, _builder.Settings, _info.Length))
            {
                processor.ReadToEnd();
                return _info + processor.ToString(true);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
