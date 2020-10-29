Imports Regexator.UI
Imports Regexator.Windows.Forms

Imports System.Diagnostics.CodeAnalysis

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class SummaryPanel

    Public Sub New()

        _rtb = New SummaryRichTextBox()
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .BackColor = _rtb.BackColor}
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _tspFind = New AppSearchToolStrip(_rtb, False)

        _pnlBox.Controls.Add(_rtb)
        _pnl.Controls.AddRange({_pnlBox, _tspFind})

        App.Formats.SummaryText.Controls.Add(_rtb)
        App.Formats.SummaryText.Controls.Add(_pnl)

    End Sub

    Public Sub ClearSummary(suppressUpdate As Boolean)

        _rtb.ClearSummary(suppressUpdate)

    End Sub

    Public Sub ReloadSummary()

        _rtb.ClearSummary(App.MainForm.IsSummaryTabSelected)
        If App.MainForm.IsSummaryTabSelected Then
            LoadSummary()
        End If

    End Sub

    Public Sub LoadSummary()

        If Panels.Output.OutputEnabled AndAlso _rtb.Loaded = False Then
            _rtb.CreateSummary()
        End If

    End Sub

    Public Sub CopyAll()

        _rtb.CopyAll()

    End Sub

    Public Sub CopySummaryText()

        LoadSummary()
        CopyAll()

    End Sub

    Friend _rtb As SummaryRichTextBox
    Friend _pnl As Panel
    Friend _pnlBox As Panel
    Friend _tspFind As SearchToolStrip

End Class
