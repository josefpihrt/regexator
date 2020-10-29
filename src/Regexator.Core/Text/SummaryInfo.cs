// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regexator.Output;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public class SummaryInfo
    {
        private string _input;
        private string _replacement;
        private string _output;

        public SummaryInfo(RegexInfo regexInfo, GroupSettings groupSettings)
        {
            RegexInfo = regexInfo ?? throw new ArgumentNullException(nameof(regexInfo));
            GroupSettings = groupSettings ?? throw new ArgumentNullException(nameof(groupSettings));
            _input = "";
            _replacement = "";
            _output = "";
            UnsuccessGroups = new HashSet<string>();
        }

        public RegexInfo RegexInfo { get; }

        public Regex Regex
        {
            get { return RegexInfo.Regex; }
        }

        public GroupInfoCollection Groups
        {
            get { return RegexInfo.Groups; }
        }

        public GroupSettings GroupSettings { get; }

        public string Input
        {
            get { return _input; }
            set { _input = value ?? ""; }
        }

        public string Replacement
        {
            get { return _replacement; }
            set { _replacement = value ?? ""; }
        }

        public string Output
        {
            get { return _output; }
            set { _output = value ?? ""; }
        }

        public ProjectInfo ProjectInfo { get; set; }
        public EvaluationMode Mode { get; set; }
        public ReplacementMode ReplacementMode { get; set; }
        public HashSet<string> UnsuccessGroups { get; }
    }
}