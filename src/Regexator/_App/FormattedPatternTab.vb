
#If TATA Then

Public Class FormattedPanel

    Public Sub New()

        _rtb = New FormattedPatternRichTextBox()
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}

        _pnl.Controls.Add(_rtb)
        App.Formats.Text.Controls.Add(_rtb)

    End Sub

    Public Sub LoadText(text As String)

        Dim formatter As New PatternFormatter()
        _rtb.Text = formatter.Format(text, _tabString)

    End Sub

    Public ReadOnly Property Text As String
        Get
            Return _rtb.Text
        End Get
    End Property

    Friend _rtb As FormattedPatternRichTextBox
    Friend _pnl As Panel

    Private _tabString As String = "    "

End Class

#End If

