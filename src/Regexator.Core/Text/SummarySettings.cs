// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Regexator.Text
{
    public class SummarySettings
    {
        private string _emptyValueCaption;
        private string _headingSeparator;

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public SummarySettings()
        {
            Headings = CreateDefaultHeadings();
            RegexOptionsSeparator = ", ";
            RegexOptionsSpace = false;
            NewLine = Environment.NewLine;
            HeadingSeparator = ":";
            GroupSeparator = ", ";
            ReplacementInReplaceModeOnly = true;
            NewLineOnValueElements = SummaryElements.Input
                | SummaryElements.Output
                | SummaryElements.Pattern
                | SummaryElements.Replacement;
            CheckEmptyValueElements = SummaryElements.Input
                | SummaryElements.Output
                | SummaryElements.Pattern
                | SummaryElements.Replacement;
            OmitIfEmptyElements = SummaryElements.Author | SummaryElements.Description | SummaryElements.Title;
            _emptyValueCaption = Resources.Empty.ToLowerInvariant();
        }

        public static Dictionary<SummaryElements, string> CreateDefaultHeadings()
        {
            return new Dictionary<SummaryElements, string>()
            {
                [SummaryElements.Author] = "Author",
                [SummaryElements.Description] = "Description",
                [SummaryElements.Title] = "Title",
                [SummaryElements.Mode] = "Mode",
                [SummaryElements.RegexOptions] = "Options",
                [SummaryElements.Pattern] = "Pattern",
                [SummaryElements.Replacement] = "Replacement",
                [SummaryElements.ReplacementMode] = "Result Mode",
                [SummaryElements.Input] = "Input",
                [SummaryElements.Output] = "Output",
                [SummaryElements.Groups] = "Groups"
            };
        }

        public bool NewLineOnValue(SummaryElements elements)
        {
            return (NewLineOnValueElements & elements) == elements;
        }

        public bool CheckEmptyValue(SummaryElements elements)
        {
            return (CheckEmptyValueElements & elements) == elements;
        }

        public bool OmitIfEmpty(SummaryElements elements)
        {
            return (OmitIfEmptyElements & elements) == elements;
        }

        public Dictionary<SummaryElements, string> Headings { get; }
        public string NewLine { get; set; }
        public string LineSeparator { get; set; }
        public string RegexOptionsSeparator { get; set; }
        public string GroupSeparator { get; set; }
        public bool RegexOptionsSpace { get; set; }
        public bool ReplacementInReplaceModeOnly { get; set; }
        public bool NewLineOnRegexOptions { get; set; }
        public SummaryElements NewLineOnValueElements { get; set; }
        public SummaryElements CheckEmptyValueElements { get; set; }
        public SummaryElements OmitIfEmptyElements { get; set; }

        public string HeadingSeparator
        {
            get { return _headingSeparator; }
            set { _headingSeparator = value ?? ""; }
        }

        public string EmptyValueCaption
        {
            get { return _emptyValueCaption; }
            set { _emptyValueCaption = value ?? ""; }
        }
    }
}