// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;
using Regexator.Text.RegularExpressions;

namespace Regexator.Output
{
    public class RegexInfo
    {
        public RegexInfo(Regex regex)
        {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Groups = new GroupInfoCollection(regex);
        }

        public override string ToString()
        {
            return Regex.ToString();
        }

        public Regex Regex { get; }

        public string Pattern
        {
            get { return Regex.ToString(); }
        }

        public RegexOptions Options
        {
            get { return Regex.Options; }
        }

        public bool IsEmpty
        {
            get { return Pattern.Length == 0; }
        }

        public GroupInfoCollection Groups { get; }
    }
}
