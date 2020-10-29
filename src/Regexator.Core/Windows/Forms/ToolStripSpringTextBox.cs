// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public class ToolStripSpringTextBox : ToolStripTextBox
    {
        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (IsOnOverflow || Owner.Orientation == Orientation.Vertical)
                return DefaultSize;

            int width = Owner.DisplayRectangle.Width;

            if (Owner.OverflowButton.Visible)
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;

            int springBoxCount = 0;

            foreach (ToolStripItem item in Owner.Items)
            {
                if (item.IsOnOverflow)
                    continue;

                if (item is ToolStripSpringTextBox)
                {
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                {
                    width = width - item.Width - item.Margin.Horizontal;
                }
            }

            if (springBoxCount > 1)
                width /= springBoxCount;

            if (width < DefaultSize.Width)
                width = DefaultSize.Width;

            Size size = base.GetPreferredSize(constrainingSize);
            size.Width = width;

            return size;
        }
    }
}
