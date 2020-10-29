// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Regexator.Text;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public abstract class SummaryBuilderBase
    {
        protected SummaryBuilderBase()
        {
            Settings = new SummarySettings();
            _elements = new Dictionary<SummaryElements, SummaryElement>();
        }

        public abstract void Append(string text);

        public abstract int Length { get; }

        protected void BuildSummary(SummaryInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            _isFirst = true;
            _elements.Clear();
            Append(SummaryElements.Author, info.ProjectInfo.Author);
            Append(SummaryElements.Description, info.ProjectInfo.Description);
            Append(SummaryElements.Title, info.ProjectInfo.Title);
            Append(SummaryElements.Mode, info.Mode.ToString());
            Append(SummaryElements.RegexOptions, GetRegexOptions(info.Regex.Options));
            Append(SummaryElements.Pattern, info.Regex.ToString());
            AppendGroups(info);

            if (info.Mode == EvaluationMode.Replace || !Settings.ReplacementInReplaceModeOnly)
                Append(SummaryElements.Replacement, info.Replacement);

            if (info.Mode == EvaluationMode.Replace && info.ReplacementMode != ReplacementMode.None)
                Append(SummaryElements.ReplacementMode, Regexator.EnumHelper.GetDescription(info.ReplacementMode));

            Append(SummaryElements.Input, info.Input);
            Append(SummaryElements.Output, info.Output);
        }

        private void AppendGroups(SummaryInfo info)
        {
            switch (info.Mode)
            {
                case EvaluationMode.Match:
                    {
                        {
                            GroupInfo[] groups = info.Groups.Where(f => f.Index != 0 || info.GroupSettings.IsZeroIgnored)
                                .ToArray();
                            if (groups.Length > 0)
                            {
                                string excluded = Resources.Excluded.ToLower(CultureInfo.CurrentCulture)
                                    .AddBrackets(BracketKind.Square);
                                string noSuccess = Resources.NoSuccess.ToLower(CultureInfo.CurrentCulture)
                                    .AddBrackets(BracketKind.Square);
                                string s = string.Join(
                                    Settings.GroupSeparator,
                                    groups
                                        .OrderBy(f => f, info.GroupSettings.Sorter)
                                        .Select(f =>
                                        {
                                            if (info.GroupSettings.IsIgnored(f))
                                                return f.Name + " " + excluded;

                                            if (info.UnsuccessGroups.Contains(f.Name))
                                                return f.Name + " " + noSuccess;

                                            return f.Name;
                                        }));
                                Append(SummaryElements.Groups, s);
                            }
                        }

                        break;
                    }
                case EvaluationMode.Split:
                    {
                        {
                            GroupInfo[] groups = info.Groups.ExceptZero().OrderBy(f => f.Index).ToArray();
                            if (groups.Length > 0)
                            {
                                string noSuccess = Resources.NoSuccess.ToLower(CultureInfo.CurrentCulture)
                                    .AddBrackets(BracketKind.Square);
                                string s = string.Join(
                                    Settings.GroupSeparator,
                                    groups.Select(
                                        f => (info.UnsuccessGroups.Contains(f.Name)) ? f.Name + " " + noSuccess : f.Name));
                                Append(SummaryElements.Groups, s);
                            }
                        }

                        break;
                    }
                default:
                    {
                        {
                            string[] groups = info.Groups.ExceptZero().OrderBy(f => f.Index).ToNames().ToArray();

                            if (groups.Length > 0)
                                Append(SummaryElements.Groups, string.Join(Settings.GroupSeparator, groups));
                        }

                        break;
                    }
            }
        }

        private void Append(SummaryElements element, string value)
        {
            value = value ?? "";
            if (value.Length != 0 || !Settings.OmitIfEmpty(element))
            {
                if (!_isFirst)
                    Append(ItemSeparator);

                _isFirst = false;
                int index = Length;

                string heading = GetHeading(element);
                Append(heading);

                string separator = (Settings.NewLineOnValue(element)) ? Settings.NewLine : " ";

                bool valueIsEmptyCaption = (value.Length == 0 && Settings.CheckEmptyValue(element));
                string s = (valueIsEmptyCaption) ? Settings.EmptyValueCaption : value;

                if (element == SummaryElements.RegexOptions
                    && !Settings.NewLineOnValue(element)
                    && Settings.NewLineOnRegexOptions)
                {
                    s = TextUtility.AddIndent(s, heading.Length + separator.Length, false);
                }

                var headingSelection = new TextSpan(index, Length - index);
                Append(separator);
                var valueSelection = new TextSpan(Length, s.Length);
                Append(s);
                _elements.Add(
                    element,
                    new SummaryElement(element, heading, headingSelection, s, valueSelection, valueIsEmptyCaption));
            }
        }

        private string GetHeading(SummaryElements item)
        {
            if (!Settings.Headings.TryGetValue(item, out string heading))
                heading = _defaultHeadings[item];

            return heading + Settings.HeadingSeparator;
        }

        private string GetRegexOptions(RegexOptions value)
        {
            return (Settings.NewLineOnRegexOptions)
                ? string.Join(Settings.NewLine, value.ToNames())
                : RegexOptionsUtility.Format(value, Settings.RegexOptionsSeparator, Settings.RegexOptionsSpace);
        }

        public SummaryElement GetElement(SummaryElements key)
        {
            if (_elements.TryGetValue(key, out SummaryElement value))
                return value;

            return null;
        }

        private string ItemSeparator
        {
            get { return Settings.NewLine + Settings.LineSeparator ?? ""; }
        }

        public SummarySettings Settings
        {
            get { return _settings; }
            set { _settings = value ?? new SummarySettings(); }
        }

        public IEnumerable<int> EnumerateHeadingIndexes()
        {
            return EnumerateHeadingIndexes(false);
        }

        public IEnumerable<int> EnumerateHeadingIndexes(bool endIndex)
        {
            foreach (int item in EnumerateHeadingSpans().Select(f => f.Index))
                yield return item;

            if (endIndex)
                yield return Length;
        }

        public IEnumerable<TextSpan> EnumerateHeadingSpans()
        {
            return Elements.Select(f => f.HeadingSpan);
        }

        public IEnumerable<SummaryElement> EnumerateElements(SummaryElements elements)
        {
            foreach (KeyValuePair<SummaryElements, SummaryElement> pair in _elements)
            {
                if ((elements & pair.Key) == pair.Key)
                    yield return pair.Value;
            }
        }

        public IEnumerable<SummaryElement> Elements
        {
            get { return _elements.Select(f => f.Value); }
        }

        private readonly Dictionary<SummaryElements, SummaryElement> _elements;
        private SummarySettings _settings;
        private bool _isFirst = true;

        private static readonly Dictionary<SummaryElements, string> _defaultHeadings
            = new Dictionary<SummaryElements, string>(SummarySettings.CreateDefaultHeadings());
    }
}
