// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Regexator.Drawing
{
    public class RichTextBoxPrintDocument : PrintDocument
    {
        [DebuggerStepThrough]
        public RichTextBoxPrintDocument(RichTextBox rtb)
        {
            RichTextBox = rtb ?? throw new ArgumentNullException(nameof(rtb));
            PageNumbers = true;
            PageNumber = 1;
            Headers = new Collection<string>();
        }

        private int PrintHeader(PrintPageEventArgs e)
        {
            return PrintUtility.PrintHeader(
                Headers.ToArray(),
                (PageNumbers) ? PageNumber : -1,
                new Font(RichTextBox.Font, FontStyle.Regular),
                e);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            int height = PrintHeader(e);
            Rectangle rct = e.MarginBounds;
            rct.Offset(0, height);
            if (PrinterSettings.PrintRange == PrintRange.Selection)
            {
                _charFrom = RichTextBox.SelectionStart;
                _charTo = RichTextBox.SelectionStart + RichTextBox.SelectionLength;
            }
            else
            {
                _charTo = RichTextBox.TextLength;
            }

            _charFrom = PrintFormatted(RichTextBox, _charFrom, _charTo, e, rct);

            //string s = RichTextBox.Text.Substring(_charFrom, _charTo - _charFrom);
            //int charsOnPage = 0;
            //int linesPerPage = 0;
            //e.Graphics.MeasureString(s, RichTextBox.Font, rct.Size, StringFormat.GenericTypographic, out charsOnPage, out linesPerPage);
            //e.Graphics.DrawString(s, RichTextBox.Font, Brushes.Black, rct, StringFormat.GenericTypographic);
            //_charFrom += charsOnPage;

            e.HasMorePages = (_charFrom < _charTo);
            PageNumber++;
            base.OnPrintPage(e);
        }

        //private static int PrintFormatted(RichTextBox rtb, int charFrom, int charTo, PrintPageEventArgs e)
        //{
        //    return RichTextBoxPrintDocument.PrintFormatted(rtb, charFrom, charTo, e, e.MarginBounds);
        //}

        // Render the contents of the RichTextBox for printing
        // Return the last character printed + 1 (printing start from this point for next page)
        private static int PrintFormatted(
            RichTextBox rtb,
            int charFrom,
            int charTo,
            PrintPageEventArgs e,
            Rectangle marginBounds)
        {
            if (rtb == null)
                throw new ArgumentNullException(nameof(rtb));

            if (e == null)
                throw new ArgumentNullException(nameof(e));

            //Calculate the area to render and print
            RECT rectToPrint;
            rectToPrint.Top = (int)(marginBounds.Top * anInch);
            rectToPrint.Bottom = (int)(marginBounds.Bottom * anInch);
            rectToPrint.Left = (int)(marginBounds.Left * anInch);
            rectToPrint.Right = (int)(marginBounds.Right * anInch);

            //Calculate the size of the page
            RECT rectPage;
            rectPage.Top = (int)(e.PageBounds.Top * anInch);
            rectPage.Bottom = (int)(e.PageBounds.Bottom * anInch);
            rectPage.Left = (int)(e.PageBounds.Left * anInch);
            rectPage.Right = (int)(e.PageBounds.Right * anInch);

            IntPtr hdc = e.Graphics.GetHdc();

            FORMATRANGE fmtRange;
            fmtRange.chrg.cpMax = charTo;           //Indicate character from to character to
            fmtRange.chrg.cpMin = charFrom;
            fmtRange.hdc = hdc;                    //Use the same DC for measuring and rendering
            fmtRange.hdcTarget = hdc;              //Point at printer hDC
            fmtRange.rc = rectToPrint;             //Indicate the area on page to print
            fmtRange.rcPage = rectPage;            //Indicate size of page

            IntPtr res = IntPtr.Zero;
            IntPtr wparam = IntPtr.Zero;
            wparam = new IntPtr(1);

            //Get the pointer to the FORMATRANGE structure in memory
            IntPtr lparam = IntPtr.Zero;
            lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, lparam, false);

            //Send the rendered data for printing
            res = NativeMethods.SendMessage(rtb.Handle, NativeMethods.EM_FORMATRANGE, wparam, lparam);

            //Free the block of memory allocated
            Marshal.FreeCoTaskMem(lparam);

            //Release the device context handle obtained by a previous call
            e.Graphics.ReleaseHdc(hdc);

            //Return last + 1 character printer
            return res.ToInt32();
        }

        //Convert the unit used by the .NET framework (1/100 inch)
        //and the unit used by Win32 API calls (twips 1/1440 inch)
        private const double anInch = 14.4;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE
        {
            public int cpMin;           //First character of range (0 for start of doc)
            public int cpMax;           //Last character of range (-1 for end of doc)
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE
        {
            public IntPtr hdc;             //Actual DC to draw on
            public IntPtr hdcTarget;       //Target DC for determining text formatting
            public RECT rc;                //Region of the DC to draw to (in twips)
            public RECT rcPage;            //Region of the whole DC (page size) (in twips)
            public CHARRANGE chrg;         //Range of text to draw (see earlier declaration)
        }

        public Collection<string> Headers { get; }
        public RichTextBox RichTextBox { get; }
        public bool PageNumbers { get; set; }
        public int PageNumber { get; private set; }

        private int _charFrom;
        private int _charTo;
    }
}
