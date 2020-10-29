Imports System.IO
Imports System.ComponentModel
Imports Regexator.FileSystem
Imports Regexator.Windows.Forms

Public Class SnippetDirectoryDataGridView
    Inherits ExtendedDataGridView

    Public Sub New()

        Dock = DockStyle.Fill
        BorderStyle = BorderStyle.None
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
        AllowUserToAddRows = True
        AllowUserToDeleteRows = True
        AlternatingRowsDefaultCellStyle = Nothing
        Me.ReadOnly = False
        MultiSelect = True
        EnableHeadersVisualStyles = True
        ColumnHeadersVisible = False
        ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        AllowUserToResizeColumns = False
        AllowUserToResizeRows = False
        AllowDrop = True
        Dock = DockStyle.Fill

    End Sub

    Protected Overrides Sub OnDataError(displayErrorDialogIfNoHandler As Boolean, e As DataGridViewDataErrorEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim ex = e.Exception
        If TypeOf ex Is ArgumentException OrElse
                TypeOf ex Is PathTooLongException OrElse
                TypeOf ex Is NotSupportedException OrElse
                TypeOf ex Is System.Security.SecurityException Then
            Dim row = Rows(e.RowIndex)
            row.ErrorText = ex.GetBaseException().Message
            displayErrorDialogIfNoHandler = False
            e.ThrowException = False
        Else
            e.ThrowException = True
        End If
        MyBase.OnDataError(displayErrorDialogIfNoHandler, e)

    End Sub

    Protected Overrides Sub OnCellValidating(e As DataGridViewCellValidatingEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim row = Rows(e.RowIndex)
        row.ErrorText = String.Empty
        MyBase.OnCellValidating(e)

    End Sub

    Protected Overrides Sub OnDragEnter(drgevent As DragEventArgs)

        If drgevent Is Nothing Then Throw New ArgumentNullException("drgevent")
        Dim e = drgevent
        If IsCurrentCellInEditMode = False OrElse CurrentCell.OwningRow.IsNewRow = False Then
            Try
                If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                    e.Effect = DragDropEffects.Link
                End If
            Catch ex As System.Runtime.InteropServices.ExternalException
            Catch ex As System.Threading.ThreadStateException
            End Try
        End If
        MyBase.OnDragEnter(e)

    End Sub

    Protected Overrides Sub OnDragDrop(drgevent As DragEventArgs)

        If drgevent Is Nothing Then Throw New ArgumentNullException("drgevent")
        Dim e = drgevent
        If (e.Effect And DragDropEffects.Link) = DragDropEffects.Link Then
            Dim paths As String() = Nothing
            Try
                If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                    paths = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
                End If
            Catch ex As System.Runtime.InteropServices.ExternalException
            Catch ex As System.Threading.ThreadStateException
            End Try
            If paths IsNot Nothing Then
                Dim lst = TryCast(DataSource, BindingList(Of SnippetDirectory))
                If lst IsNot Nothing Then
                    For Each di In paths.Select(Function(f) Executor.CreateDirectoryInfo(f)).Where(Function(f) f IsNot Nothing)
                        lst.Add(New SnippetDirectory(di))
                    Next
                End If
            End If
        End If
        MyBase.OnDragDrop(drgevent)

    End Sub

End Class
