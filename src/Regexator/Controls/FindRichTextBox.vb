Imports System.ComponentModel
Imports Regexator.Windows.Forms

Public NotInheritable Class FindRichTextBox
    Inherits ExtendedRichTextBox

    Private _currentLineIndex As Integer

    Public Sub New()

        WordWrap = False
        DetectUrls = False
        HideSelection = False
        Dock = DockStyle.Fill
        BorderStyle = BorderStyle.None
        Me.ReadOnly = True

        Dim cms As New ContextMenuStrip()
        AddHandler cms.Opening,
            Sub(sender As Object, e As CancelEventArgs)
                If cms.Items.Count = 0 Then
                    cms.Items.AddRange(CreateToolStripItems(cms).ToArray())
                    e.Cancel = False
                End If
            End Sub
        ContextMenuStrip = cms

    End Sub

    Private Iterator Function CreateToolStripItems(cms As ContextMenuStrip) As IEnumerable(Of ToolStripItem)

        Yield ToolStripItemFactory.CreateCopyItem(Me)
        Yield ToolStripItemFactory.CreateCopyAllItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveFindResultsAsTxt(), Sub() Commands.SaveFindResultsAsRtf())
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(Me)

    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        MyBase.OnKeyDown(e)

    End Sub

    Protected Overrides Sub OnSelectionChanged(e As EventArgs)

        Dim lineIndex = Me.GetLineFromCharIndexModified(SelectionStart)
        If _currentLineIndex <> lineIndex Then
            _currentLineIndex = lineIndex
            OnCurrentLineChanged(EventArgs.Empty)
        End If

        MyBase.OnSelectionChanged(e)

    End Sub

    Public Overrides Sub PrintText()

        Drawing.PrintUtility.Print(Me, My.Resources.FindResults)

    End Sub

    Private Sub OnCurrentLineChanged(e As EventArgs)

        RaiseEvent CurrentLineChanged(Me, e)

    End Sub

    Public ReadOnly Property CurrentLineIndex As Integer
        Get
            Return _currentLineIndex
        End Get
    End Property

    Public Event CurrentLineChanged As EventHandler

End Class