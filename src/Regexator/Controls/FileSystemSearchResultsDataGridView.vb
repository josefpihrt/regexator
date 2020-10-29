Imports System.Diagnostics.CodeAnalysis
Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports Regexator.Collections.Generic
Imports Regexator.IO
Imports Regexator.Windows.Forms

Public Class FileSystemSearchResultsDataGridView
    Inherits ExtendedDataGridView

    Private _cms As FileSystemSearchResultsContextMenuStrip

    Public Property DirectoryNameColumn As DataGridViewTextBoxColumn
    Public Property FileNameColumn As DataGridViewTextBoxColumn
    Public Property NewFileNameColumn As DataGridViewTextBoxColumn
    Public Property RenameColumn As DataGridViewDisableButtonColumn
    Public Property DeleteColumn As DataGridViewDisableButtonColumn

    <SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")>
    Public Sub New()

        Dock = DockStyle.Fill
        Me.ReadOnly = True
        MultiSelect = True
        BorderStyle = BorderStyle.None
        AllowUserToAddRows = False
        AllowUserToDeleteRows = False
        EnableHeadersVisualStyles = True
        RowHeadersVisible = True
        AllowUserToResizeColumns = True
        AllowUserToResizeRows = False
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
        AutoGenerateColumns = False
        AlternatingRowsDefaultCellStyle = DefaultCellStyle

        DirectoryNameColumn = New DataGridViewTextBoxColumn() With {.Name = "DirectoryName", .DataPropertyName = "DirectoryName", .HeaderText = My.Resources.DirectoryName.ToLowerInvariant(), .ReadOnly = True}
        FileNameColumn = New DataGridViewTextBoxColumn() With {.Name = "Name", .DataPropertyName = "Name", .HeaderText = My.Resources.Name.ToLowerInvariant(), .ReadOnly = True}
        NewFileNameColumn = New DataGridViewTextBoxColumn() With {.Name = "NewName", .DataPropertyName = "NewName", .HeaderText = My.Resources.NewName.ToLowerInvariant(), .ReadOnly = True}
        RenameColumn = New DataGridViewDisableButtonColumn() With {.Name = "Rename", .ReadOnly = True, .HeaderText = My.Resources.Rename.ToLowerInvariant(), .Text = My.Resources.Rename.ToLowerInvariant(), .UseColumnTextForButtonValue = True}
        DeleteColumn = New DataGridViewDisableButtonColumn() With {.Name = "Delete", .ReadOnly = True, .HeaderText = My.Resources.Delete.ToLowerInvariant(), .Text = My.Resources.Delete.ToLowerInvariant(), .UseColumnTextForButtonValue = True}

        Me.Columns.Add(DirectoryNameColumn)
        Me.Columns.Add(FileNameColumn)
        Me.Columns.Add(NewFileNameColumn)
        Me.Columns.Add(RenameColumn)
        Me.Columns.Add(DeleteColumn)

        SetFormat()
        SetSelectionFormat()

        AddHandler App.Formats.TableText.FormatChanged, Sub() SetFormat()
        AddHandler App.Formats.TableSelectedText.FormatChanged, Sub() SetSelectionFormat()

    End Sub

    Private Sub SetFormat()

        Dim info = App.Formats.TableText
        If info.BackColorEnabled Then
            DefaultCellStyle.BackColor = info.BackColor
        End If
        If info.ForeColorEnabled Then
            DefaultCellStyle.ForeColor = info.ForeColor
        End If
        DefaultCellStyle.Font = info.Font

    End Sub

    Private Sub SetSelectionFormat()

        Dim info = App.Formats.TableSelectedText
        If info.BackColorEnabled Then
            DefaultCellStyle.SelectionBackColor = info.BackColor
        End If
        If info.ForeColorEnabled Then
            DefaultCellStyle.SelectionForeColor = info.ForeColor
        End If
        DefaultCellStyle.Font = info.Font

    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        Dim modifiers = keyData And Keys.Modifiers
        Dim keyCode = keyData And Keys.KeyCode
        If modifiers = Keys.None Then
            If keyCode = Keys.F4 Then
                OpenSelectedItemInNotepad()
                Return True
            End If
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

    Protected Overrides Function ProcessDataGridViewKey(e As System.Windows.Forms.KeyEventArgs) As Boolean

        If e Is Nothing Then
            Throw New ArgumentNullException("e")
        End If

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Delete Then
                DeleteSelectedItems()
                Return True
            ElseIf e.KeyCode = Keys.F2 Then
                RenameSelectedItems()
                Return True
            End If
        End If

        Return MyBase.ProcessDataGridViewKey(e)

    End Function

    Protected Overrides Sub OnCellClick(e As DataGridViewCellEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.RowIndex <> -1 Then
            If e.ColumnIndex = RenameColumn.Index Then
                If DirectCast(Rows(e.RowIndex).DataBoundItem, SearchResult).CanRename AndAlso MessageDialog.Question(My.Resources.DoYouReallyWantToRenameThisItem) = DialogResult.Yes Then
                    RenameItem(Rows(e.RowIndex))
                End If
            ElseIf e.ColumnIndex = DeleteColumn.Index Then
                If DirectCast(Rows(e.RowIndex).DataBoundItem, SearchResult).CanDelete Then
                    Dim message = If(My.Settings.FileSystemUseRecycleBin, My.Resources.DoYouReallyWantToMoveThisItemToRecycleBin, My.Resources.DoYouReallyWantToPermanentlyDeleteThisItem)
                    If MessageDialog.Question(message) = DialogResult.Yes Then
                        DeleteItem(Rows(e.RowIndex))
                    End If
                End If
            End If
        End If
        MyBase.OnCellClick(e)

    End Sub

    Private Sub DeleteSelectedItems()

        If IsAnySelectedRowToDelete Then
            Dim message = If(My.Settings.FileSystemUseRecycleBin, My.Resources.DoYouReallyWantToMoveSelectedItemsToRecycleBin, My.Resources.DoYouReallyWantToPermanentlyDeleteSelectedItems)
            If MessageDialog.Question(message) = DialogResult.Yes Then
                For Each row As DataGridViewRow In SelectedRows.Cast(Of DataGridViewRow).OrderBy(Function(f) f.Index)
                    DeleteItem(row)
                Next
            End If
        End If

    End Sub

    Private Sub DeleteItem(row As DataGridViewRow)

        Dim useRecycleBin = My.Settings.FileSystemUseRecycleBin
        Dim result = DirectCast(row.DataBoundItem, SearchResult)
        If result.CanDelete Then
            If result.Delete(If(useRecycleBin, RecycleOption.SendToRecycleBin, RecycleOption.DeletePermanently)) Then
                Me.Rows.Remove(row)
            Else
                row.ErrorText = result.Message
            End If
        End If

    End Sub

    Private Sub RenameSelectedItems()

        If IsAnySelectedRowToRename Then
            If MessageDialog.Question(My.Resources.DoYouReallyWantToRenameSelectedItems) = DialogResult.Yes Then
                For Each row As DataGridViewRow In SelectedRows.Cast(Of DataGridViewRow).OrderBy(Function(f) f.Index)
                    RenameItem(row)
                Next
            End If
        End If

    End Sub

    Private Sub RenameItem(row As DataGridViewRow)

        Dim result = DirectCast(row.DataBoundItem, SearchResult)
        If result.CanRename Then
            If result.Rename() Then
                DirectCast(row.Cells(RenameColumn.Index), DataGridViewDisableButtonCell).Enabled = False
                row.ErrorText = Nothing
                row.Cells(FileNameColumn.Index).Style.Font = New Font(DefaultCellStyle.Font, DefaultCellStyle.Font.Style Or FontStyle.Strikeout)
            Else
                row.ErrorText = result.Message
            End If
        End If

    End Sub

    Private Shared Sub OpenFile(row As DataGridViewRow)

        Dim result = DirectCast(row.DataBoundItem, SearchResult)
        If Not result.IsDeleted Then
            Process.Start(result.FullName)
        End If

    End Sub

    Private Sub OpenSelectedItemInNotepad()

        Dim result = GetSingleSelectedFile()
        If result IsNot Nothing AndAlso result.IsDeleted = False Then
            Process.Start("notepad.exe", result.FullName)
        End If

    End Sub

    Private Sub OpenSelectedItemContainingFolder()

        Dim result = GetSingleSelectedItem()
        If result IsNot Nothing Then
            If File.Exists(result.FullName) Then
                Process.Start("explorer.exe", String.Concat("/select, """, result.FullName, """"))
            ElseIf Directory.Exists(result.DirectoryName) Then
                Process.Start("explorer.exe", String.Concat("/select, """, result.DirectoryName, """"))
            End If
        End If

    End Sub

    Friend Sub ResizeDisplayedRowsAndColumns()

        AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCells, False)
        AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells, True)

    End Sub

    Protected Overrides Sub OnColumnHeaderMouseClick(e As DataGridViewCellMouseEventArgs)

        CurrentCell = Nothing
        MyBase.OnColumnHeaderMouseClick(e)

    End Sub

    Protected Overrides Sub OnScroll(e As ScrollEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")

        If e.ScrollOrientation = ScrollOrientation.VerticalScroll Then
            AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCells, True)
        End If

        MyBase.OnScroll(e)

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")

        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.C Then
                CopySelectedItems()
            End If
        End If

        MyBase.OnKeyDown(e)

    End Sub

    Protected Overrides Sub OnCellFormatting(e As DataGridViewCellFormattingEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")

        If e.RowIndex <> -1 AndAlso e.ColumnIndex <> -1 Then
            If e.ColumnIndex = NewFileNameColumn.Index Then
                Dim item = DirectCast(Rows(e.RowIndex).DataBoundItem, SearchResult)
                Dim cell = Me.Item(e.ColumnIndex, e.RowIndex)
                cell.ErrorText = Nothing
                If item.IsNewNameUnchanged Then
                    e.Value = String.Format("<{0}>", My.Resources.Unchanged.ToLowerInvariant())
                    e.FormattingApplied = True
                    If item.IsDeleted Then
                        e.CellStyle.Font = New Font(DefaultCellStyle.Font, DefaultCellStyle.Font.Style Or FontStyle.Strikeout)
                    End If
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                Else
                    If DirectCast(Rows(e.RowIndex).DataBoundItem, SearchResult).IsValidNewName = False Then
                        cell.ErrorText = My.Resources.NewNameIsInvalidMsg
                    End If
                End If
            ElseIf e.ColumnIndex = RenameColumn.Index Then
                If DirectCast(Rows(e.RowIndex).DataBoundItem, SearchResult).CanRename = False Then
                    Dim cell = DirectCast(Item(e.ColumnIndex, e.RowIndex), DataGridViewDisableButtonCell)
                    cell.Enabled = False
                End If
            End If
        End If

        MyBase.OnCellFormatting(e)

    End Sub

    Protected Overrides Sub OnCellContextMenuStripNeeded(e As DataGridViewCellContextMenuStripNeededEventArgs)

        If e.ColumnIndex <> -1 AndAlso e.RowIndex <> -1 Then

            If _cms Is Nothing Then
                _cms = New FileSystemSearchResultsContextMenuStrip()
                AddHandler _cms.OpenItem.Click, Sub() OpenSelectedItemInNotepad()
                AddHandler _cms.OpenContainingFolderItem.Click, Sub() OpenSelectedItemContainingFolder()
                AddHandler _cms.CopyItem.Click, Sub() CopySelectedItems()
                AddHandler _cms.RenameItem.Click, Sub() RenameSelectedItems()
                AddHandler _cms.DeleteItem.Click, Sub() DeleteSelectedItems()
            End If

            _cms.OpenItem.Enabled = IsSingleFileSelected
            _cms.OpenContainingFolderItem.Enabled = (SelectedRows.Count = 1)
            _cms.CopyItem.Enabled = (SelectedRows.Count = 1)
            _cms.RenameItem.Enabled = IsAnySelectedRowToRename
            _cms.DeleteItem.Enabled = IsAnySelectedRowToDelete

            e.ContextMenuStrip = _cms

        End If

        MyBase.OnCellContextMenuStripNeeded(e)

    End Sub

    Private Iterator Function GetSelectedPaths() As IEnumerable(Of String)

        For Each row As DataGridViewRow In SelectedRows
            Yield DirectCast(row.DataBoundItem, SearchResult).FullName
        Next

    End Function

    Private ReadOnly Property IsAnySelectedRowToDelete As Boolean
        Get
            Return SelectedRows.Cast(Of DataGridViewRow).Any(Function(f) DirectCast(f.DataBoundItem, SearchResult).CanDelete)
        End Get
    End Property

    Private ReadOnly Property IsAnySelectedRowToRename As Boolean
        Get
            Return SelectedRows.Cast(Of DataGridViewRow).Any(Function(f) DirectCast(f.DataBoundItem, SearchResult).CanRename)
        End Get
    End Property

    Private ReadOnly Property IsSingleFileSelected As Boolean
        Get
            Return SelectedRows.Count = 1 AndAlso DirectCast(SelectedRows(0).DataBoundItem, SearchResult).IsFile
        End Get
    End Property

    Private Function GetSingleSelectedFile() As SearchResult

        If SelectedRows.Count = 1 Then
            Dim result = DirectCast(SelectedRows(0).DataBoundItem, SearchResult)
            If result.IsFile Then
                Return result
            End If
        End If

        Return Nothing

    End Function

    Private Function GetSingleSelectedItem() As SearchResult

        If SelectedRows.Count = 1 Then
            Dim result = DirectCast(SelectedRows(0).DataBoundItem, SearchResult)
            Return result
        End If

        Return Nothing

    End Function

    Private Sub CopySelectedItems()

        Clipboard.SetFileDropList(GetSelectedPaths().ToStringCollection())

    End Sub

    Protected Overrides ReadOnly Property ShowFocusCues As Boolean
        Get
            Return False
        End Get
    End Property

End Class
