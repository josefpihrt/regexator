Imports System.ComponentModel
Imports Regexator.Snippets

Public Class ReplacementRichTextBox
    Inherits RegexRichTextBox

    Public Sub New()

        WordWrap = My.Settings.ReplacementWordWrap
        _senseLazy = New Lazy(Of SnippetSense)(Function() New SnippetSense(Me))
        Dim cms As New ContextMenuStrip()
        AddHandler cms.Opening,
            Sub(sender As Object, e As CancelEventArgs)
                If cms.Items.Count = 0 Then
                    cms.Items.AddRange(CreateItems(cms).ToArray())
                    e.Cancel = False
                End If
            End Sub
        ContextMenuStrip = cms

    End Sub

    Private Iterator Function CreateItems(cms As ContextMenuStrip) As IEnumerable(Of ToolStripItem)

        Dim factory = Sense.Factory
        Yield factory.CreateSubstitutionSnippetsItem()
        Yield factory.CreateRecentItem()
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateAdjustSelectionItem(Me)
        Yield ToolStripItemFactory.CreateSelectedLinesItem(Me)
        Yield New ToolStripSeparator()
        Dim tsiWordWrap = ToolStripItemFactory.CreateReplacementWordWrapItem()
        Yield tsiWordWrap
        Yield New ToolStripSeparator()
        For Each item In ToolStripItemFactory.CreateCutCopyPasteItems(Me)
            Yield item
        Next
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(Me)
        Dim action = Sub() tsiWordWrap.Checked = WordWrap
        AddHandler cms.Opening, Sub() action()
        action()

    End Function

    Public Sub ShowTitleSnippetList()

        Sense.Show(Data.EnumerateSubstitutionSnippets())

    End Sub

    Public Sub ShowCodeSnippetList()

        Sense.Show(Data.EnumerateSubstitutionSnippets(), Function(f) New SnippetCodeSenseItem(f))

    End Sub

    Public Sub ShowTitleCharacterList()

        Sense.Show(Data.Characters, Function(f) New SnippetTitleSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Public Sub ShowCodeCharacterList()

        Sense.Show(Data.Characters, Function(f) New SnippetCodeSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.Modifiers = Keys.None Then
        ElseIf e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.Q Then
                ShowTitleSnippetList()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.W Then
                ShowTitleCharacterList()
            ElseIf e.KeyCode = Keys.S Then
                Explorer.SaveReplacement()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.E Then
                Sense.ShowRecentTitles()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.B Then
                Sense.InsertLastRecent()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Space Then
                ShowTitleSnippetList()
                e.SuppressKeyPress = True
            End If
        ElseIf e.Modifiers = Keys.Shift Then
        ElseIf e.Modifiers = (Keys.Control Or Keys.Shift) Then
            If e.KeyCode = Keys.Q Then
                ShowCodeSnippetList()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.W Then
                ShowCodeCharacterList()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.E Then
                Sense.ShowRecentValues()
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.B Then
                Sense.InsertLastRecentCode()
                e.SuppressKeyPress = True
            End If
        End If
        MyBase.OnKeyDown(e)

    End Sub

    Public Overrides Sub PrintText()

        Drawing.PrintUtility.Print(Me, My.Resources.Description)

    End Sub

    Public ReadOnly Property Sense As SnippetSense
        Get
            Return _senseLazy.Value
        End Get
    End Property

    Private ReadOnly _senseLazy As Lazy(Of SnippetSense)

End Class