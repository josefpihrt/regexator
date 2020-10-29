// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using Regexator.Text.RegularExpressions;

namespace Regexator.Text
{
    public class RichTextBoxSearcher : TextSearcher
    {
        public RichTextBoxSearcher(RichTextBox rtb)
            : base(new RichTextBoxWrapper(rtb))
        {
            RichTextBox = rtb;
            RichTextBox.SelectionChanged += (object sender, EventArgs e) =>
            {
                if (ResetEnabled)
                    OnReset(EventArgs.Empty);
            };
        }

        protected override void ReplaceAll(ReplaceItemCollection items, int selectionEnd)
        {
            using (new ScrollBlocker(RichTextBox))
                base.ReplaceAll(items, selectionEnd);
        }

        public RichTextBox RichTextBox { get; }
    }
}
