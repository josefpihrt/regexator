Imports System.Diagnostics.CodeAnalysis
Imports System.IO
Imports System.Text.RegularExpressions
Imports Regexator.IO
Imports Regexator.FileSystem
Imports Regexator.Windows.Forms

<SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
Public Class ExplorerPanel
    Inherits FileSystemManager

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Public Sub New()

        _trv = TreeView
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}
        _lblPath = New ToolStripStatusLabel() With {.Spring = True, .TextAlign = ContentAlignment.MiddleLeft, .AutoToolTip = True}
        _tsp = New AppToolStrip()
        _sts = New StatusStrip() With {.SizingGrip = False, .BackColor = SystemColors.Control, .ShowItemToolTips = True, .Renderer = New ExplorerStatusStripRenderer()}
        _btnReload = New ToolStripButton(Nothing, My.Resources.IcoReload.ToBitmap(), Sub() ReloadDirectory()) With {.ToolTipText = My.Resources.Refresh}
        _btnCollapseAll = New ToolStripButton(Nothing, My.Resources.IcoCollapseAll.ToBitmap(), Sub() _trv.CollapseAll(True)) With {.ToolTipText = My.Resources.CollapseAll}

        ShowHidden = My.Settings.FileSystemShowHidden
        UseRecycleBin = My.Settings.FileSystemUseRecycleBin
        ConfirmFileInputRemoval = My.Settings.ConfirmFileInputRemoval
        RecentManager.AddRange(AppSettings.RecentProjects)
        RecentManager.AddRange(AppSettings.RecentDirectories)

        AddHandler _trv.KeyDown, AddressOf TreeView_KeyDown
        AddHandler SaveManager.SaveRequested, AddressOf SaveManager_SaveRequested
        AddHandler History.CanUndoChanged, Sub() App.MainForm._tspMain._tssUndo.Enabled = History.CanUndo
        AddHandler History.CanRedoChanged, Sub() App.MainForm._tspMain._tssRedo.Enabled = History.CanRedo

        _tsp.Items.AddRange(EnumerateToolStripItems().ToArray())
        _sts.Items.Add(_lblPath)
        _pnl.Controls.Add(_trv)
        _pnl.Controls.AddRange({_tsp, _sts})

    End Sub

    Private Iterator Function EnumerateToolStripItems() As IEnumerable(Of ToolStripItem)

        Yield _btnReload
        Yield _btnCollapseAll

    End Function

    Protected Overrides Sub OnCurrentProjectChanged(e As EventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim flg = (CurrentProjectNode IsNot Nothing)
        If flg = False Then
            Unload()
        End If
        MyBase.OnCurrentProjectChanged(e)

    End Sub

    Protected Overrides Sub OnCurrentInputChanged(e As EventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim flg = (CurrentInputNode IsNot Nothing)
        If flg = False Then
            Panels.Input.Unload()
        End If
        MyBase.OnCurrentInputChanged(e)

    End Sub

    Protected Overrides Sub OnRootNodeChanged(e As TreeViewEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")

        Unload()
        Dim node = TryCast(e.Node, FileSystemNode)
        _lblPath.Text = If(node IsNot Nothing, node.FullName, String.Empty)
        MyBase.OnRootNodeChanged(e)

    End Sub

    Protected Overrides Sub Open(container As ProjectContainer, input As Input, mode As OpenMode)

        App.MainForm.Refresh()

        Using New RedrawDisabler(App.MainForm)
            Using Panels.Output.OutputEnabledRestorer()
                Select Case mode
                    Case OpenMode.InputOnly
                        Panels.Input.Load(input)
                        Panels.Input.GoToCursor()
                    Case OpenMode.None
                        LoadContainer(container)
                        Panels.Input.Load(input)
                        Panels.Pattern.GoToCursor()
                End Select
            End Using
        End Using

        MyBase.Open(container, input, mode)

    End Sub

    Public Sub InitialLoad(appExitSuccess As Boolean)

        If File.Exists(InitialFilePath) Then
            LoadProject(InitialFilePath)
        ElseIf appExitSuccess Then
            Dim dirPath = My.Settings.LastDirectoryPath
            If String.IsNullOrEmpty(dirPath) = False Then
                If Directory.Exists(dirPath) Then
                    LoadDirectory(dirPath)
                End If
            Else
                Dim projectPath = My.Settings.LastProjectPath
                If String.IsNullOrEmpty(projectPath) = False Then
                    If File.Exists(projectPath) Then
                        LoadProject(projectPath)
                    Else
                        Dim dirPath2 As String = Nothing
                        Try
                            dirPath2 = Path.GetDirectoryName(projectPath)
                        Catch ex As ArgumentException
                        Catch ex As PathTooLongException
                        End Try
                        If Directory.Exists(dirPath2) Then
                            LoadDirectory(dirPath2)
                        End If
                    End If
                End If
            End If
        End If

    End Sub

    Public Sub SavePattern()

        SaveManager.Save(SaveMode.Pattern, False)

    End Sub

    Public Sub SaveProject()

        SaveManager.Save(SaveMode.Project, False)

    End Sub

    Public Sub SaveProjectAs()

        Debug.Assert(CurrentProjectNode IsNot Nothing)
        LoadNewProject(If(CurrentProjectNode IsNot Nothing, App.ProjectContainer, DefaultProjectContainer),
            If(CurrentInputNode IsNot Nothing, Panels.Input.Input, Nothing))

    End Sub

    Public Sub SaveInput()

        SaveManager.Save(SaveMode.Input, False)

    End Sub

    Public Sub SaveInputText()

        SaveManager.Save(SaveMode.InputText, False)

    End Sub

    Public Sub SaveInputAs(kind As InputKind)

        Dim projectNode = CurrentProjectNode
        If projectNode IsNot Nothing Then
            Dim name As String = Nothing
            Dim fileNode = TryCast(CurrentInputNode, FileInputNode)
            If fileNode IsNot Nothing Then
                name = If(kind = InputKind.Included, fileNode.FileNameWithoutExtension, fileNode.FileName)
            Else
                Dim inputNode = TryCast(CurrentInputNode, InputNode)
                If inputNode IsNot Nothing Then
                    name = inputNode.Input.Name
                End If
            End If
            Dim input = Panels.Input.Input
            input.Kind = kind
            LoadNewInput(projectNode, input, name)
        End If

    End Sub

    Public Sub SaveReplacement()

        SaveManager.Save(SaveMode.Replacement, False)

    End Sub

    Public Sub SaveProjectInfo()

        SaveManager.Save(SaveMode.ProjectInfo, False)

    End Sub

    Public Sub SaveAll()

        SaveManager.Save(False)

    End Sub

    Private Shared Sub Unload()

        Using Panels.Output.OutputEnabledRestorer()
            Panels.Input.Unload()
            UnloadContainer()
        End Using

    End Sub

    Private Shared Sub UnloadContainer()

        LoadContainer(Nothing)

    End Sub

    Private Shared Sub LoadContainer(container As ProjectContainer)

        Mode = If(container IsNot Nothing, container.Mode, EvaluationMode.Match)
        App.RegexOptionsManager.ClearHistory()
        App.RegexOptionsManager.Value = If(container IsNot Nothing, container.Pattern.RegexOptions, RegexOptions.None)
        Panels.Output.Options = If(container IsNot Nothing, container.OutputInfo.Options, OutputOptions.None)
        Panels.Output.ResetBlock()
        Groups.Reset(If(container IsNot Nothing, container.OutputInfo.Groups.IgnoredGroups.ToArray(), New String() {}))
        Panels.Replacement.Load(container)
        FileSystemSearchResults.Load(container)
        Info.Load(container)
        Panels.Pattern.Load(container)

    End Sub

    Public Sub LoadRecentProjectItems(items As ToolStripItemCollection)

        If items Is Nothing Then Throw New ArgumentNullException("items")
        items.LoadItems(RecentManager.CreateToolStripMenuItems(ItemKind.Project, True).Take(My.Settings.RecentItemsMaxCount))
        If items.Count > 0 Then
            items.Add(New ToolStripMenuItem(My.Resources.ClearRecentProjectList, My.Resources.IcoDelete.ToBitmap(), Sub() RecentManager.Clear(ItemKind.Project)))
        End If

    End Sub

    Public Sub LoadRecentDirectoryItems(items As ToolStripItemCollection)

        If items Is Nothing Then Throw New ArgumentNullException("items")
        items.LoadItems(RecentManager.CreateToolStripMenuItems(ItemKind.Directory, True).Take(My.Settings.RecentItemsMaxCount))
        If items.Count > 0 Then
            items.Add(New ToolStripMenuItem(My.Resources.ClearRecentFolderList, My.Resources.IcoDelete.ToBitmap(), Sub() RecentManager.Clear(ItemKind.Directory)))
        End If

    End Sub

    Public Sub Load(item As SearchResult)

        If item Is Nothing Then Throw New ArgumentNullException("e")

        Explorer.TreeView.Select()
        Application.DoEvents()
        If item.IsDirectory Then
            Explorer.LoadDirectory(item.FullName)
        Else
            If Explorer.IsCurrentProjectNode(item.FullName) = False Then
                Explorer.LoadProject(item.FullName)
            End If
        End If

    End Sub

    Private Sub SaveManager_SaveRequested(sender As Object, e As SaveEventArgs)

        If e.Mode = SaveMode.All OrElse e.Mode = SaveMode.Project OrElse e.Mode = SaveMode.Replacement OrElse e.Mode = SaveMode.Pattern OrElse e.Mode = SaveMode.ProjectInfo Then
            e.Container = App.ProjectContainer
        End If
        If e.Mode = SaveMode.All OrElse e.Mode = SaveMode.Input OrElse e.Mode = SaveMode.InputText Then
            e.Input = Panels.Input.Input
        End If

    End Sub

    Private Sub TreeView_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Escape Then
                If Panels.Pattern._rtb.Enabled Then
                    Panels.Pattern._rtb.Select()
                End If
            End If
        End If

    End Sub

    Public Sub SelectCurrentProjectNode()

        Dim node = CurrentProjectNode
        If node IsNot Nothing AndAlso node.IsSelected = False Then
            _trv.SelectedNode = node
        End If

    End Sub

    Public Sub SelectCurrentInputNode()

        Dim node = CurrentInputNode
        If node IsNot Nothing AndAlso node.IsSelected = False Then
            _trv.SelectedNode = node
        End If

    End Sub

    Public ReadOnly Property CurrentProjectFileNameWithoutExtension As String
        Get
            Dim path = CurrentProjectPath
            Return If(path IsNot Nothing, System.IO.Path.GetFileNameWithoutExtension(path), Nothing)
        End Get
    End Property

    Public ReadOnly Property CurrentInputFileNameWithoutExtension As String
        Get
            Dim fileNode = TryCast(CurrentInputNode, FileInputNode)
            If fileNode IsNot Nothing Then
                Return fileNode.FileNameWithoutExtension
            Else
                Dim inputNode = TryCast(CurrentInputNode, InputNode)
                If inputNode IsNot Nothing Then
                    Return inputNode.FileName
                End If
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Iterator Property CurrentPaths As IEnumerable(Of String)
        Get
            If CurrentProjectPath IsNot Nothing Then
                Yield CurrentProjectPath
            End If
            If CurrentInputFullName IsNot Nothing Then
                Yield CurrentInputFullName
            End If
        End Get
    End Property

    Public Overrides ReadOnly Property DefaultInput As Input
        Get
            Return App.DefaultInput
        End Get
    End Property

    Public Overrides ReadOnly Property DefaultProjectContainer As ProjectContainer
        Get
            Return App.DefaultProjectContainer
        End Get
    End Property

    Friend _pnl As Panel
    Friend _trv As FileSystemTreeView
    Friend _tsp As AppToolStrip
    Friend _sts As StatusStrip
    Friend _lblPath As ToolStripStatusLabel
    Friend _btnReload As ToolStripButton
    Friend _btnCollapseAll As ToolStripButton

End Class