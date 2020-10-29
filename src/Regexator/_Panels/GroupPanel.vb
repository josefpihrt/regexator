Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports Regexator
Imports Regexator.UI
Imports Regexator.Text.RegularExpressions
Imports Regexator.Collections.Generic
Imports Regexator.Windows.Forms

Public NotInheritable Class GroupPanel
    Implements IDisposable

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Public Sub New()

        _dgv = New GroupDataGridView() With {.BorderStyle = BorderStyle.None, .ContextMenuStrip = New ContextMenuStrip()}
        _tsp = New AppToolStrip()
        _pnlGrid = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}

        _btnUndo = New ToolStripButton(Nothing, My.Resources.IcoArrowLeft.ToBitmap(), Sub() _history.Undo()) With {.Enabled = False, .ToolTipText = My.Resources.Undo & " " & My.Resources.CtrlZ.AddParentheses()}
        _btnRedo = New ToolStripButton(Nothing, My.Resources.IcoArrowRight.ToBitmap(), Sub() _history.Redo()) With {.Enabled = False, .ToolTipText = My.Resources.Redo & " " & My.Resources.CtrlY.AddParentheses()}
        _btnSetAll = New ToolStripButton(Nothing, My.Resources.IcoCheckAll.ToBitmap(), Sub() SetAll()) With {.ToolTipText = My.Resources.SetAll}

        _dgv.ContextMenuStrip = CreateContextMenuStrip()
        _dgv.DataSource = _items

        AddHandler _dgv.ContextMenuStrip.Opening, Sub(sender As Object, e As CancelEventArgs) e.Cancel = (Mode <> EvaluationMode.Match)
        AddHandler _dgv.Sorted, AddressOf DataGridView_Sorted
        AddHandler _dgv.CellValueChanged, AddressOf DataGridView_CellValueChanged
        AddHandler _dgv.CellContextMenuStripNeeded, AddressOf DataGridView_CellContextMenuStripNeeded
        AddHandler _dgv.KeyDown, AddressOf DataGridView_KeyDown
        AddHandler _dgv.CellMouseDoubleClick, AddressOf DataGridView_CellMouseDoubleClick
        AddHandler _items.ListChanged, AddressOf Items_ListChanged
        AddHandler _history.CanUndoChanged, Sub() _btnUndo.Enabled = _history.CanUndo
        AddHandler _history.CanRedoChanged, Sub() _btnRedo.Enabled = _history.CanRedo

        _tsp.Items.AddRange({_btnUndo, _btnRedo, New ToolStripSeparator(), _btnSetAll})
        _pnlGrid.Controls.Add(_dgv)
        _pnl.Controls.AddRange({_pnlGrid, _tsp})

    End Sub

    Public Sub ToggleGroupEnabled(groupIndex As Integer)

        If groupIndex < 0 Then Throw New ArgumentOutOfRangeException("groupIndex")
        Dim info = _dgv.FindGroupInfo(groupIndex)
        If info IsNot Nothing Then
            info.ToggleEnabled()
            OnEnabledGroupsChanged()
        End If

    End Sub

    Private Sub Items_ListChanged(sender As Object, e As ListChangedEventArgs)

        If e.ListChangedType = ListChangedType.ItemChanged AndAlso e.PropertyDescriptor.Name = "Enabled" Then
            Dim item = _items(e.NewIndex)
            If item.Enabled Then
                _ignoredGroups.Remove(item.Name)
            Else
                _ignoredGroups.Add(item.Name)
            End If
        End If

    End Sub

    Public Sub Reset(ignoredGroups As String())

        If ignoredGroups Is Nothing Then Throw New ArgumentNullException("ignoredGroups")
        _prevSortProperty = GroupSettings.DefaultSortProperty
        _prevSortDirection = GroupSettings.DefaultSortDirection
        _ignoredGroups = New SortedSet(Of String)(ignoredGroups)
        _history.Clear()

    End Sub

    Private Sub SetAll(sender As Object, e As EventArgs)

        SetAll()

    End Sub

    Private Sub SetAll()

        SetEnabled(_items.Select(Function(f) f.Name).ToArray())

    End Sub

    Private Sub SetSelectedOnly(sender As Object, e As EventArgs)

        SetEnabled(New String() {DirectCast(sender, ToolStripMenuItem).Tag.ToString()})

    End Sub

    Private Sub SetAllButSelected(sender As Object, e As EventArgs)

        SetEnabled(_items _
            .Select(Function(f) f.Name) _
            .Except(New String() {DirectCast(sender, ToolStripMenuItem).Tag.ToString()}) _
            .ToArray())

    End Sub

    Private Sub SetEnabled(names As String())

        If _prevEnabledGroups.SequenceEqual(names) = False Then
            For Each item In _items
                item.Enabled = names.Contains(item.Name)
            Next
            OnEnabledGroupsChanged()
        End If

    End Sub

    Private Sub DataGridView_Sorted(sender As Object, e As EventArgs)

        Dim sortDirection = Me.SortDirection
        Dim sortPropertyName = Me.SortPropertyName
        If _raiseSorted Then
            OnEnabledGroupsChanged(sortPropertyName, sortDirection)
        End If
        _prevSortDirection = sortDirection
        _prevSortProperty = sortPropertyName

    End Sub

    Private Sub DataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)

        If e.ColumnIndex <> -1 AndAlso _dgv.Columns(e.ColumnIndex).Name = "Enabled" Then
            OnEnabledGroupsChanged()
        End If

    End Sub

    Private Sub DataGridView_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Space Then
                Dim items = _dgv.SelectedGroupInfos().ToArray()
                If items.Any() Then
                    If items.All(Function(f) f.Enabled) Then
                        For Each item In items
                            item.Enabled = False
                        Next
                    Else
                        For Each item In items
                            item.Enabled = True
                        Next
                    End If
                    OnEnabledGroupsChanged()
                End If
            End If
        ElseIf e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.Z Then
                _history.UndoIfCan()
            ElseIf e.KeyCode = Keys.Y Then
                _history.RedoIfCan()
            End If
        End If

    End Sub

    Private Sub DataGridView_CellContextMenuStripNeeded(sender As Object, e As DataGridViewCellContextMenuStripNeededEventArgs)

        If App.Mode = EvaluationMode.Match Then
            Dim cms = New ContextMenuStrip()
            cms.Items.Add(New ToolStripMenuItem(My.Resources.SetAll, My.Resources.IcoCheckAll.ToBitmap(), AddressOf SetAll))
            If e.RowIndex <> -1 Then
                Dim item = DirectCast(_dgv.Rows(e.RowIndex).DataBoundItem, GroupInfoItem)
                cms.Items.Add(New ToolStripSeparator())
                cms.Items.Add(New ToolStripMenuItem(My.Resources.SetThisOnly, Nothing, AddressOf SetSelectedOnly) With {.Tag = item.Name})
                cms.Items.Add(New ToolStripMenuItem(My.Resources.SetAllButThis, Nothing, AddressOf SetAllButSelected) With {.Tag = item.Name})
            End If
            e.ContextMenuStrip = cms
        End If

    End Sub

    Private Sub DataGridView_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs)

        If e.RowIndex <> -1 AndAlso e.ColumnIndex <> -1 AndAlso (TypeOf _dgv.Columns(e.ColumnIndex) Is DataGridViewCheckBoxColumn) = False Then
            Dim item = DirectCast(_dgv.Rows(e.RowIndex).DataBoundItem, GroupInfoItem)
            item.ToggleEnabled()
            OnEnabledGroupsChanged()
        End If

    End Sub

    Public Sub Load()

        Load(New GroupInfo() {})

    End Sub

    Public Sub Load(items As GroupInfo())

        If items Is Nothing Then Throw New ArgumentNullException("items")
        _items.Clear()
        For Each item In items.Select(Function(f) New GroupInfoItem(f))
            _items.Add(item)
            item.Enabled = Not _ignoredGroups.Contains(item.Name)
        Next
        _prevEnabledGroups = GetEnabledGroups()
        Sort(_prevSortProperty, _prevSortDirection, False)
        _history.Clear()
        AddCommand(_prevSortProperty, _prevSortDirection, _prevEnabledGroups)

    End Sub

    Private Sub OnEnabledGroupsChanged()

        OnEnabledGroupsChanged(_prevSortProperty, _prevSortDirection)

    End Sub

    Private Sub OnEnabledGroupsChanged(sortProperty As GroupSortProperty, sortDirection As ListSortDirection)

        Dim enabledGroups = GetEnabledGroups()
        AddCommand(sortProperty, sortDirection, enabledGroups)
        _prevEnabledGroups = enabledGroups
        If App.Mode = EvaluationMode.Match Then
            Panels.Output.LoadData()
        End If

    End Sub

    Private Sub AddCommand(sortProperty As GroupSortProperty, sortDirection As ListSortDirection, enabledGroups As String())

        If _history.IsExecuting = False Then
            Dim command = New Command(
                Sub()
                    Sort(sortProperty, sortDirection, True)
                    SetEnabled(enabledGroups)
                End Sub,
                String.Format(CultureInfo.CurrentCulture, "Sort Property: {0}, Sort Direction: {1}, Enabled Groups: {2}", sortProperty, sortDirection, String.Join(", ", enabledGroups)))
            _history.AddCommand(command)
        End If

    End Sub

    Private Sub Sort(sortProperty As GroupSortProperty, sortDirection As ListSortDirection, raiseSorted As Boolean)

        If _dgv.Columns.Contains(sortProperty.ToString()) Then
            _raiseSorted = raiseSorted
            _dgv.Sort(_dgv.Columns(sortProperty.ToString()), sortDirection)
            _raiseSorted = True
        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        Dispose(True)
        GC.SuppressFinalize(Me)

    End Sub

    Private Sub Dispose(disposing As Boolean)

        If _disposed = False Then
            If disposing Then
                _dgv.Dispose()
                _dgv = Nothing
            End If
        End If
        _disposed = True

    End Sub

    Public Iterator Function CreateToolStripItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.SetAll, Nothing, AddressOf SetAll)
        Dim tseToggle = New ToolStripSeparator()
        Dim tsiToggle = New ToolStripMenuItem(My.Resources.ToggleGroup, Nothing, New ToolStripSeparator())
        Dim action =
            Sub()
                Dim flg As Boolean = _items.Count > 0
                tseToggle.Visible = flg
                tsiToggle.Visible = flg
                If flg AndAlso tsiToggle.DropDownItems.Count = 0 Then
                    tsiToggle.DropDownItems.Add(New ToolStripSeparator())
                End If
            End Sub
        action()
        AddHandler tsiToggle.DropDownOpening, Sub() tsiToggle.DropDownItems.LoadItems(CreateToggleToolStripItems())
        AddHandler parent.DropDownOpening, Sub() action()
        Yield tseToggle
        Yield tsiToggle

    End Function

    Public Function CreateToggleToolStripItems() As IEnumerable(Of ToolStripMenuItem)

        Return _items _
            .Where(Function(f) f.Index <= 9) _
            .OrderBy(Function(f) f.Index) _
            .Select(Function(f) CreateToggleToolStripItem(f))

    End Function

    Public Function CreateToggleToolStripItem(info As GroupInfoItem) As ToolStripMenuItem

        If info Is Nothing Then Throw New ArgumentNullException("info")
        Return New ToolStripMenuItem(info.Name, Nothing,
            Sub()
                info.ToggleEnabled()
                OnEnabledGroupsChanged()
            End Sub) With {.Checked = info.Enabled, .ShortcutKeyDisplayString = GetShortcutKeyDisplayString(info)}

    End Function

    Private Shared Function GetShortcutKeyDisplayString(info As GroupInfo) As String

        If info.Index < 10 Then
            Debug.Assert(My.Resources.ResourceManager.GetString("Ctrl" & info.Index.ToString(CultureInfo.InvariantCulture)) IsNot Nothing)
            Return My.Resources.ResourceManager.GetString("Ctrl" & info.Index.ToString(CultureInfo.InvariantCulture))
        End If
        Return String.Empty

    End Function

    Private Function CreateContextMenuStrip() As ContextMenuStrip

        Dim cms As New ContextMenuStrip()
        cms.Items.Add(New ToolStripMenuItem(My.Resources.SetAll, My.Resources.IcoCheckAll.ToBitmap(), AddressOf SetAll))
        Return cms

    End Function

    Private Function GetEnabledGroups() As String()

        Return _items.Where(Function(f) f.Enabled).Select(Function(f) f.Name).ToArray()

    End Function

    Public Function GetIgnoredGroups() As String()

        Return _items.Where(Function(f) f.Enabled = False).Select(Function(f) f.Name).ToArray()

    End Function

    Public ReadOnly Property GroupNames As IEnumerable(Of String)
        Get
            Return _items.Select(Function(f) f.Name)
        End Get
    End Property

    Public ReadOnly Property IsAnyEnabled() As Boolean
        Get
            Return _items.Any(Function(f) f.Enabled)
        End Get
    End Property

    Public ReadOnly Property IsZeroEnabled As Boolean
        Get
            Dim info = _items.FirstOrDefault(Function(f) f.Index = 0)
            Return If(info IsNot Nothing, info.Enabled, False)
        End Get
    End Property

    Public ReadOnly Property SortPropertyName As GroupSortProperty
        Get
            Dim result As GroupSortProperty
            If _dgv.SortedColumn IsNot Nothing AndAlso [Enum].TryParse(_dgv.SortedColumn.Name, result) Then
                Return result
            End If
            Return GroupSettings.DefaultSortProperty
        End Get
    End Property

    Public ReadOnly Property SortDirection As ListSortDirection
        Get
            Return If(_dgv.SortOrder = SortOrder.Descending, ListSortDirection.Descending, ListSortDirection.Ascending)
        End Get
    End Property

    Public ReadOnly Property Sorter As GroupInfoSorter
        Get
            Return New GroupInfoSorter(SortPropertyName, SortDirection)
        End Get
    End Property

    Public ReadOnly Property History As CommandHistory
        Get
            Return _history
        End Get
    End Property

    Public ReadOnly Property GroupSettings As GroupSettings
        Get
            Return New GroupSettings(SortPropertyName, SortDirection, GetIgnoredGroups())
        End Get
    End Property

    Friend _dgv As GroupDataGridView
    Friend _tsp As AppToolStrip
    Friend _pnlGrid As Panel
    Friend _pnl As Panel
    Friend _btnUndo As ToolStripButton
    Friend _btnRedo As ToolStripButton
    Friend _btnSetAll As ToolStripButton

    Private ReadOnly _items As New SortableBindingList(Of GroupInfoItem)
    Private ReadOnly _history As New CommandHistory
    Private _ignoredGroups As New SortedSet(Of String)
    Private _prevEnabledGroups As String() = New String() {}
    Private _prevSortProperty As GroupSortProperty = GroupSettings.DefaultSortProperty
    Private _prevSortDirection As ListSortDirection = GroupSettings.DefaultSortDirection
    Private _disposed As Boolean
    Private _raiseSorted As Boolean

End Class
