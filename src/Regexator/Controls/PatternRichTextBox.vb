Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports Regexator.Snippets
Imports Regexator.Windows.Forms

Public Class PatternRichTextBox
    Inherits RegexRichTextBox

    Public Sub New()

        MyBase.New()
        NoTab = True
        Name = "rtbPattern"
        WordWrap = My.Settings.PatternWordWrap
        Exporter.IsPattern = True
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
        Yield factory.CreateFavoriteSnippetsItem()
        Yield factory.CreateRecentItem()
        Yield factory.CreateGroupedSnippetsItem()
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateAdjustSelectionItem(Me, True)
        Yield ToolStripItemFactory.CreateSelectedLinesItem(Me, True)
        Yield New ToolStripSeparator()
        Dim tsiWordWrap = ToolStripItemFactory.CreatePatternWordWrapItem()
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

    Protected Overrides Function ProcessCmdKey(ByRef m As Message, keyData As Keys) As Boolean

        Dim modifiers = keyData And Keys.Modifiers
        Dim keyCode = keyData And Keys.KeyCode
        Dim isAltPressed = ((modifiers And Keys.Alt) = Keys.Alt)
        If keyCode = Keys.Enter AndAlso isAltPressed = False AndAlso My.Settings.PatternIndentNewLine Then
            Dim indent As String = String.Empty
            If TextLength > 0 Then
                Dim index As Integer = GetLineStartIndex(SelectionStart)
                indent = _indentRegex.Match(Text.Substring(index, SelectionStart - index)).Value
            End If
            SelectedText = vbLf & indent
            Return True
        End If
        Return MyBase.ProcessCmdKey(m, keyData)

    End Function

    Public Overrides Sub PrintText()

        Drawing.PrintUtility.Print(Me, Explorer.CurrentProjectPath)

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
            ElseIf e.KeyCode = Keys.C Then
                If KeyboardUtility.IsKeyDown(Keys.K) Then
                    AddLineComment()
                    e.SuppressKeyPress = True
                End If
            ElseIf e.KeyCode = Keys.U Then
                If KeyboardUtility.IsKeyDown(Keys.K) Then
                    RemoveLineComment()
                    e.SuppressKeyPress = True
                End If
            End If
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

    Public Sub ShowTitleSnippetList()

        Sense.Show(Data.EnumerateSnippets(), Function(f) New SnippetTitleSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Public Sub ShowCodeSnippetList()

        Sense.Show(Data.EnumerateSnippets(), Function(f) New SnippetCodeSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Public Sub ShowTitleCharacterList()

        Sense.Show(Data.PatternCharacters, Function(f) New SnippetTitleSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Public Sub ShowCodeCharacterList()

        Sense.Show(Data.PatternCharacters, Function(f) New SnippetCodeSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Public Sub AddLineComment()

        BeginUpdate()
        TextBoxUtility.AddLineComment(Me)
        EndUpdate()

    End Sub

    Public Sub RemoveLineComment()

        BeginUpdate()
        TextBoxUtility.RemoveLineComment(Me)
        EndUpdate()

    End Sub

    Public ReadOnly Property Sense As SnippetSense
        Get
            Return _senseLazy.Value
        End Get
    End Property

    Private ReadOnly _senseLazy As Lazy(Of SnippetSense)

    Private Shared ReadOnly _indentRegex As Regex = New Regex("^\ *")

End Class
