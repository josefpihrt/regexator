// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Text
{
    public class TextBoxSummaryBuilder : SummaryBuilderBase
    {
        public void CreateSummary(SummaryInfo info, TextBoxBase textBox)
        {
            TextBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            BuildSummary(info);
        }

        public override void Append(string text)
        {
            if (!string.IsNullOrEmpty(text))
                TextBox.AppendText(text);
        }

        public override string ToString()
        {
            return TextBox.Text;
        }

        public override int Length
        {
            get { return TextBox.TextLength; }
        }

        public TextBoxBase TextBox { get; private set; }
    }
}
