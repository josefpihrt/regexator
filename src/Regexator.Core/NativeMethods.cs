// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Regexator
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int nVirtKey);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        internal const int EM_FORMATRANGE = WM_USER + 57;
        internal const int EM_GETEVENTMASK = WM_USER + 59;
        internal const int EM_GETSCROLLPOS = WM_USER + 221;
        internal const int EM_GETZOOM = WM_USER + 224;
        internal const int EM_HIDESELECTION = WM_USER + 63;
        internal const int EM_SETEVENTMASK = WM_USER + 69;
        internal const int EM_SETSCROLLPOS = WM_USER + 222;
        internal const int EM_SETZOOM = WM_USER + 225;
        internal const int SB_HORZ = 0x0;
        internal const int SB_THUMBPOSITION = 4;
        internal const int SB_VERT = 0x1;
        internal const int WM_HSCROLL = 0x114;
        internal const int WM_SETREDRAW = 0x000B;
        internal const int WM_USER = 0x0400;
        internal const int WM_VSCROLL = 0x115;
    }
}
