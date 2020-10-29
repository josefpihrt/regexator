Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports Regexator.Collections.Generic
Imports Regexator.IO

Public Class FileSystemSearchResultsPanel

    Friend _dgv As FileSystemSearchResultsDataGridView
    Friend _pnl As Panel
    Friend _tsp As AppToolStrip
    Friend _btnStartSearch As ToolStripButton
    Friend _btnStopSearch As ToolStripButton
    Friend _btnSearchDirectoryName As ToolStripButton
    Friend _btnSearchFileName As ToolStripButton
    Friend _btnSearchSubdirectories As ToolStripButton
    Friend _btnFileName As ToolStripButton
    Friend _btnFileNameWithoutExtension As ToolStripButton
    Friend _btnExtension As ToolStripButton
    Friend _btnColumnsAutoWidth As ToolStripButton
    Friend _lblInfo As ToolStripLabel

    Private _worker As BackgroundWorker
    Private _cancellationTokenSource As CancellationTokenSource
    Private _results As SortableBindingList(Of SearchResult)
    Private _fileNamePart As FileNamePart
    Private _searchOption As SearchOption
    Private _searchMode As SearchMode
    Private ReadOnly _sorter As SearchResultSorter = New SearchResultSorter()

    Public Sub New()

        _results = New SortableBindingList(Of SearchResult)()

        _dgv = New FileSystemSearchResultsDataGridView() With {.BorderStyle = BorderStyle.None}
        _tsp = New AppToolStrip() With {.FirstItemHasLeftMargin = True}

        _btnStartSearch = New ToolStripButton(Nothing, My.Resources.IcoMediaPlay.ToBitmap(), Sub() ExecuteSearch()) With {.ToolTipText = My.Resources.Search + " (" + My.Resources.F5 + ")"}
        _btnStopSearch = New ToolStripButton(Nothing, My.Resources.IcoMediaStop.ToBitmap(), Sub() _cancellationTokenSource.Cancel()) With {.ToolTipText = My.Resources.StopSearch, .Enabled = False}

        _btnSearchDirectoryName = New ToolStripButton(Nothing, My.Resources.IcoFolder.ToBitmap(), Sub() SearchMode = SearchMode.DirectoryName) With {.CheckOnClick = True, .ToolTipText = My.Resources.SearchDirectories}
        _btnSearchFileName = New ToolStripButton(Nothing, My.Resources.IcoFile.ToBitmap(), Sub() SearchMode = SearchMode.FileName) With {.CheckOnClick = True, .Checked = True, .ToolTipText = My.Resources.SearchFiles}

        _btnSearchSubdirectories = New ToolStripButton(Nothing, My.Resources.IcoTree.ToBitmap(), Sub() ToggleSearchOption()) With {.CheckOnClick = True, .ToolTipText = My.Resources.SearchSubdirectories}
        _btnFileName = New ToolStripButton(Nothing, My.Resources.IcoFileName.ToBitmap(), Sub() FileNamePart = FileNamePart.NameAndExtension) With {.CheckOnClick = True, .ToolTipText = My.Resources.SearchFileNameAndExtension}
        _btnFileNameWithoutExtension = New ToolStripButton(Nothing, My.Resources.IcoFileNameWithoutExtension.ToBitmap(), Sub() FileNamePart = FileNamePart.NameWithoutExtension) With {.CheckOnClick = True, .ToolTipText = My.Resources.SearchFileNameWithoutExtension}
        _btnExtension = New ToolStripButton(Nothing, My.Resources.IcoExtension.ToBitmap(), Sub() FileNamePart = FileNamePart.Extension) With {.CheckOnClick = True, .ToolTipText = My.Resources.SearchExtension}
        _btnColumnsAutoWidth = New ToolStripButton(Nothing, My.Resources.IcoWidth.ToBitmap(), Sub() _dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)) With {.ToolTipText = My.Resources.FitColumnsWidthToContent}
        _lblInfo = New ToolStripLabel() With {.Alignment = ToolStripItemAlignment.Right}

        _fileNamePart = FileNamePart.NameAndExtension
        _btnFileName.Checked = True
        FileNamePart = FileNamePart.NameAndExtension

        _searchOption = SearchOption.AllDirectories
        _btnSearchSubdirectories.Checked = True
        SearchOption = SearchOption.AllDirectories

        SearchMode = SearchMode.FileName

        _tsp.Items.AddRange(EnumerateItems().ToArray())

        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _pnl.Controls.Add(_dgv)

    End Sub

    Public Sub Load(container As ProjectContainer)

        FileSystemSearchInfo = If(container IsNot Nothing, container.FileSystemSearchInfo, New FileSystemSearchInfo())

        ClearData()

    End Sub

    Private Sub InitializeWorker()

        _worker = New BackgroundWorker() With {.WorkerReportsProgress = True, .WorkerSupportsCancellation = True}

        AddHandler _worker.DoWork,
            Sub(sender As Object, e As DoWorkEventArgs)
                Debug.WriteLine("worker DoWork start")
                e.Cancel = Not FindAll(DirectCast(e.Argument, FileSystemSearchContainer))
                Debug.WriteLine("worker DoWork end")
            End Sub

        AddHandler _worker.ProgressChanged,
            Sub(sender As Object, e As ProgressChangedEventArgs)
                _results.Add(DirectCast(e.UserState, SearchResult))
                If _results.Count <= 10 Then
                    _dgv.ResizeDisplayedRowsAndColumns()
                End If
                _lblInfo.Text = _results.Count.ToString(CultureInfo.CurrentCulture)
            End Sub

        AddHandler _worker.RunWorkerCompleted,
            Sub(sender As Object, e As RunWorkerCompletedEventArgs)
                If e.Cancelled Then
                    Debug.WriteLine("worker was cancelled")
                End If
                Debug.Assert(_results IsNot Nothing)
                If _results.Count = 0 Then
                    _dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells)
                End If
                _lblInfo.Text = _results.Count.ToString(CultureInfo.CurrentCulture)
                _btnStartSearch.Enabled = True
                _btnStopSearch.Enabled = False
                Debug.WriteLine("worker RunWorkerCompleted end")
            End Sub

    End Sub

    Private Function FindAll(container As FileSystemSearchContainer) As Boolean

        Dim maxCount = My.Settings.FileSystemSearchMaxCount
        Dim cnt As Integer = 0

        Try
            For Each result As SearchResult In container.Searcher.Find(container.Directories)

                cnt += 1
                _worker.ReportProgress(0, result)

                If cnt = maxCount Then
                    Exit For
                End If

            Next
        Catch ex As InvalidOperationException
            Return False
        End Try

        Return True

    End Function

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _btnStartSearch
        Yield _btnStopSearch
        Yield New ToolStripSeparator()
        Yield _btnSearchFileName
        Yield _btnSearchDirectoryName
        Yield New ToolStripSeparator()
        Yield _btnSearchSubdirectories
        Yield New ToolStripSeparator()
        Yield _btnFileName
        Yield _btnFileNameWithoutExtension
        Yield _btnExtension
        Yield New ToolStripSeparator()
        Yield _btnColumnsAutoWidth
        Yield _lblInfo

    End Function

    Public Sub ClearData()

        _results = New SortableBindingList(Of SearchResult)()
        _lblInfo.Text = ""

    End Sub

    Public Sub ExecuteSearch()

        If _worker Is Nothing OrElse _worker.IsBusy = False Then
            Dim regexInfo = Panels.Pattern.RegexInfo
            If regexInfo IsNot Nothing Then

                If _worker Is Nothing Then
                    InitializeWorker()
                End If

                _cancellationTokenSource = New CancellationTokenSource()

                Dim searcher = New FileSystemSearcher(_cancellationTokenSource.Token)
                searcher.Regex = regexInfo.Regex
                searcher.FileNamePart = FileNamePart
                searcher.SearchOption = SearchOption
                searcher.Replacement = Panels.Replacement.CurrentText
                searcher.ReplacementMode = Panels.Replacement.ReplacementMode
                searcher.SearchMode = SearchMode

                ClearData()

                _dgv.DataSource = _results

                Dim directories = Panels.Input._rtb.EnumerateDirectories().ToArray()

                If directories.Length = 0 Then
                    MessageDialog.Warning("No directory paths were found in the input panel." + vbLf + vbLf + String.Format("(You can can specify multiple paths on a single line using '{0}' delimiter.)", Path.PathSeparator))
                Else
                    _btnStartSearch.Enabled = False
                    _btnStopSearch.Enabled = True
                    _worker.RunWorkerAsync(New FileSystemSearchContainer() With {.Searcher = searcher, .Directories = directories})
                End If

            End If
        End If

    End Sub

    Public Property FileSystemSearchInfo As FileSystemSearchInfo
        Get
            Return New FileSystemSearchInfo(SearchMode, SearchOption, FileNamePart)
        End Get
        Set(value As FileSystemSearchInfo)
            SearchMode = value.SearchMode
            SearchOption = value.SearchOption
            FileNamePart = value.FileNamePart
        End Set
    End Property

    Public Property FileNamePart As FileNamePart
        Get
            Return _fileNamePart
        End Get
        Set(value As FileNamePart)
            If _fileNamePart <> value Then
                _fileNamePart = value
                _btnFileName.Checked = (value = FileNamePart.NameAndExtension)
                _btnFileNameWithoutExtension.Checked = (value = FileNamePart.NameWithoutExtension)
                _btnExtension.Checked = (value = FileNamePart.Extension)
            End If
        End Set
    End Property

    Public Property SearchOption As SearchOption
        Get
            Return _searchOption
        End Get
        Set(value As SearchOption)
            If _searchOption <> value Then
                _searchOption = value
                _btnSearchSubdirectories.Checked = (value = System.IO.SearchOption.AllDirectories)
            End If
        End Set
    End Property

    Public Property SearchMode As SearchMode
        Get
            Return _searchMode
        End Get
        Set(value As SearchMode)
            If _searchMode <> value Then
                _searchMode = value
                _btnSearchDirectoryName.Checked = (value = IO.SearchMode.DirectoryName)
                _btnSearchFileName.Checked = (value = IO.SearchMode.FileName)

                _btnFileName.Enabled = (SearchMode = IO.SearchMode.FileName)
                _btnFileNameWithoutExtension.Enabled = (SearchMode = IO.SearchMode.FileName)
                _btnExtension.Enabled = (SearchMode = IO.SearchMode.FileName)
            End If
        End Set
    End Property

    Private Sub ToggleSearchOption()

        If SearchOption = System.IO.SearchOption.AllDirectories Then
            SearchOption = SearchOption.TopDirectoryOnly
        Else
            SearchOption = SearchOption.AllDirectories
        End If

    End Sub

End Class