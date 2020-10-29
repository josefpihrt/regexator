// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;

namespace Regexator.Text
{
    public class RichTextBoxWrapper : ITextContainer
    {
        public RichTextBoxWrapper(RichTextBox richTextBox)
        {
            RichTextBox = richTextBox ?? throw new ArgumentNullException(nameof(richTextBox));
        }

        public void SelectText(int start, int length)
        {
            RichTextBox.Select(start, length);
        }

        public string Text
        {
            get { return RichTextBox.Text; }
            set { RichTextBox.Text = value; }
        }

        public string SelectedText
        {
            get { return RichTextBox.SelectedText; }
            set { RichTextBox.SelectedText = value; }
        }

        public int SelectionStart
        {
            get { return RichTextBox.SelectionStart; }
            set { RichTextBox.SelectionStart = value; }
        }

        public int SelectionLength
        {
            get { return RichTextBox.SelectionLength; }
            set { RichTextBox.SelectionLength = value; }
        }

        public RichTextBox RichTextBox { get; }
    }
}
