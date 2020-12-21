Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports Regexator.Collections.Generic
Imports Regexator.FileSystem
Imports Regexator.IO
Imports Regexator.Text
Imports Regexator.Text.RegularExpressions
Imports Regexator.Windows.Forms

Public NotInheritable Class FileSystemSearchDataGridView
    Inherits DataGridView

    Private _searchPhrase As String = String.Empty
    Private _hideOnLostFocus As Boolean = True
    Private ReadOnly _directoryColumn As DataGridViewTextBoxColumn
    Private ReadOnly _fileNameColumn As DataGridViewTextBoxColumn

    Public Sub New()

        DoubleBuffered = True
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        StandardTab = False
        AllowUserToResizeRows = False
        EnableHeadersVisualStyles = False
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ShowCellToolTips = False
        RowHeadersVisible = False
        ColumnHeadersVisible = False
        BackgroundColor = Color.White
        CellBorderStyle = DataGridViewCellBorderStyle.None
        AllowUserToAddRows = False
        AllowUserToDeleteRows = False
        AutoGenerateColumns = False
        Me.ReadOnly = True
        MultiSelect = False

        _directoryColumn = New DataGridViewTextBoxColumn()
        _directoryColumn.DataPropertyName = "DirectoryName"
        _directoryColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        _fileNameColumn = New DataGridViewTextBoxColumn()
        _fileNameColumn.DataPropertyName = "Name"
        _fileNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader

        Columns.Add(_directoryColumn)
        Columns.Add(_fileNameColumn)

    End Sub

    Private Sub Load()

        BeginUpdate()
        If SearchPhrase.Length > 0 AndAlso RegexLibrary.InvalidFileNameChar.IsMatch(SearchPhrase) = False Then
            Dim items As HashSet(Of SearchResult) = Find()
            Dim lst As New List(Of SearchResult)(items.Count)
            lst.AddRange(items)
            Dim results = New SortableBindingList(Of SearchResult)(lst)
            If results.Count > 0 Then
                DataSource = results
                Sort(_fileNameColumn, ListSortDirection.Ascending)
            End If
        Else
            DataSource = Nothing
        End If
        EndUpdate()

    End Sub

    Private Function Find() As HashSet(Of SearchResult)

        Dim results As New HashSet(Of SearchResult)(New SearchResultEqualityComparer())

        Dim searcher As New FileSystemSearcher()
        searcher.FilePattern = "*.rgx"
        searcher.SearchOption = SearchOption.AllDirectories
        searcher.SearchMode = SearchMode.FileName
        searcher.FileNamePart = FileNamePart.NameWithoutExtension
        searcher.Regex = New Regex(RegexUtility.Escape(SearchPhrase), RegexOptions.IgnoreCase)
        searcher.RequireMatchForDeeperSearch = True

        For Each result As SearchResult In searcher.Find(Explorer.EnumerateRootNodes().Select(Function(f) f.FullName))
            results.Add(result)
        Next

        For Each recentItem As RecentItem In Explorer.RecentManager.EnumerateItems()
            If recentItem.Kind = ItemKind.Directory Then
                For Each item As SearchResult In searcher.Find(recentItem.FullName)
                    results.Add(item)
                Next
            ElseIf recentItem.Kind = ItemKind.Project Then
                For Each item As SearchResult In searcher.Find(recentItem.FullName)
                    results.Add(item)
                Next
                If searcher.IsMatch(recentItem.FullName) Then
                    results.Add(New SearchResult(recentItem.FullName))
                End If
            End If
        Next

        Return results

    End Function

    Public Sub ShowGrid(location As Point)

        Dim index As Integer = App.MainForm.Controls.IndexOf(Me)
        If index = -1 Then
            App.MainForm.Controls.Add(Me)
            RowTemplate.Height = DefaultCellStyle.Font.Height + 2
        End If

        Me.Location = location
        Visible = True
        BringToFront()

    End Sub

    Public Sub HideGrid()

        Visible = False
        Dim index As Integer = App.MainForm.Controls.IndexOf(Me)
        If index <> -1 Then
            App.MainForm.Controls.RemoveAt(index)
        End If

        SearchPhrase = String.Empty

    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)

        If HideOnLostFocus Then
            HideGrid()
        End If

        MyBase.OnLostFocus(e)

    End Sub

    Public Property HideOnLostFocus() As Boolean
        Get
            Return _hideOnLostFocus
        End Get
        Set(ByVal value As Boolean)
            _hideOnLostFocus = value
        End Set
    End Property

    Public Property SearchPhrase As String
        Get
            Return _searchPhrase
        End Get
        Set(value As String)
            If _searchPhrase <> value Then
                _searchPhrase = value
                Load()
            End If
        End Set
    End Property

    Protected Overrides ReadOnly Property ShowFocusCues As Boolean
        Get
            Return False
        End Get
    End Property

End Class
