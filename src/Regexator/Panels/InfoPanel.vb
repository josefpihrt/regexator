Imports Regexator
Imports Regexator.UI
Imports System.Diagnostics.CodeAnalysis

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class InfoPanel

    Public Sub New()

        _pgd = New ProjectInfoPropertyGrid()
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}
        _tsp = New AppToolStrip()
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .Enabled = False}

        _btnSave = New ToolStripButton(Nothing, My.Resources.IcoSave.ToBitmap(), Sub(sender As Object, e As EventArgs) Explorer.SaveProjectInfo()) With {.ToolTipText = My.Resources.SaveProjectInfo & " " & My.Resources.CtrlS.AddParentheses()}

        ProjectInfo = New ProjectInfo()
        AddHandler _pgd.PropertyValueChanged, Sub(sender As Object, e As PropertyValueChangedEventArgs) Summary.ReloadSummary()
        AddHandler _pgd.Enter,
            Sub()
                If My.Settings.TrackActiveItemInSolutionExplorer Then
                    Explorer.SelectCurrentProjectNode()
                End If
            End Sub

        _tsp.Items.AddRange({_btnSave, New ToolStripSeparator()})
        _pnlBox.Controls.Add(_pgd)
        _pnl.Controls.AddRange({_pnlBox, _tsp})

        App.Formats.DescriptionText.Controls.Add(_pgd)

    End Sub

    Public Sub Load(container As ProjectContainer)

        ProjectInfo = If(container IsNot Nothing, container.ProjectInfo, New ProjectInfo())
        _pnl.Enabled = container IsNot Nothing

    End Sub

    Public Property ProjectInfo As ProjectInfo
        Get
            Return DirectCast(_pgd.SelectedInfo.Clone(), ProjectInfo)
        End Get
        Private Set(value As ProjectInfo)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            _pgd.SelectedObject = DirectCast(value.Clone(), ProjectInfo)
        End Set
    End Property

    Friend _pgd As ProjectInfoPropertyGrid
    Friend _pnlBox As Panel
    Friend _tsp As AppToolStrip
    Friend _pnl As Panel

    Friend _btnSave As ToolStripButton

End Class