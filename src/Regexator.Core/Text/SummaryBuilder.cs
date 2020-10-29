// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Regexator.Text
{
    public class SummaryBuilder : SummaryBuilderBase
    {
        private StringBuilder _sb;

        public SummaryBuilder()
        {
            _sb = new StringBuilder();
        }

        public void CreateSummary(SummaryInfo info)
        {
            _sb = new StringBuilder();
            BuildSummary(info);
        }

        public override void Append(string text)
        {
            if (!string.IsNullOrEmpty(text))
                _sb.Append(text);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        public override int Length
        {
            get { return _sb.Length; }
        }
    }
}
