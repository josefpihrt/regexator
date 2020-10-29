Imports System.ComponentModel
Imports System.Globalization
Imports Regexator.Text
Imports Regexator.Output
Imports Regexator.Windows.Forms

Public Class OutputDataGridView
    Inherits ExtendedDataGridView

    Public Sub New()

        Dock = DockStyle.Fill
        Me.ReadOnly = True
        MultiSelect = True
        AllowUserToAddRows = False
        AllowUserToDeleteRows = False
        EnableHeadersVisualStyles = True
        RowHeadersVisible = False
        AlternatingRowsDefaultCellStyle = Nothing
        AllowUserToResizeColumns = True
        AllowUserToResizeRows = True
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        SelectionMode = DataGridViewSelectionMode.CellSelect
        MatchModeLayout = If(My.Settings.OutputMatchTableByGroups, TableLayout.Group, TableLayout.Value)
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

    Public Sub ClearData()

        DataSource = Nothing
        _builder = Nothing
        _loaded = False

    End Sub

    Public Sub Load(builder As RegexBuilder)

        If builder Is Nothing Then Throw New ArgumentNullException("builder")
        _builder = builder
        DefaultCellStyle.NullValue = builder.Settings.Symbols.NoCapture
        Dim matchBuilder = TryCast(builder, MatchBuilder)
        If matchBuilder IsNot Nothing Then
            SelectionMode = If(MatchModeLayout = TableLayout.Group, DataGridViewSelectionMode.CellSelect, DataGridViewSelectionMode.FullRowSelect)
            Load(If(MatchModeLayout = TableLayout.Group, matchBuilder.GetGroupTable(), matchBuilder.GetTable()))
        Else
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Load(builder.GetTable())
        End If

    End Sub

    Private Sub Load(table As DataTable)

        If table.Rows.Count > 0 Then
            _suppressSelectionChanged = True
            DataSource = table
            ResizeDisplayedRowsAndColumns()
            ClearSelection()
            _suppressSelectionChanged = False
            _loaded = True
        End If

    End Sub

    Public Sub Highlight(block As RegexBlock)

        If block Is Nothing Then
            ClearSelection()
            CurrentCell = Nothing
            Dim col = Columns.GetFirstVisibleNotFrozen()
            If col IsNot Nothing Then
                FirstDisplayedScrollingColumnIndex = col.Index
            End If
            Dim row = Rows.GetFirstVisibleNotFrozen()
            If row IsNot Nothing Then
                FirstDisplayedScrollingRowIndex = row.Index
            End If
        Else
            Select Case Mode
                Case EvaluationMode.Match
                    If MatchModeLayout = TableLayout.Group Then
                        SelectCell(block.MatchItemIndex.ToString(CultureInfo.InvariantCulture), block.GroupName)
                    ElseIf MatchModeLayout = TableLayout.Value Then
                        ClearSelection()
                        For Each row In FindRows(block.CaptureBlocks)
                            row.Selected = True
                        Next
                    End If
                Case EvaluationMode.Replace
                    SelectRows(block.Key)
                Case EvaluationMode.Split
                    SelectRows(block.Key)
            End Select
        End If


    End Sub

    Private Sub SelectRows(key As String)

        ClearSelection()
        For Each row In FindRows(key)
            row.Selected = True
        Next

    End Sub

    Private Sub SelectCell(key As String, columnName As String)

        Dim row = FindFirstRow(key)
        If row IsNot Nothing Then
            Dim cell = row.Cells(columnName)
            If SelectedCells.Count = 0 OrElse SelectedCells.Count > 1 OrElse SelectedCells(0).RowIndex <> cell.RowIndex OrElse SelectedCells(0).ColumnIndex <> cell.ColumnIndex Then
                ClearSelection()
                cell.Selected = True
            End If
        End If

    End Sub

    Private Function FindRows(key As String) As IEnumerable(Of DataGridViewRow)

        Dim columnIndex = IdColumnIndex
        If columnIndex <> -1 Then
            Return Rows _
                .Cast(Of DataGridViewRow)() _
                .Where(Function(f) String.Equals(key, f.Cells(columnIndex).Value.ToString()))
        End If
        Return Enumerable.Empty(Of DataGridViewRow)()

    End Function

    Private Function FindFirstRow(key As String) As DataGridViewRow

        Dim columnIndex = IdColumnIndex
        If columnIndex <> -1 Then
            Return Rows _
                .Cast(Of DataGridViewRow)() _
                .FirstOrDefault(Function(f) String.Equals(key, f.Cells(columnIndex).Value.ToString()))
        End If
        Return Nothing

    End Function

    Private Function FindRows(captureBlocks As IEnumerable(Of CaptureBlock)) As IEnumerable(Of DataGridViewRow)

        Dim columnIndex = IdColumnIndex
        If columnIndex <> -1 Then
            Return Rows _
                .Cast(Of DataGridViewRow)() _
                .Join(
                    captureBlocks,
                    Function(r) CInt(r.Cells(columnIndex).Value),
                    Function(b) b.CaptureId,
                    Function(r, b) r)
        End If
        Return Enumerable.Empty(Of DataGridViewRow)()

    End Function

    Public Overloads Sub Sort(sortDirection As ListSortDirection)

        If Columns.Contains(IdColumnName) Then
            Sort(Columns(IdColumnName), sortDirection)
        End If

    End Sub

    Public Sub EnsureDisplayed()

        If SelectedRows.Count > 1 Then
            Dim indexes = SelectedRows.Cast(Of DataGridViewRow).Select(Function(f) f.Index).OrderBy(Function(f) f).ToArray()
            EnsureRowDisplayed(indexes.Last())
            EnsureRowDisplayed(indexes.First())
        ElseIf SelectedRows.Count = 1 Then
            EnsureRowDisplayed(SelectedRows(0).Index)
        ElseIf SelectedCells.Count = 1 Then
            EnsureCellDisplayed(SelectedCells(0))
        End If

    End Sub

    Private Sub ResizeDisplayedRowsAndColumns()

        AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCells, False)
        AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells, True)

    End Sub

    Private Sub NoCaptureSymbolItem_Click(sender As Object, e As EventArgs)

        Dim tsi = DirectCast(sender, ToolStripMenuItem)
        UseNoCaptureSymbol = tsi.Checked

    End Sub

    Private Sub WrapTextItem_Click(sender As Object, e As EventArgs)

        Dim tsi = DirectCast(sender, ToolStripMenuItem)
        WrapText = tsi.Checked

    End Sub

    Private Sub CopyCellValueItem_Click(sender As Object, e As EventArgs)

        Dim tsi = DirectCast(sender, ToolStripMenuItem)
        Dim cell = DirectCast(tsi.Tag, DataGridViewCell)
        If cell.FormattedValue IsNot Nothing Then
            Dim s = cell.FormattedValue.ToString().EnsureCarriageReturnLinefeed()
            If String.IsNullOrEmpty(s) = False Then
                AppUtility.SetClipboardText(s)
            End If
        End If

    End Sub

    Protected Overrides Sub OnCellContextMenuStripNeeded(e As DataGridViewCellContextMenuStripNeededEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        e.ContextMenuStrip = New ContextMenuStrip()
        e.ContextMenuStrip.Items.AddRange(CreateToolStripItems(e).ToArray())
        MyBase.OnCellContextMenuStripNeeded(e)

    End Sub

    Private Iterator Function CreateToolStripItems(e As DataGridViewCellContextMenuStripNeededEventArgs) As IEnumerable(Of ToolStripItem)

        If e.ColumnIndex <> -1 Then
            If e.RowIndex <> -1 Then
                Dim cell = Item(e.ColumnIndex, e.RowIndex)
                If cell.FormattedValue IsNot Nothing AndAlso String.IsNullOrEmpty(cell.FormattedValue.ToString()) = False Then
                    Yield New ToolStripMenuItem(My.Resources.CopyCellValue, Nothing, AddressOf CopyCellValueItem_Click) With {.Tag = cell}
                End If
            Else
                Yield New ToolStripMenuItem(My.Resources.CopyUniqueValues, Nothing, Sub() CopyUniqueValues(e.ColumnIndex, StringComparer.CurrentCulture))
                Yield New ToolStripMenuItem(My.Resources.CopyUniqueValues & " " & My.Resources.IgnoreCase.AddParentheses(), Nothing, Sub() CopyUniqueValues(e.ColumnIndex, StringComparer.CurrentCultureIgnoreCase))
                Yield New ToolStripSeparator()
                Yield New ToolStripMenuItem(My.Resources.CopyColumn, Nothing, Sub() CopyColumn(Columns(e.ColumnIndex)))
            End If
        End If
        Yield New ToolStripMenuItem(My.Resources.CopyAll, Nothing, Sub(sender As Object, e2 As EventArgs) CopyAll())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.WrapText, Nothing, AddressOf WrapTextItem_Click) With {.Checked = WrapText, .CheckOnClick = True}
        Yield New ToolStripMenuItem(My.Resources.NoCaptureSymbol, Nothing, AddressOf NoCaptureSymbolItem_Click) With {.Checked = UseNoCaptureSymbol, .CheckOnClick = True}

    End Function

    Private Sub CopyUniqueValues(columnIndex As Integer, comparer As StringComparer)

        AppUtility.SetClipboardText(String.Join(vbCrLf, GetUniqueValues(columnIndex, comparer)))

    End Sub

    Private Function GetUniqueValues(columnIndex As Integer, comparer As StringComparer) As IEnumerable(Of String)

        Return Rows _
            .Cast(Of DataGridViewRow) _
            .Select(Function(f) f.Cells(columnIndex).Value) _
            .Where(Function(f) f IsNot Nothing) _
            .Select(Function(f) f.ToString()) _
            .Distinct(comparer)

    End Function

    Protected Overrides Sub OnCellFormatting(e As DataGridViewCellFormattingEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.Value Is DBNull.Value Then
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End If
        MyBase.OnCellFormatting(e)

    End Sub

    Protected Overrides Sub OnColumnAdded(e As DataGridViewColumnEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.Column.Name = IdColumnName Then
            e.Column.Frozen = True
        End If
        e.Column.DefaultCellStyle.Alignment = If(_builder.Settings.NumberAlignment = LeftRightAlignment.Right AndAlso e.Column.ValueType = GetType(Integer),
            DataGridViewContentAlignment.TopRight,
            DataGridViewContentAlignment.TopLeft)
        MyBase.OnColumnAdded(e)

    End Sub

    Protected Overrides Sub OnSelectionChanged(e As EventArgs)

        If _suppressSelectionChanged = False Then
            MyBase.OnSelectionChanged(e)
        End If

    End Sub

    Protected Overrides Sub OnColumnHeaderMouseClick(e As DataGridViewCellMouseEventArgs)

        CurrentCell = Nothing
        MyBase.OnColumnHeaderMouseClick(e)

    End Sub

    Protected Overrides Sub OnSorted(e As EventArgs)

        If SortedColumn IsNot Nothing AndAlso Rows.Count > 0 Then
            CurrentCell = Item(SortedColumn.Index, 0)
        End If
        MyBase.OnSorted(e)

    End Sub

    Protected Overrides Sub OnScroll(e As ScrollEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.ScrollOrientation = ScrollOrientation.VerticalScroll Then
            AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCells, True)
        End If
        MyBase.OnScroll(e)

    End Sub

    Public Sub CopyAll()

        SelectAll()
        CopySelection()

    End Sub

    Private Sub CopyColumn(column As DataGridViewColumn)

        Dim mode = SelectionMode
        _suppressSelectionChanged = True
        ClearSelection()
        CurrentCell = Nothing
        If SelectionMode = DataGridViewSelectionMode.FullColumnSelect OrElse SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect Then
            column.Selected = True
        Else
            If SelectionMode = DataGridViewSelectionMode.FullRowSelect OrElse SelectionMode = DataGridViewSelectionMode.RowHeaderSelect Then
                SelectionMode = DataGridViewSelectionMode.CellSelect
            End If
            For rowIndex As Integer = 0 To RowCount - 1
                Item(column.Index, rowIndex).Selected = True
            Next
        End If
        CopySelection()
        If mode = DataGridViewSelectionMode.FullRowSelect OrElse mode = DataGridViewSelectionMode.RowHeaderSelect Then
            SelectionMode = mode
        End If
        ClearSelection()
        CurrentCell = Nothing
        _suppressSelectionChanged = False

    End Sub

    Private Sub CopySelection()

        Dim mode = ClipboardCopyMode
        ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Try
            Clipboard.SetDataObject(GetClipboardContent())
        Catch ex As Exception
            If TypeOf ex Is System.Runtime.InteropServices.ExternalException OrElse TypeOf ex Is System.Threading.ThreadStateException Then
                MessageDialog.Warning(ex.Message)
            Else
                Throw
            End If
        End Try
        ClipboardCopyMode = mode

    End Sub

    Protected Overridable Sub OnMatchModeLayoutChanged(e As EventArgs)

        RaiseEvent MatchModeLayoutChanged(Me, e)

    End Sub

    Public ReadOnly Property SelectionKey As String
        Get
            Dim idColumnIndex = Me.IdColumnIndex
            If idColumnIndex <> -1 Then
                If SelectedRows.Count = 1 Then
                    Dim id = SelectedRows(0).Cells(idColumnIndex).Value
                    Dim matchBuilder = TryCast(_builder, MatchBuilder)
                    If matchBuilder IsNot Nothing AndAlso MatchModeLayout = TableLayout.Value Then
                        If Columns.Contains(Captions.DefaultMatchChar) Then
                            Dim matchNumber = SelectedRows(0).Cells(Captions.DefaultMatchChar).Value.ToString()
                            If Columns.Contains(Captions.DefaultGroupChar) AndAlso Columns.Contains(Captions.DefaultCaptureChar) Then
                                Dim groupName = SelectedRows(0).Cells(Captions.DefaultGroupChar).Value.ToString()
                                Dim captureNumber = SelectedRows(0).Cells(Captions.DefaultCaptureChar).Value.ToString()
                                If groupName <> "0" Then
                                    Return matchNumber & groupName & captureNumber
                                End If
                            End If
                            Return matchNumber
                        Else
                            Dim index = CInt(id)
                            If index < matchBuilder.Data.CaptureCount Then
                                Dim captureItem = matchBuilder.Data.Items.CaptureItems(index)
                                If captureItem.IsDefaultCapture = False Then
                                    Return captureItem.Key
                                End If
                                Return captureItem.MatchItem.Key
                            End If
                        End If
                    Else
                        Return id.ToString()
                    End If
                ElseIf SelectedCells.Count = 1 Then
                    Dim cell = SelectedCells(0)
                    Dim key = cell.OwningRow.Cells(idColumnIndex).Value.ToString()
                    Dim colName = cell.OwningColumn.Name
                    Dim builder = TryCast(_builder, MatchBuilder)
                    If builder IsNot Nothing AndAlso cell.OwningColumn.Index <> idColumnIndex AndAlso colName <> "0" Then
                        If builder.Data.GroupInfos.Contains(colName) Then
                            key &= colName
                        End If
                    End If
                    Return key
                End If
            End If
            Return Nothing
        End Get
    End Property

    Public Property WrapText As Boolean
        Get
            Return _wrapText
        End Get
        Set(value As Boolean)
            If _wrapText <> value Then
                _wrapText = value
                DefaultCellStyle.WrapMode = If(_wrapText, DataGridViewTriState.True, DataGridViewTriState.False)
                ResizeDisplayedRowsAndColumns()
            End If
        End Set
    End Property

    Public Property UseNoCaptureSymbol As Boolean
        Get
            Return _useNoCaptureSymbol
        End Get
        Set(value As Boolean)
            If _useNoCaptureSymbol <> value Then
                _useNoCaptureSymbol = value
                DefaultCellStyle.NullValue = If(_useNoCaptureSymbol,
                    If(_builder IsNot Nothing, _builder.Settings.Symbols.NoCapture, Symbols.DefaultNoCapture),
                    Nothing)
            End If
        End Set
    End Property

    Public ReadOnly Property Loaded As Boolean
        Get
            Return _loaded
        End Get
    End Property

    Public Property MatchModeLayout As TableLayout
        Get
            Return _layout
        End Get
        Set(value As TableLayout)
            If _layout <> value Then
                _layout = value
                Dim result = TryCast(_builder, MatchBuilder)
                If result IsNot Nothing Then
                    SelectionMode = If(MatchModeLayout = TableLayout.Group, DataGridViewSelectionMode.CellSelect, DataGridViewSelectionMode.FullRowSelect)
                    Load(If(_layout = TableLayout.Group, result.GetGroupTable(), result.GetTable()))
                End If
                OnMatchModeLayoutChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public ReadOnly Property IdColumnIndex As Integer
        Get
            If Columns.Contains(IdColumnName) Then
                Return Columns(IdColumnName).Index
            End If
            Return -1
        End Get
    End Property

    Private _layout As TableLayout
    Private _builder As RegexBuilder
    Private _loaded As Boolean
    Private _suppressSelectionChanged As Boolean
    Private _wrapText As Boolean
    Private _useNoCaptureSymbol As Boolean

    Private Const IdColumnName As String = Symbols.DefaultNumber

    Public Event MatchModeLayoutChanged As EventHandler

End Class
