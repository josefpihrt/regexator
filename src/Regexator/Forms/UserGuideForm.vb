Imports System.IO
Imports Regexator.Windows.Forms

Public Class UserGuideForm

    Public Sub New()

        InitializeComponent()
        SuspendLayout()
        Padding = New Padding(1, 1, 1, 1)
        Text = My.Resources.Regexator & " " & My.Resources.UserGuide
        StartPosition = FormStartPosition.Manual
        KeyPreview = True
        Icon = My.Resources.IcoRegexator

        _web = New WebBrowser() With {.Dock = DockStyle.Fill, .ScriptErrorsSuppressed = True, .IsWebBrowserContextMenuEnabled = False,
            .WebBrowserShortcutsEnabled = False, .AllowWebBrowserDrop = False}
        _sts = New StatusStrip() With {.BackColor = SystemColors.Control}
        _lblStatus = New ToolStripStatusLabel() With {.TextAlign = ContentAlignment.MiddleLeft, .Spring = True}

        _sts.Items.Add(_lblStatus)
        Controls.Add(_web)
        Controls.Add(_sts)
        ResumeLayout(True)

        AddHandler _web.StatusTextChanged, AddressOf Browser_StatusTextChanged
        AddHandler _web.Navigating, AddressOf Browser_Navigating

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        SetLocationAndSize(My.Settings.UserGuideFormWindowState, My.Settings.UserGuideFormLocation, My.Settings.UserGuideFormSize)
        If File.Exists(_fileUrl.OriginalString) Then
            _web.Navigate(_fileUrl)
        End If
        MyBase.OnLoad(e)

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        My.Settings.UserGuideFormLocation = Location
        My.Settings.UserGuideFormSize = Size
        My.Settings.UserGuideFormWindowState = WindowState
        MyBase.OnFormClosing(e)

    End Sub

    Private Sub Browser_StatusTextChanged(sender As Object, e As EventArgs)

        Dim result As Uri = Nothing
        If String.IsNullOrEmpty(_web.StatusText) OrElse (Uri.TryCreate(_web.StatusText, UriKind.Absolute, result) AndAlso IsInternal(result) = False) Then
            _lblStatus.Text = _web.StatusText
        Else
            _lblStatus.Text = String.Empty
        End If

    End Sub

    Private Sub Browser_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs)

        If IsInternal(e.Url) = False Then
            e.Cancel = True
            Process.Start(e.Url.ToString())
        End If

    End Sub

    Private Shared Function IsInternal(url As Uri) As Boolean

        Return url Is Nothing OrElse _schemes.Contains(url.Scheme)

    End Function

    Private ReadOnly _web As WebBrowser
    Private ReadOnly _sts As StatusStrip
    Private ReadOnly _lblStatus As ToolStripStatusLabel

    Private Shared ReadOnly _fileUrl As Uri = New Uri(GetFilePath(AppFile.UserGuideHtml))
    Private Shared ReadOnly _schemes As String() = {"file", "mhtml"}

End Class