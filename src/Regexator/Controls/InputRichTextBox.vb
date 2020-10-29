Imports System.ComponentModel
Imports Regexator.Text
Imports Regexator.Output
Imports Regexator.Snippets
Imports Regexator.Windows.Forms
Imports System.Text.RegularExpressions
Imports System.IO
Imports Pihrtsoft.Text.RegularExpressions.Linq
Imports Pihrtsoft.Text.RegularExpressions.Linq.Extensions

Public Class InputRichTextBox
    Inherits RegexRichTextBox

    Private Shared ReadOnly _dirRegex As Regex = _
        Patterns.EntireInput(
            Patterns _
                .WhileWhiteSpace() _
                .NamedGroup("value", Patterns.NotChar(Path.GetInvalidPathChars()).MaybeMany().Lazy()) _
                .WhileChar(Chars.WhiteSpace() + Path.DirectorySeparatorChar)
        ).ToRegex()

    Private Shared ReadOnly _splitPathRegex As Regex = Patterns.NoncapturingGroup(Patterns.Linefeed, Path.PathSeparator).ToRegex()

    Public Sub New()

        MyBase.New()
        WordWrap = My.Settings.InputWordWrap
        Name = "rtbInput"
        _senseLazy = New Lazy(Of SnippetSense)(Function() New SnippetSense(Me))
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

        Dim tsiGoTo = ToolStripItemFactory.CreateGoToItem(
            Sub() SelectFirstCapture(),
            Sub() SelectPreviousCapture(False),
            Sub() SelectNextCapture(False),
            Sub() SelectLastCapture())
        Yield tsiGoTo
        Yield New ToolStripSeparator()
        Dim tsiSelectValue = New ToolStripMenuItem(My.Resources.SelectValue)
        AddHandler tsiSelectValue.Click,
            Sub()
                Dim node = TryCast(tsiSelectValue.Tag, RegexBlock)
                If node IsNot Nothing Then
                    Me.Select(node.InputSpan.Index, node.InputSpan.Length)
                End If
            End Sub
        Yield tsiSelectValue
        Dim tsiCopyValue = New ToolStripMenuItem(My.Resources.CopyValue)
        AddHandler tsiCopyValue.Click,
            Sub()
                Dim node = TryCast(tsiCopyValue.Tag, RegexBlock)
                If node IsNot Nothing Then
                    AppUtility.SetClipboardText(node.Value)
                    Me.Select(node.InputSpan.Index, node.InputSpan.Length)
                End If
            End Sub
        Yield tsiCopyValue
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateAdjustSelectionItem(Me)
        Yield ToolStripItemFactory.CreateSelectedLinesItem(Me)
        Yield New ToolStripSeparator()
        Dim tsiEncoding = ToolStripItemFactory.CreateInputEncodingItem()
        Yield tsiEncoding
        Yield New ToolStripSeparator()
        Dim tsiWordWrap = ToolStripItemFactory.CreateInputWordWrapItem()
        Yield tsiWordWrap
        Yield New ToolStripSeparator()
        For Each item In ToolStripItemFactory.CreateCutCopyPasteItems(Me)
            Yield item
        Next
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(Me)
        Dim action =
            Sub()
                tsiGoTo.Enabled = Not SelectionOnlyActive
                tsiEncoding.Enabled = (Panels.Input.Kind = InputKind.File)
                tsiWordWrap.Checked = WordWrap
                Dim item = Searcher.FindItem(GetCharIndexFromMousePosition())
                Dim flg = item IsNot Nothing AndAlso item.Block.InputSpan IsNot Nothing AndAlso item.Block IsNot Nothing
                tsiSelectValue.Enabled = flg
                tsiSelectValue.Tag = If(flg, item.Block, Nothing)
                tsiCopyValue.Enabled = flg
                tsiCopyValue.Tag = If(flg, item.Block, Nothing)
            End Sub
        AddHandler cms.Opening, Sub() action()
        action()

    End Function

    Public Iterator Function EnumerateDirectories() As IEnumerable(Of String)

        If Me.TextLength = 0 Then
            Return
        End If

        Dim hs As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each value As String In _splitPathRegex.EnumerateSplit(CurrentText(NewLineMode.Lf))
            If value.Length > 0 Then
                Dim s = _dirRegex.Match(value).Group("value").Value
                If s.Length > 0 AndAlso Path.IsPathRooted(s) Then
                    If s(s.Length - 1) <> Path.DirectorySeparatorChar Then
                        s = s & Path.DirectorySeparatorChar
                    End If
                    If hs.Add(s) Then
                        Yield s
                    End If
                End If
            End If
        Next

    End Function

    Public Sub SelectFirstCapture()

        If SelectionOnlyActive = False Then
            Dim item = Searcher.FirstItem
            If item IsNot Nothing Then
                SelectCapture(item.Block)
            End If
        End If

    End Sub

    Public Sub SelectPreviousCapture(enableCurrent As Boolean)

        If SelectionOnlyActive = False Then
            Dim index As Integer = SelectionStart - IndexOffset
            Dim result = Searcher.FindItems(index)
            If result IsNot Nothing AndAlso result.Current.Block.InputSpan IsNot Nothing Then
                Dim item As IndexSearcherItem = result.Previous
                If enableCurrent AndAlso index > result.Current.Block.InputSpan.Index Then
                    item = result.Current
                End If
                If item IsNot Nothing Then
                    SelectCapture(item.Block)
                End If
            End If
        End If

    End Sub

    Public Sub SelectNextCapture(enableCurrent As Boolean)

        If SelectionOnlyActive = False Then
            Dim index As Integer = SelectionStart - IndexOffset
            Dim result = Searcher.FindItems(index)
            If result IsNot Nothing AndAlso result.Current.Block.InputSpan IsNot Nothing Then
                Dim item As IndexSearcherItem = result.Next
                If enableCurrent AndAlso index < result.Current.Block.InputSpan.Index Then
                    item = result.Current
                End If
                If item IsNot Nothing Then
                    SelectCapture(item.Block)
                End If
            End If
        End If

    End Sub

    Public Sub SelectLastCapture()

        If SelectionOnlyActive = False Then
            Dim item = Searcher.LastItem
            If item IsNot Nothing Then
                SelectCapture(item.Block)
            End If
        End If

    End Sub

    Private Shared Sub SelectCapture(block As RegexBlock)

        Panels.Output.Highlight(block, HighlightSource.None)

    End Sub

    Public Overrides Sub PrintText()

        Drawing.PrintUtility.Print(Me, Explorer.CurrentInputFullName)

    End Sub

    Public Sub ShowTitleCharacterList()

        Sense.Show(Data.Characters, Function(f) New SnippetTitleSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Public Sub ShowCodeCharacterList()

        Sense.Show(Data.Characters, Function(f) New SnippetCodeSenseItem(f), SnippetOptions.HideCategory)

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Select Case e.Modifiers
            Case Keys.Alt
                Select Case e.KeyCode
                    Case Keys.Left
                        SelectPreviousCapture(True)
                    Case Keys.Right
                        SelectNextCapture(True)
                End Select
            Case Keys.Control
                Select Case e.KeyCode
                    Case Keys.W
                        ShowTitleCharacterList()
                        e.SuppressKeyPress = True
                    Case Keys.E
                        Sense.ShowRecentTitles()
                        e.SuppressKeyPress = True
                    Case Keys.B
                        Sense.InsertLastRecent()
                        e.SuppressKeyPress = True
                End Select
            Case (Keys.Shift Or Keys.Alt)
                Select Case e.KeyCode
                    Case Keys.Left
                        SelectFirstCapture()
                    Case Keys.Right
                        SelectLastCapture()
                End Select
            Case (Keys.Shift Or Keys.Control)
                Select Case e.KeyCode
                    Case Keys.W
                        ShowCodeCharacterList()
                        e.SuppressKeyPress = True
                    Case Keys.E
                        Sense.ShowRecentValues()
                        e.SuppressKeyPress = True
                    Case Keys.B
                        Sense.InsertLastRecentCode()
                        e.SuppressKeyPress = True
                End Select
        End Select
        MyBase.OnKeyDown(e)

    End Sub

    Public ReadOnly Property SelectionOnlyActive As Boolean
        Get
            Return SelectionOnly AndAlso CurrentLineOnly = False AndAlso SelectionLength > 0
        End Get
    End Property

    Public Property Searcher As IndexSearcher
        Get
            Return _searcher
        End Get
        Set(value As IndexSearcher)
            _searcher = If(value, IndexSearcher.Empty)
        End Set
    End Property

    Public ReadOnly Property Sense As SnippetSense
        Get
            Return _senseLazy.Value
        End Get
    End Property

    Private ReadOnly _senseLazy As Lazy(Of SnippetSense)
    Private _searcher As IndexSearcher = IndexSearcher.Empty

End Class
