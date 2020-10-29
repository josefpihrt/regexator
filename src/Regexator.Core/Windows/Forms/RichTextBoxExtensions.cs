// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace Regexator.Windows.Forms
{
    public static class RichTextBoxExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void ResetZoom(this RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            NativeMethods.SendMessage(rtb.Handle, NativeMethods.EM_SETZOOM, (IntPtr)1, (IntPtr)1);
        }

        public static Point GetSelectionStartLocation(this RichTextBox rtb)
        {
            return GetSelectionStartLocation(rtb, 0);
        }

        public static Point GetSelectionStartLocation(this RichTextBox rtb, int verticalOffset)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            return rtb.GetIndexLocation(rtb.SelectionStart, verticalOffset);
        }

        public static Point GetSelectionEndLocation(this RichTextBox rtb)
        {
            return GetSelectionEndLocation(rtb, 0);
        }

        public static Point GetSelectionEndLocation(this RichTextBox rtb, int verticalOffset)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            return rtb.GetIndexLocation(rtb.SelectionStart + rtb.SelectionLength, verticalOffset);
        }

        public static Point GetIndexLocation(this RichTextBox rtb, int index, int verticalOffset)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            if (rtb.GetLineStartIndex(index) == index && rtb.SelectionLength > 0)
                index--;

            Point pt = rtb.GetPositionFromCharIndex(index);
            pt.Offset(0, (int)(rtb.Font.Height * rtb.ZoomFactor) + verticalOffset);
            return pt;
        }

        public static bool IsCaretVisible(this RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            int index = rtb.SelectionStart + rtb.SelectionLength;

            if (rtb.SelectionLength > 0 && rtb.SelectionEndsAtBeginningOfLine())
                index--;

            return rtb.IsCharVisible(index);
        }

        public static bool IsCharVisible(this RichTextBox rtb, int index)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            Rectangle rct = rtb.RectangleToScreen(rtb.ClientRectangle);
            Point pt = rtb.PointToScreen(rtb.GetPositionFromCharIndex(index));
            if (rct.Contains(pt))
            {
                pt.Offset(0, (int)(rtb.Font.Height * rtb.ZoomFactor));
                return rct.Contains(pt);
            }

            return false;
        }

        public static void EnsureCaretVisible(this RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            if (!rtb.IsCaretVisible())
                rtb.ScrollToCaret();
        }

        public static void CopyAll(this RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            using (new ScrollBlocker(rtb, true))
            {
                rtb.SelectAll();
                rtb.Copy();
            }
        }

        public static void Highlight(this RichTextBox rtb, int index, int length, FontStyle style)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            if (length > 0)
            {
                rtb.Select(index, length);
                rtb.SelectionFont = new Font(
                    rtb.Font,
                    (rtb.SelectionFont != null) ? rtb.SelectionFont.Style : (FontStyle.Regular | style));
            }
        }

        public static void ClearFormat(this RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            rtb.SelectAll();
            rtb.SelectionBackColor = rtb.BackColor;
            rtb.SelectionColor = rtb.ForeColor;
            rtb.SelectionFont = rtb.Font;
        }

        public static void Highlight(this RichTextBox rtb, int index, int length, Color backColor)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            rtb.Select(index, length);
            rtb.SelectionBackColor = backColor;
        }

        public static void Highlight(this RichTextBox rtb, int index, int length, Color backColor, Color foreColor)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            rtb.Select(index, length);
            rtb.SelectionBackColor = backColor;
            rtb.SelectionColor = foreColor;
        }
    }
}
