Imports System.ComponentModel
Imports Regexator.Output
Imports Regexator.Text
Imports Regexator.Windows.Forms

Public Class OutputRichTextBox
    Inherits ExtendedRichTextBox

    Public Sub New()

        MyBase.New()
        Dock = DockStyle.Fill
        BorderStyle = BorderStyle.None
        Me.ReadOnly = True
        DetectUrls = False
        HideSelection = False
        Searcher = IndexSearcher.Empty
        WordWrap = My.Settings.OutputTextWordWrap

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

    Public Overloads Sub Highlight(builder As RegexBuilder)

        Highlight(builder, HighlightOptions.DisplayRectangle)

    End Sub

    Public Overloads Sub Highlight(builder As RegexBuilder, options As HighlightOptions)

        If builder Is Nothing Then
            Throw New ArgumentNullException("builder")
        End If

        Using blocker As New ScrollBlocker(Me)

            ClearFormat()

            If (options And HighlightOptions.DisplayRectangle) = HighlightOptions.DisplayRectangle Then
                HighlightDisplayRectangle(builder)
            Else
                HighlightAll(builder)
            End If

            Me.Select(blocker.TextSpan)

        End Using

    End Sub

    Private Sub HighlightAll(builder As RegexBuilder)

        App.Formats.Capture.Highlight(Me, builder.Blocks.SelectMany(Function(f) f.TextSpans))
        App.Formats.Symbol.Highlight(Me, builder.Blocks.SelectMany(Function(f) f.SymbolTextSpans))

    End Sub

    Private Sub HighlightDisplayRectangle(builder As RegexBuilder)

        Dim first = GetCharIndexFromPosition(New Point(DisplayRectangle.Left, DisplayRectangle.Top))
        Dim last = GetCharIndexFromPosition(New Point(DisplayRectangle.Right, DisplayRectangle.Bottom))
        Dim textSpan = New TextSpan(first, last - first)

        App.Formats.Capture.Highlight(Me, EnumerateSpans(textSpan, builder.Blocks).SelectMany(Function(f) f.TextSpans))
        App.Formats.Symbol.Highlight(Me, EnumerateSpans(textSpan, builder.Blocks).SelectMany(Function(f) f.SymbolTextSpans))

    End Sub

    Private Iterator Function EnumerateSpans(span As TextSpan, blocks As IEnumerable(Of RegexBlock)) As IEnumerable(Of RegexBlock)

        Dim flg As Boolean = False

        For Each block As RegexBlock In blocks
            If flg = False Then
                If block.EndIndex >= span.Index Then
                    flg = True
                    Yield block
                End If
            ElseIf block.StartIndex <= span.EndIndex Then
                Yield block
            Else
                Return
            End If
        Next

    End Function

    Public Overloads Function Highlight(block As RegexBlock) As TextSpan

        If block Is Nothing Then
            Dim builder = Panels.Output.Builder
            If builder IsNot Nothing Then
                Highlight(builder)
                Return builder.Blocks.SelectMany(Function(f) f.TextSpans).FirstOrDefault()
            End If
        Else
            If App.Mode = EvaluationMode.Replace AndAlso My.Settings.OutputHighlightBeforeAfterResult Then
                HighlightBeforeAndAfterResult(block)
            End If
            App.Formats.Info.Highlight(Me, block.InfoSelections.SelectMany(Function(f) f.EnumerateItems()))
            App.Formats.Capture.Highlight(Me, block.TextSpans)
            App.Formats.Symbol.Highlight(Me, block.SymbolTextSpans)
            Dim selections = block.TextSpans
            If selections.Count > 0 Then
                Return selections.Combine()
            End If
        End If

        Return Nothing

    End Function

    Private Sub HighlightBeforeAndAfterResult(block As RegexBlock)

        Dim blocks = Panels.Output.Builder?.Blocks
        If blocks IsNot Nothing AndAlso blocks.Count > 0 Then
            If block.MatchItemIndex > 0 Then
                Dim index = blocks(block.MatchItemIndex - 1).TextSpan.EndIndex
                Dim endIndex = block.TextSpan.Index
                App.Formats.BeforeAfterResult.Highlight(Me, index, endIndex - index)
            End If
            If block.MatchItemIndex < (blocks.Count - 1) Then
                Dim index = block.TextSpan.EndIndex
                Dim endIndex = blocks(block.MatchItemIndex + 1).TextSpan.Index
                App.Formats.BeforeAfterResult.Highlight(Me, index, endIndex - index)
            End If
        End If

    End Sub

    Private Sub SelectFirstCapture()

        Dim item = Searcher.FirstItem
        If item IsNot Nothing Then
            SelectCapture(item)
        End If

    End Sub

    Private Sub SelectPreviousCapture(enableCurrent As Boolean)

        Dim index As Integer = SelectionStart
        Dim result = Searcher.FindItems(index)
        If result IsNot Nothing AndAlso result.Current.SelectionIndex <> -1 Then
            Dim item As IndexSearcherItem = result.Previous
            If enableCurrent AndAlso index > result.Current.SelectionIndex Then
                item = result.Current
            End If
            If item IsNot Nothing Then
                SelectCapture(item)
            End If
        End If

    End Sub

    Private Sub SelectNextCapture(enableCurrent As Boolean)

        Dim index As Integer = SelectionStart
        Dim result = Searcher.FindItems(index)
        If result IsNot Nothing AndAlso result.Current.SelectionIndex <> -1 Then
            Dim item As IndexSearcherItem = result.Next
            If enableCurrent AndAlso index < result.Current.SelectionIndex Then
                item = result.Current
            End If
            If item IsNot Nothing Then
                SelectCapture(item)
            End If
        End If

    End Sub

    Private Sub SelectLastCapture()

        Dim item = Searcher.LastItem
        If item IsNot Nothing Then
            SelectCapture(item)
        End If

    End Sub

    Private Shared Sub SelectCapture(item As IndexSearcherItem)

        Panels.Output.Highlight(item.Block, HighlightSource.None)

    End Sub

    Public Sub ClearAll(suppressUpdate As Boolean)

        If suppressUpdate Then
            BeginUpdate()
        End If
        ClearFormat()
        Clear()
        If suppressUpdate Then
            EndUpdate()
        End If

    End Sub

    Private Iterator Function CreateToolStripItems(cms As ContextMenuStrip) As IEnumerable(Of ToolStripItem)

        Yield ToolStripItemFactory.CreateGoToItem(
            Sub() SelectFirstCapture(),
            Sub() SelectPreviousCapture(False),
            Sub() SelectNextCapture(False),
            Sub() SelectLastCapture())
        Dim block As RegexBlock = Nothing
        Dim tse = New ToolStripSeparator()
        Dim tsiSelectValue = New ToolStripMenuItem(My.Resources.SelectValue, Nothing,
            Sub()
                Dim span = block.TextSpans.Combine()
                Me.Select(span.Index, span.Length)
            End Sub)
        Dim tsiCopyValue = New ToolStripMenuItem(My.Resources.CopyValue, Nothing,
            Sub()
                AppUtility.SetClipboardText(GetOutputValue(block))
                Dim span = block.TextSpans.Combine()
                Me.Select(span.Index, span.Length)
            End Sub)
        Yield tse
        Yield tsiSelectValue
        Yield tsiCopyValue
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateCopyItem(Me)
        Yield ToolStripItemFactory.CreateCopyAllItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveOutputAsTxt(), Sub() Commands.SaveOutputAsRtf())
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(Me)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(Me)
        Dim action = Sub()
                         Dim item = Searcher.FindItem(GetCharIndexFromMousePosition())
                         block = item?.Block.ValueBlock
                         tse.Visible = (block IsNot Nothing)
                         tsiSelectValue.Visible = (block IsNot Nothing)
                         tsiCopyValue.Visible = (block IsNot Nothing)
                     End Sub
        action()
        AddHandler cms.Opening, Sub() action()

    End Function

    Private Shared Function GetOutputValue(block As RegexBlock) As String

        Dim replaceBlock = TryCast(block, ReplaceBlock)
        If replaceBlock IsNot Nothing Then
            Return replaceBlock.Result.Value
        End If
        Return block.Value

    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.Modifiers = Keys.Alt Then
            If e.KeyCode = Keys.Left Then
                SelectPreviousCapture(True)
            ElseIf e.KeyCode = Keys.Right Then
                SelectNextCapture(True)
            End If
        ElseIf e.Modifiers = (Keys.Alt Or Keys.Shift) Then
            If e.KeyCode = Keys.Left Then
                SelectFirstCapture()
            ElseIf e.KeyCode = Keys.Right Then
                SelectLastCapture()
            End If
        End If
        MyBase.OnKeyDown(e)

    End Sub

    Public Overrides Sub PrintText()

        Drawing.PrintUtility.Print(Me, My.Resources.Output)

    End Sub

    Public Property Searcher As IndexSearcher
        Get
            Return _searcher
        End Get
        Set(value As IndexSearcher)
            _searcher = If(value, IndexSearcher.Empty)
        End Set
    End Property

    Private _searcher As IndexSearcher

End Class