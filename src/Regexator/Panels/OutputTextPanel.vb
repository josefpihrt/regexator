Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports Regexator.Output
Imports Regexator.UI
Imports Regexator.Text
Imports Regexator.Windows.Forms

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class OutputTextPanel

    Public Sub New()

        _rtb = New OutputRichTextBox()
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .BackColor = _rtb.BackColor}
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _tspFind = New AppSearchToolStrip(_rtb, False)

        AddHandler _rtb.SelectionChanged,
            Sub()
                If Panels.Output.Suppressed = False AndAlso _rtb.SelectionLength = 0 Then
                    Dim item = _rtb.Searcher.FindItem(_rtb.SelectionStart)
                    If item IsNot Nothing Then
                        Panels.Output.Highlight(item.Block, HighlightSource.Text)
                    End If
                End If
            End Sub

        _pnlBox.Controls.Add(_rtb)
        _pnl.Controls.AddRange({_pnlBox, _tspFind})

        App.Formats.Text.Controls.Add(_rtb)
        App.Formats.Text.Controls.Add(_pnl)

    End Sub

    Public Sub Highlight(builder As RegexBuilder)

        Highlight(builder, HighlightOptions.DisplayRectangle)

    End Sub

    Public Sub Highlight(builder As RegexBuilder, options As HighlightOptions)

        _rtb.Highlight(builder, options)

    End Sub

    Public Sub Highlight(block As RegexBlock, source As HighlightSource)

        Using blocker As New ScrollBlocker(_rtb)
            _rtb.ClearFormat()
            Dim selection = If(Highlight(block), TextSpan.Empty)
            If selection IsNot Nothing Then
                blocker.Restore(selection)
            Else
                blocker.Restore()
            End If
            If source = HighlightSource.Text Then
                _rtb.Select(blocker.TextSpan)
            ElseIf selection IsNot Nothing Then
                Dim index = _rtb.GetLineStartIndex(selection.Index)
                If _rtb.IsCharVisible(index) = False Then
                    _rtb.Select(index)
                End If
                _rtb.Select(selection.Index)
            End If
        End Using

    End Sub

    Private Function Highlight(block As RegexBlock) As TextSpan

        If Panels.Output.HasOptions(OutputOptions.Highlight) Then
            Return _rtb.Highlight(block)
        ElseIf block IsNot Nothing Then
            Return block.TextSpans.FirstOrDefault()
        End If
        Return Nothing

    End Function

    Public Sub LoadMessage(header As String)

        LoadMessage(header, Nothing)

    End Sub

    Public Sub LoadMessage(header As String, content As String)

        LoadMessage(header, content, Color.Empty)

    End Sub

    Public Sub LoadMessage(header As String, content As String, foreColor As Color)

        If header Is Nothing Then Throw New ArgumentNullException("header")

        Using New RedrawDisabler(_rtb)
            ClearAll(False)
            _rtb.Text = header.ToUpper(CultureInfo.CurrentCulture).Enclose(" ").Enclose(New String("-"c, 5))
            Dim length As Integer = _rtb.TextLength
            If content IsNot Nothing Then
                _rtb.AppendText(vbLf & vbLf & content)
            End If
            _rtb.Select(0, length)
            _rtb.SelectionFont = New Font(_rtb.SelectionFont, FontStyle.Bold)
            If foreColor <> Color.Empty Then
                _rtb.SelectionColor = foreColor
            End If
            _rtb.SelectBeginning()
        End Using

    End Sub

    Public Sub ClearText()

        _rtb.Clear()

    End Sub

    Public Sub ClearAll(suppressUpdate As Boolean)

        _rtb.ClearAll(suppressUpdate)

    End Sub

    Public Property Text As String
        Get
            Return _rtb.Text
        End Get
        Set(value As String)
            _rtb.Text = value
        End Set
    End Property

    Public Property Searcher As IndexSearcher
        Get
            Return _rtb.Searcher
        End Get
        Set(value As IndexSearcher)
            _rtb.Searcher = value
        End Set
    End Property

    Friend _rtb As OutputRichTextBox
    Friend _pnl As Panel
    Friend _pnlBox As Panel
    Friend _tspFind As SearchToolStrip

End Class
