// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Windows.Forms;
using Regexator.Text;

namespace Regexator.Windows.Forms
{
    public class TextBoxTextSpan : TextSpan
    {
        public TextBoxTextSpan(int index)
            : base(index, 0)
        {
        }

        public TextBoxTextSpan(int index, int length)
            : base(index, length)
        {
        }

        private TextBoxTextSpan(int index, int length, TextBoxBase box)
            : base(index, length)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            TextBox = box;
        }

        public TextBoxTextSpan(TextBoxBase box)
            : base(box.SelectionStart, box.SelectionLength)
        {
            TextBox = box ?? throw new ArgumentNullException(nameof(box));
        }

        public void Select()
        {
            TextBox?.Select(Index, Length);
        }

        public void Select(TextBoxBase textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            textBox.Select(Index, Length);
        }

        public void Highlight(RichTextBox rtb, Color backColor)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            rtb.Select(Index, Length);
            rtb.SelectionBackColor = backColor;
        }

        public override TextSpan Offset(int count)
        {
            return new TextBoxTextSpan(Index + count, Length, TextBox);
        }

        public override TextSpan Extend(int count)
        {
            return new TextBoxTextSpan(Index, Length + count, TextBox);
        }

        public override TextSpan Combine(TextSpan textSpan)
        {
            if (textSpan == null)
                throw new ArgumentNullException(nameof(textSpan));

            int index = Math.Min(Index, textSpan.Index);
            int endIndex = Math.Max(EndIndex, textSpan.EndIndex);
            return new TextBoxTextSpan(index, endIndex - index, TextBox);
        }

        public TextBoxBase TextBox { get; }
    }
}
