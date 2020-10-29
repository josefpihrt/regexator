Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Regexator
Imports Regexator.Text
Imports Regexator.Windows.Forms

Public Class CurrentLineBox

    Public Sub New(rtb As RegexRichTextBox)

        InitializeComponent()
        _rtb = rtb
        TabStop = False
        Dock = DockStyle.Top
        BorderStyle = BorderStyle.None
        AutoSize = False
        Padding = New Padding(2, 3, 2, 0)
        ResizeRedraw = True

        _box = New ExtendedRichTextBox With {
            .ContextMenuStrip = CreateContextMenuStrip(),
            .ReadOnly = True,
            .Multiline = False,
            .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None,
            .DetectUrls = False
        }

        Controls.Add(_box)
        Height = FontHeight + _box.Margin.Vertical
        AddHandler _box.ContentsResized, AddressOf Box_ContentResized
        AddHandler _box.GotFocus, AddressOf Box_GotFocus

    End Sub

    Private Function CreateContextMenuStrip() As ContextMenuStrip

        Dim cms As New ContextMenuStrip()
        cms.Items.AddRange(CreateItems(cms).ToArray())
        Return cms

    End Function

    Private Iterator Function CreateItems(cms As ContextMenuStrip) As IEnumerable(Of ToolStripItem)

        Dim tsiIncludeNewLine = New ToolStripMenuItem(My.Resources.IncludeNewLine, Nothing, Sub() _rtb.CurrentLineIncludesNewLine = Not _rtb.CurrentLineIncludesNewLine)
        Yield tsiIncludeNewLine
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.Cut, My.Resources.IcoCut.ToBitmap(),
            Sub()
                If _rtb.TextLength > 0 Then
                    _rtb.BeginUpdate()
                    TextBoxUtility.CutCurrentLine(_rtb)
                    _rtb.EndUpdate()
                End If
            End Sub)
        Yield New ToolStripMenuItem(My.Resources.Copy, My.Resources.IcoCopy.ToBitmap(),
            Sub()
                If _rtb.TextLength > 0 Then
                    _rtb.BeginUpdate()
                    TextBoxUtility.CopyCurrentLine(_rtb)
                    _rtb.EndUpdate()
                End If
            End Sub)
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.Duplicate, Nothing, AddressOf DuplicateItem_Click)
        AddHandler cms.Opening, Sub() tsiIncludeNewLine.Checked = _rtb.CurrentLineIncludesNewLine

    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim c = Color.DarkGray
        Dim s = ButtonBorderStyle.Solid
        Dim w = 1
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
            c, w, s,
            c, w, s,
            c, w, s,
            Color.Empty, 0, ButtonBorderStyle.None)
        MyBase.OnPaint(e)

    End Sub

    Private Sub DuplicateItem_Click(sender As Object, e As EventArgs)

        If _rtb.Lines.Any() Then
            _rtb.BeginUpdate()
            _rtb.SelectLineEnd(_rtb.GetStartLine())
            _rtb.SelectedText = vbLf & _rtb.GetCurrentLineText()
            _rtb.SelectCurrentLine(False)
            _rtb.EndUpdate()
        End If

    End Sub

    Private Sub Box_ContentResized(sender As Object, e As ContentsResizedEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Height = e.NewRectangle.Height + _box.Margin.Vertical

    End Sub

    Private Sub Box_GotFocus(sender As Object, e As EventArgs)

        _rtb.GoToCursor()
        _rtb.SelectCurrentLine()

    End Sub

    Private Sub ProcessText()

        If _box.TextLength = 0 Then
            _box.Text = My.Resources.Empty.ToLower(CultureInfo.CurrentCulture)
            _box.SelectAll()
            _box.SelectionFont = New Font(_box.Font, FontStyle.Italic)
            _box.SelectionColor = Color.Gray
        End If

    End Sub

    Public Overrides Property Text As String
        Get
            Return _box.Text
        End Get
        Set(value As String)
            Using New RedrawDisabler(_box)
                _box.ClearFormat()
                _box.Text = _endingNewLineRegex.Replace(value, Function(m) TextProcessor.ProcessSymbols(m.Value, Panels.Output.OutputSettings))
                ProcessText()
            End Using
        End Set
    End Property

    Private ReadOnly _rtb As RegexRichTextBox
    Private ReadOnly _box As ExtendedRichTextBox
    Private Shared ReadOnly _endingNewLineRegex As Regex = New Regex("\r?\n\z")

End Class
