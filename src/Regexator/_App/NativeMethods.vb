Imports System.Runtime.InteropServices

Friend Module NativeMethods

    <DllImport("uxtheme.dll", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Friend Function SetWindowTheme(hWnd As IntPtr, pszSubAppName As String, pszSubIdList As String) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Friend Function ShowWindow(ByVal hwnd As IntPtr, ByVal nCmdShow As ShowWindowCommands) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

End Module

Friend Enum ShowWindowCommands As Integer

    Hide = 0
    Normal = 1
    ShowMinimized = 2
    Maximize = 3
    ShowMaximized = 3
    ShowNoActivate = 4
    Show = 5
    Minimize = 6
    ShowMinNoActive = 7
    ShowNA = 8
    Restore = 9
    ShowDefault = 10
    ForceMinimize = 11

End Enum

