// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Regexator.Text.RegularExpressions
{
    public class ReplaceData
    {
        private string _output;
        private ReplaceItemCollection _items;
        private List<ReplaceItem> _lst;
        private int _offset;
        private int _count;

        public ReplaceData(Regex regex, string input)
            : this(regex, input, "")
        {
        }

        public ReplaceData(Regex regex, string input, string replacement)
            : this(regex, input, replacement, ReplacementSettings.Default)
        {
        }

        public ReplaceData(Regex regex, string input, string replacement, ReplacementSettings settings)
        {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Replacement = replacement ?? throw new ArgumentNullException(nameof(replacement));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        private string Replace()
        {
            _lst = new List<ReplaceItem>();

            if (Settings.Limit == MatchData.InfiniteLimit)
            {
                return Regex.Replace(Input, f => Evaluator(f));
            }
            else
            {
                return Regex.Replace(Input, f => Evaluator(f), Settings.Limit);
            }
        }

        private string Evaluator(Match match)
        {
            var item = new ReplaceItem(match, GetResult(match), match.Index + _offset, _count);
            _lst.Add(item);
            _offset += item.Result.Length - match.Length;
            _count++;

            if (Settings.Limit != MatchData.InfiniteLimit && _count == Settings.Limit && match.NextMatch().Success)
            {
                LimitState = LimitState.Limited;
            }
            else
            {
                LimitState = LimitState.NotLimited;
            }

            return item.Result.Value;
        }

        private string GetResult(Match match)
        {
            switch (Settings.Mode)
            {
                case ReplacementMode.ToUpper:
                    return match.Result(Replacement).ToUpper(CultureInfo.CurrentCulture);
                case ReplacementMode.ToLower:
                    return match.Result(Replacement).ToLower(CultureInfo.CurrentCulture);
                case ReplacementMode.Char:
                    {
                        switch (Replacement.Length)
                        {
                            case 0:
                                return "";
                            case 1:
                                return new string(Replacement[0], match.Length);
                            default:
                                return Multiply(Replacement, match.Length);
                        }
                    }
                default:
                    return match.Result(Replacement);
            }
        }

        private static string Multiply(string value, int multiplier)
        {
            var sb = new StringBuilder(multiplier * value.Length);
            for (int i = 0; i < multiplier; i++)
                sb.Append(value);

            return sb.ToString();
        }

        public Regex Regex { get; }

        public string Input { get; }

        public string Output
        {
            get
            {
                if (_lst == null)
                    _output = Replace();

                return _output;
            }
        }

        public string Replacement { get; }

        public ReplaceItemCollection Items
        {
            get
            {
                if (_items == null)
                {
                    if (_lst == null)
                        _output = Replace();

                    _items = new ReplaceItemCollection(_lst);
                }

                return _items;
            }
        }

        public LimitState LimitState { get; private set; }

        public ReplacementSettings Settings { get; }
    }
}
