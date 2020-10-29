Imports Regexator.Windows.Forms

Public Class SummaryRichTextBox
    Inherits ExtendedRichTextBox

    Public Sub New()

        MyBase.New()
        Dock = DockStyle.Fill
        BorderStyle = BorderStyle.None
        Me.ReadOnly = True
        DetectUrls = False
        HideSelection = False
        WordWrap = My.Settings.OutputSummaryWordWrap
        ContextMenuStrip = New ContextMenuStrip()
        ContextMenuStrip.Items.AddRange(CreateToolStripItems().ToArray())

    End Sub

    Private Iterator Function CreateToolStripItems() As IEnumerable(Of ToolStripItem)

        Yield ToolStripItemFactory.CreateCopyItem(Me)
        Yield ToolStripItemFactory.CreateCopyAllItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveSummaryAsTxt(), Sub() Commands.SaveSummaryAsRtf())
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(Me)

    End Function

    Public Sub CreateSummary()

        Dim builder = Panels.Output.Builder
        If builder IsNot Nothing Then
            Dim creator = New SummaryCreator(builder.Blocks)
            creator.CreateSummary()
            _searcher = creator.Searcher
        End If
        _loaded = True

    End Sub

    Public Sub ClearSummary(suppressUpdate As Boolean)

        If suppressUpdate Then
            BeginUpdate()
        End If
        _searcher = Int32BinarySearcher.Empty
        SelectAll()
        SelectionBackColor = BackColor
        SelectionColor = ForeColor
        SelectionFont = Font
        Clear()
        If suppressUpdate Then
            EndUpdate()
        End If
        _loaded = False

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.Modifiers = Keys.Alt Then
            If e.KeyCode = Keys.Left Then
                SelectPreviousElement()
            ElseIf e.KeyCode = Keys.Right Then
                SelectNextElement()
            End If
        ElseIf e.Modifiers = (Keys.Alt Or Keys.Shift) Then
            If e.KeyCode = Keys.Left Then
                SelectFirstElement()
            ElseIf e.KeyCode = Keys.Right Then
                SelectLastElement()
            End If
        End If
        MyBase.OnKeyDown(e)

    End Sub

    Public Overrides Sub PrintText()

        If Loaded = False Then
            CreateSummary()
        End If
        Drawing.PrintUtility.Print(Me, My.Resources.Summary)

    End Sub

    Private Sub SelectFirstElement()

        Dim index = _searcher.FirstItem
        If index <> -1 Then
            Me.Select(index, 0)
        End If

    End Sub

    Private Sub SelectPreviousElement()

        Dim index As Integer = SelectionStart
        Dim result = _searcher.FindItems(index)
        If result IsNot Nothing AndAlso result.Current <> -1 Then
            If result.Current = index AndAlso result.Previous <> -1 Then
                Me.Select(result.Previous, 0)
            Else
                Me.Select(result.Current, 0)
            End If
        End If

    End Sub

    Private Sub SelectNextElement()

        Dim index As Integer = SelectionStart
        Dim result = _searcher.FindItems(index)
        If result IsNot Nothing AndAlso result.Next <> -1 Then
            Me.Select(result.Next, 0)
        End If

    End Sub

    Private Sub SelectLastElement()

        Dim index = _searcher.LastItem
        If index <> -1 Then
            Me.Select(index, 0)
        End If

    End Sub

    Public ReadOnly Property Loaded As Boolean
        Get
            Return _loaded
        End Get
    End Property

    Private _loaded As Boolean
    Private _searcher As Int32BinarySearcher = Int32BinarySearcher.Empty

End Class