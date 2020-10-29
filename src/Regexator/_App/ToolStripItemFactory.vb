Imports System.Text
Imports Regexator
Imports Regexator.Text
Imports Regexator.FileSystem
Imports Regexator.Windows.Forms

Public NotInheritable Class ToolStripItemFactory

    Private Sub New()

    End Sub

    Public Shared Function CreateAdjustSelectionItem(rtb As RegexRichTextBox) As ToolStripMenuItem

        Return CreateAdjustSelectionItem(rtb, False)

    End Function

    Public Shared Function CreateAdjustSelectionItem(rtb As RegexRichTextBox, addEscapeItems As Boolean) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.AdjustSelection, Nothing, CreateAdjustSelectionItems(rtb, addEscapeItems).ToArray())

    End Function

    Private Shared Iterator Function CreateAdjustSelectionItems(rtb As RegexRichTextBox, addEscapeItems As Boolean) As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.ToLower, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(Function() TextBoxUtility.SelectionToLower(rtb))) With {.ShortcutKeyDisplayString = My.Resources.CtrlU}
        Yield New ToolStripMenuItem(My.Resources.ToUpper, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(Function() TextBoxUtility.SelectionToUpper(rtb))) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftU}
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.ToSingleLine, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(Function() rtb.SelectionToSingleline(True)))
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.RemoveEmptyLines, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(Function() TextBoxUtility.RemoveEmptyLinesFromSelection(rtb)))
        Yield New ToolStripMenuItem(My.Resources.RemoveWhiteSpaceLines, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(Function() TextBoxUtility.RemoveWhiteSpaceLinesFromSelection(rtb)))
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.TrimLeadingWhiteSpaceMultiline, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(AddressOf rtb.TrimSelectionStartMultiline))
        Yield New ToolStripMenuItem(My.Resources.TrimTrailingWhiteSpaceMultiline, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(AddressOf rtb.TrimSelectionEndMultiline))
        Yield New ToolStripMenuItem(My.Resources.TrimWhiteSpaceMultiline, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(AddressOf rtb.TrimSelectionMultiline))
        If addEscapeItems Then
            Yield New ToolStripSeparator()
            Yield New ToolStripMenuItem(My.Resources.Escape, Nothing, Sub() RichTextBoxUtility.EscapeSelection(rtb))
            Yield New ToolStripMenuItem(My.Resources.Unescape, Nothing,
                Sub()
                    Try
                        RichTextBoxUtility.UnescapeSelection(rtb)
                    Catch ex As ArgumentException
                        MessageDialog.Warning(ex.Message)
                    End Try
                End Sub)
        End If

    End Function

    Public Shared Function CreateSelectedLinesItem(rtb As RegexRichTextBox) As ToolStripMenuItem

        Return CreateSelectedLinesItem(rtb, False)

    End Function

    Public Shared Function CreateSelectedLinesItem(rtb As RegexRichTextBox, addCommentItems As Boolean) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.SelectedLines, Nothing, CreateSelectedLinesItems(rtb, addCommentItems).ToArray())

    End Function

    Private Shared Iterator Function CreateSelectedLinesItems(rtb As RegexRichTextBox, addCommentItems As Boolean) As IEnumerable(Of ToolStripItem)

        If addCommentItems Then
            Yield New ToolStripMenuItem(
                My.Resources.Comment,
                My.Resources.IcoCommentAdd.ToBitmap(),
                Sub() TextBoxUtility.AddLineComment(rtb)) With {.ShortcutKeyDisplayString = My.Resources.CtrlKCtrlC}
            Yield New ToolStripMenuItem(
                My.Resources.Uncomment,
                My.Resources.IcoCommentRemove.ToBitmap(),
                Sub() TextBoxUtility.RemoveLineComment(rtb)) With {.ShortcutKeyDisplayString = My.Resources.CtrlKCtrlU}
            Yield New ToolStripSeparator()
        End If
        Yield New ToolStripMenuItem(My.Resources.MoveUp, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(AddressOf rtb.MoveSelectionUp)) With {.ShortcutKeyDisplayString = My.Resources.AltUp}
        Yield New ToolStripMenuItem(My.Resources.MoveDown, Nothing, Sub() rtb.DoWithoutCurrentTextChanged(AddressOf rtb.MoveSelectionDown)) With {.ShortcutKeyDisplayString = My.Resources.AltDown}

    End Function

    Public Shared Function CreateResetZoomItem(rtb As RichTextBox) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.ResetZoom, Nothing, Sub() rtb.ResetZoom()) With {.ShortcutKeyDisplayString = My.Resources.CtrlAlt0}

    End Function

    Public Shared Function CreateInputEncodingItem() As ToolStripMenuItem

        Dim item = New ToolStripMenuItem(My.Resources.Encoding, Nothing, New ToolStripMenuItem())
        AddHandler item.DropDownOpening,
            Sub()
                item.DropDownItems.Clear()
                If FileSystemUtility.CommonEncodings.Contains(Panels.Input.Encoding) = False Then
                    item.DropDownItems.Add(CreateEncodingItem(Panels.Input.Encoding))
                    item.DropDownItems.Add(New ToolStripSeparator())
                End If
                item.DropDownItems.AddRange(CreateCommonEncodingsItems().ToArray())
                item.DropDownItems.Add(New ToolStripSeparator())
                item.DropDownItems.Add(CreateAllEncodingsItem)
                Dim tsi = item.DropDownItems.OfType(Of ToolStripMenuItem).Where(Function(f) f.DropDownItems.Count = 0).FirstOrDefault(Function(f) Panels.Input.Encoding.CodePage = CInt(f.Tag))
                If tsi IsNot Nothing Then
                    tsi.Checked = True
                End If
            End Sub
        item.Enabled = False
        Return item

    End Function

    Private Shared Function CreateCommonEncodingsItems() As IEnumerable(Of ToolStripMenuItem)

        Return FileSystemUtility.CommonEncodings.Select(Function(f) CreateEncodingItem(f))

    End Function

    Private Shared Function CreateAllEncodingsItem() As ToolStripMenuItem

        Dim item As New ToolStripMenuItem(My.Resources.AllEncodings, Nothing, New ToolStripMenuItem())
        AddHandler item.DropDownOpening,
            Sub()
                item.DropDownItems.Clear()
                item.DropDownItems.AddRange(CreateAllEncodingsItems().ToArray())
                Dim tsi = item.DropDownItems _
                    .OfType(Of ToolStripMenuItem) _
                    .FirstOrDefault(Function(f) Panels.Input.Encoding.CodePage = CInt(f.Tag))
                If tsi IsNot Nothing Then
                    tsi.Checked = True
                End If
            End Sub
        Return item

    End Function

    Private Shared Function CreateAllEncodingsItems() As IEnumerable(Of ToolStripMenuItem)

        Return Encoding.GetEncodings() _
            .OrderBy(Function(f) f.DisplayName) _
            .Select(Function(f) CreateEncodingItem(f))

    End Function

    Private Shared Function CreateEncodingItem(encoding As Encoding) As ToolStripMenuItem

        Dim item As New ToolStripMenuItem(encoding.EncodingName, Nothing, Sub() Panels.Input.Encoding = encoding)
        item.Tag = encoding.CodePage
        item.ToolTipText = My.Resources.Name & My.Resources.ColonStr & " " & encoding.WebName & My.Resources.CommaStr & " " &
            My.Resources.CodePage & My.Resources.ColonStr & " " & encoding.CodePage
        Return item

    End Function

    Private Shared Function CreateEncodingItem(info As EncodingInfo) As ToolStripMenuItem

        Dim item As New ToolStripMenuItem(info.DisplayName, Nothing, Sub() Panels.Input.Encoding = info.GetEncoding())
        item.Tag = info.CodePage
        item.ToolTipText = My.Resources.Name & My.Resources.ColonStr & " " & info.Name & My.Resources.CommaStr & " " &
            My.Resources.CodePage & My.Resources.ColonStr & " " & info.CodePage
        Return item

    End Function

    Public Shared Function CreateGoToItem(findFirst As Action, findPrevious As Action, findNext As Action, findLast As Action) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.GoTo_, Nothing, {
            New ToolStripMenuItem(My.Resources.FirstItem, My.Resources.IcoFindFirst.ToBitmap(), Sub() findFirst()) With {
                .ShortcutKeyDisplayString = My.Resources.AltShiftLeft},
            New ToolStripMenuItem(My.Resources.PreviousItem, My.Resources.IcoFindPrevious.ToBitmap(), Sub() findPrevious()) With {
                .ShortcutKeyDisplayString = My.Resources.AltLeft},
            New ToolStripMenuItem(My.Resources.NextItem, My.Resources.IcoFindNext.ToBitmap(), Sub() findNext()) With {
                .ShortcutKeyDisplayString = My.Resources.AltRight},
            New ToolStripMenuItem(My.Resources.LastItem, My.Resources.IcoFindLast.ToBitmap(), Sub() findLast()) With {
                .ShortcutKeyDisplayString = My.Resources.AltShiftRight}})

    End Function

    Public Shared Function CreateCutCopyPasteItems(rtb As ExtendedRichTextBox) As ToolStripMenuItem()

        Return {CreateCutItem(rtb), CreateCopyItem(rtb), CreatePasteItem(rtb)}

    End Function

    Public Shared Iterator Function CreateCutCopyPasteItems(rtb As RegexRichTextBox) As IEnumerable(Of ToolStripMenuItem)

        If rtb Is Nothing Then Throw New ArgumentNullException("rtb")
        Yield CreateCutItem(rtb)
        Yield CreateCopyItem(rtb)
        Yield CreateCopySpecialItem(rtb)
        Yield CreatePasteItem(rtb)
        Yield rtb.Exporter.CreateImportItem(My.Resources.PasteSpecial)

    End Function

    Private Shared Function CreateCopySpecialItem(rtb As RegexRichTextBox) As ToolStripMenuItem

        Dim item As New ToolStripMenuItem(My.Resources.CopySpecial)
        item.DropDownItems.AddRange(CreateCopySpecialItems(item, rtb).ToArray())
        Return item

    End Function

    Private Shared Iterator Function CreateCopySpecialItems(parent As ToolStripMenuItem, rtb As RegexRichTextBox) As IEnumerable(Of ToolStripItem)

        For Each item In rtb.Exporter.CreateExportItems(TextKind.SelectionOrCurrent, parent)
            Yield item
        Next
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.AsSingleLine, Nothing, Sub() AppUtility.SetClipboardText(rtb.SelectedText.ToSingleline()))

    End Function

    Public Shared Function CreateCutItem(rtb As ExtendedRichTextBox) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.Cut, My.Resources.IcoCut.ToBitmap(), Sub() rtb.Cut()) With {
            .ShortcutKeyDisplayString = My.Resources.CtrlX}

    End Function

    Public Shared Function CreateCopyItem(rtb As RichTextBox) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.Copy, My.Resources.IcoCopy.ToBitmap(), Sub() rtb.Copy()) With {
            .ShortcutKeyDisplayString = My.Resources.CtrlC}

    End Function

    Public Shared Function CreatePasteItem(rtb As ExtendedRichTextBox) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.Paste, My.Resources.IcoPaste.ToBitmap(), Sub() rtb.PasteText()) With {
            .ShortcutKeyDisplayString = My.Resources.CtrlV}

    End Function

    Public Shared Function CreateCopyAllItem(rtb As RichTextBox) As ToolStripMenuItem

        Return CreateCopyAllItem(Sub() rtb.CopyAll())

    End Function

    Public Shared Function CreateCopyAllItem(onClick As EventHandler) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.CopyAll, Nothing, onClick)

    End Function

    Public Shared Function CreateSelectAllItem(rtb As RichTextBox) As ToolStripMenuItem

        Return New ToolStripMenuItem(
            My.Resources.SelectAll,
            Nothing,
            Sub() rtb.SelectAll()) With {.ShortcutKeyDisplayString = My.Resources.CtrlA}

    End Function

    Public Shared Function CreatePrintItem(rtb As ExtendedRichTextBox) As ToolStripMenuItem

        Return New ToolStripMenuItem(
            My.Resources.Print & My.Resources.EllipsisStr,
            My.Resources.IcoPrint.ToBitmap(),
            Sub() rtb.PrintText()) With {.ShortcutKeyDisplayString = My.Resources.CtrlP}

    End Function

    Public Shared Function CreateNewLineModeItem(value As INewLineMode) As ToolStripMenuItem

        Dim tsiLf = New ToolStripMenuItem(My.Resources.Linefeed)
        AddHandler tsiLf.Click,
            Sub()
                If tsiLf.Checked = False Then
                    value.ToggleNewLine()
                End If
            End Sub
        Dim tsiCrLf = New ToolStripMenuItem(My.Resources.CarriageReturnPlusLinefeed)
        AddHandler tsiCrLf.Click,
            Sub()
                If tsiCrLf.Checked = False Then
                    value.ToggleNewLine()
                End If
            End Sub
        Dim tsi As New ToolStripMenuItem(My.Resources.NewLine, Nothing, tsiLf, tsiCrLf)
        AddHandler tsi.DropDownOpening,
            Sub()
                tsiLf.Checked = (value.NewLineMode = Regexator.Text.NewLineMode.Lf)
                tsiCrLf.Checked = (value.NewLineMode = Regexator.Text.NewLineMode.CrLf)
            End Sub
        Return tsi

    End Function

    Public Shared Function CreateSaveItem(saveAsTxt As Action, saveAsRtf As Action) As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.SaveTextAs & My.Resources.EllipsisStr, Nothing, {
            New ToolStripMenuItem(My.Resources.PlainText, Nothing, Sub() saveAsTxt()),
            New ToolStripMenuItem(My.Resources.RichTextFormat, Nothing, Sub() saveAsRtf())})

    End Function

    Public Shared Function CreatePatternWordWrapItem() As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.WordWrap, Nothing, Sub() Panels.Pattern.ToggleWordWrap()) With {.Checked = Panels.Pattern.WordWrap}

    End Function

    Public Shared Function CreateReplacementWordWrapItem() As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.WordWrap, Nothing, Sub() Panels.Replacement.ToggleWordWrap()) With {.Checked = Panels.Replacement.WordWrap}

    End Function

    Public Shared Function CreateInputWordWrapItem() As ToolStripMenuItem

        Return New ToolStripMenuItem(My.Resources.WordWrap, Nothing, Sub() Panels.Input.ToggleWordWrap()) With {.Checked = Panels.Input.WordWrap}

    End Function

End Class