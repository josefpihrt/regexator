// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;

namespace Regexator.Drawing
{
    public static class PrintUtility
    {
        public static void Print(RichTextBox rtb)
        {
            Print(rtb, new string[] { });
        }

        public static void Print(RichTextBox rtb, string header)
        {
            Print(rtb, new string[] { header });
        }

        public static void Print(RichTextBox rtb, string[] headers)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            var dlg = new PrintDialog()
            {
                UseEXDialog = true,
                AllowPrintToFile = false,
                AllowSelection = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (var doc = new RichTextBoxPrintDocument(rtb))
                {
                    doc.PrinterSettings = dlg.PrinterSettings;
                    const int m = (int)(1.5 / 2.54 * 100);
                    doc.DefaultPageSettings.Margins = new Margins(m, m, m, m);

                    foreach (string header in headers)
                        doc.Headers.Add(header);

                    doc.Print();
                }
            }
        }

        public static int PrintHeader(string[] values, int pageNumber, Font font, PrintPageEventArgs e)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (font == null)
                throw new ArgumentNullException(nameof(font));

            int offset = 0;
            foreach (string value in values)
            {
                Size size = PrintHeader(
                    value,
                    font,
                    offset,
                    e);
                offset += size.Height;
            }

            if (pageNumber != -1)
                PrintPageNumber(pageNumber, font, offset - font.Height, e);

            int lineHeight = PrintLine(offset, e);
            return offset + lineHeight + font.Height;
        }

        private static Size PrintHeader(string header, Font font, int verticalOffset, PrintPageEventArgs e)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            if (e == null)
                throw new ArgumentNullException(nameof(e));

            Rectangle rct = e.MarginBounds;
            rct.Offset(0, verticalOffset);
            header = (string.IsNullOrEmpty(header)) ? " " : header;
            var format = new StringFormat(StringFormat.GenericTypographic);
            format.FormatFlags |= StringFormatFlags.NoWrap;
            format.Trimming = StringTrimming.EllipsisPath;
            Size headerSize = e.Graphics.MeasureString(
                header,
                font,
                rct.Size,
                format,
                out _,
                out _)
                .ToSize();
            e.Graphics.DrawString(
                header,
                font,
                Brushes.Black,
                rct,
                format);
            return headerSize;
        }

        private static void PrintPageNumber(int pageNumber, Font font, int yOffset, PrintPageEventArgs e)
        {
            Rectangle rct = e.MarginBounds;
            rct = new Rectangle(rct.Right, rct.Top, e.PageBounds.Right - rct.Right, rct.Height);
            rct.Offset(0, yOffset);
            e.Graphics.DrawString(
                pageNumber.ToString(CultureInfo.CurrentCulture),
                font,
                Brushes.Black,
                rct,
                StringFormat.GenericTypographic);
        }

        private static int PrintLine(int verticalOffset, PrintPageEventArgs e)
        {
            e.Graphics.DrawLine(
                Pens.Black,
                new Point(e.MarginBounds.Left, e.MarginBounds.Top + verticalOffset + 1),
                new Point(e.MarginBounds.Right, e.MarginBounds.Top + verticalOffset + 1));
            return (int)Pens.Black.Width;
        }
    }
}
