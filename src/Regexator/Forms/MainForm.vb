Imports Regexator
Imports Regexator.Text
Imports System.Diagnostics.CodeAnalysis
Imports Regexator.Windows.Forms

Public Class MainForm

    Public Sub New()

        InitializeComponent()
        Text = My.Application.Info.Title
#If DEBUG Then
        Text &= " (DEBUG)"
#End If
        Icon = My.Resources.IcoRegexator
        SetLocationAndSize(My.Settings.MainFormWindowState, My.Settings.MainFormLocation, My.Settings.MainFormSize)

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        Dim success = False
        Using New RedrawDisabler(Me)
            SetSplitterDistance()
            Initialize()
            Panels.Pattern._tbxCurrentLine.Visible = Panels.Pattern.CurrentLineOnly
            Panels.Replacement._tbxCurrentLine.Visible = Panels.Replacement.CurrentLineOnly
            Panels.Input._tbxCurrentLine.Visible = Panels.Input.CurrentLineOnly
            _mnsMain.LoadItems()
            _tspMain.LoadItems()
            If My.Settings.ProjectExplorerOnLeft = False Then
                SwapPanels()
            End If
            SetSplitterDistance()
            ActiveControl = Explorer._trv
            Panels.Output.OutputEnabled = My.Settings.OutputEnabled
            success = My.Settings.AppExitSuccess
            My.Settings.AppExitSuccess = False
            My.Settings.Save()
        End Using

        Export.LoadSettings()
        Explorer.InitialLoad(success)
        MyBase.OnLoad(e)

    End Sub

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Private Sub Initialize()

        _tslMode.Text = EnumHelper.GetDescription(EvaluationMode.Match)
        _tslMode.ToolTipText = My.Resources.EvaluationMode
        _tslExportMode.Text = Regexator.Text.EnumHelper.GetDescription(My.Settings.CodeExportMode)
        _tslExportMode.ToolTipText = My.Resources.ExportMode
        _tslEncoding.ToolTipText = My.Resources.InputFileEncoding
        _tslGroups.ToolTipText = My.Resources.Groups
        _stsMain.Visible = My.Settings.MainFormStatusBarVisible

        Panels.Pattern = New PatternPanel()
        _tbpPattern.Controls.Add(Panels.Pattern._pnl)
        _tbcPattern.TabPages.Add(_tbpPattern)
        _spcPattern.Panel1.Controls.Add(_tbcPattern)

        Panels.Replacement = New ReplacementPanel()
        _tbpReplacement.Controls.Add(Panels.Replacement._pnl)

        Info = New InfoPanel()
        _tbpInfo.Controls.Add(Info._pnl)

        Export = New ExportPanel()
        _tbpExport.Controls.Add(Export._pnl)

        FindResults = New FindResultsPanel()
        _tbpFindResults.Controls.Add(FindResults._pnl)

#If TATA Then
        App.FormattedPanel = New FormattedPanel()
        _tbpFormatted.Controls.Add(App.FormattedPanel._pnl)
        _tbcPattern.TabPages.Add(_tbpFormatted)
#End If

        Array.ForEach(App.RegexOptionsManager.Items.ToArray(), Sub(f) f.Visible = My.Settings.VisibleRegexOptions.Contains(f.Value))
        Options = New AppRegexOptionsPanel(App.RegexOptionsManager)
        AddHandler App.RegexOptionsManager.ValueChanged, Sub() Panels.Pattern.CreateRegex()
        _tbpOptions.Controls.Add(Options._pnl)
        _tspMain.CreateOptionsButtons()

        Groups = New GroupPanel()
        _tbpGroups.Controls.Add(Groups._pnl)

        Panels.Input = New InputPanel()
        _tbpInput.Controls.Add(Panels.Input._pnl)
        _tbcInput.TabPages.Add(_tbpInput)
        _spcInputOutput.Panel1.Controls.Add(_tbcInput)

        Explorer = New ExplorerPanel()
        _spc.Panel1.Controls.Add(Explorer._pnl)

        _spcPattern.Panel2.Controls.Add(_tbcOther)

        AppUtility.AddTabPages(_tbcOther, {_tbpReplacement, _tbpGroups, _tbpOptions, _tbpInfo, _tbpExport, _tbpFindResults}, My.Settings.OtherTabNames)

        AddHandler _tbcOther.SelectedIndexChanged,
            Sub()
                If ReferenceEquals(_tbcOther.SelectedTab, _tbpExport) Then
                    Export.PatternText = Panels.Pattern.CurrentText
                End If
            End Sub

        LoadOutputControls()

        SetOutputWordWrap()
        Dim btnWordWrap = Panels.Output._tsp._btnWordWrap
        AddHandler btnWordWrap.CheckedChanged,
            Sub()
                If _tbpText.IsSelected() Then
                    OutputText._rtb.WordWrap = btnWordWrap.Checked
                ElseIf _tbpSummary.IsSelected() Then
                    Summary._rtb.WordWrap = btnWordWrap.Checked
                End If
            End Sub

        Dim dgv = OutputTable._dgv
        Dim btnGroup = Panels.Output._tsp._btnGroupLayout
        Dim btnValue = Panels.Output._tsp._btnValueLayout

        btnGroup.Checked = (dgv.MatchModeLayout = TableLayout.Group)
        btnValue.Checked = (dgv.MatchModeLayout = TableLayout.Value)

        AddHandler btnValue.CheckedChanged, Sub() dgv.MatchModeLayout = If(btnValue.Checked, TableLayout.Value, TableLayout.Group)
        AddHandler btnGroup.CheckedChanged, Sub() dgv.MatchModeLayout = If(btnGroup.Checked, TableLayout.Group, TableLayout.Value)

        AddHandler dgv.MatchModeLayoutChanged, Sub()
                                                   btnGroup.Checked = (dgv.MatchModeLayout = TableLayout.Group)
                                                   btnValue.Checked = (dgv.MatchModeLayout = TableLayout.Value)
                                               End Sub

        AddHandler _tbcTextOutput.SelectedIndexChanged,
            Sub()
                If _tbcTextOutput.MovingTab = False Then
                    If IsTableTabSelected Then
                        OutputTable.LoadTable()
                        OutputTable.EnsureDisplayed()
                    ElseIf IsSummaryTabSelected Then
                        Summary.LoadSummary()
                    End If
                    If IsTableTabSelected Then
                        Panels.Output._tsp.ShowTableLayoutItems()
                    Else
                        Panels.Output._tsp.HideTableLayoutItems()
                    End If
                    SetOutputWordWrap()
                End If
            End Sub

    End Sub

    Private Sub LoadOutputControls()

        OutputText = New OutputTextPanel()
        _tbpText = New TabPage(My.Resources.Text) With {.Name = "OutputTextTab"}
        _tbpText.Controls.Add(OutputText._pnl)

        OutputTable = New OutputTablePanel()
        _tbpTable = New TabPage(My.Resources.Table) With {.Name = "OutputTableTab"}
        _tbpTable.Controls.Add(OutputTable._pnl)

        Summary = New SummaryPanel()
        _tbpSummary = New TabPage(My.Resources.Summary) With {.Name = "OutputSummaryTab"}
        _tbpSummary.Controls.Add(Summary._pnl)

        _tbcTextOutput = New AppTabControl()
        AppUtility.AddTabPages(_tbcTextOutput, {_tbpText, _tbpTable, _tbpSummary}, My.Settings.TextOutputTabNames)

        FileSystemSearchResults = New FileSystemSearchResultsPanel()
        _tbpFileSystemSearchResults = New TabPage(My.Resources.Results) With {.Name = "FileSystemSearchResultsTab"}
        _tbpFileSystemSearchResults.Controls.AddRange({FileSystemSearchResults._pnl})

        _tbcFileSystemSearch = New AppTabControl()
        AppUtility.AddTabPages(_tbcFileSystemSearch, {_tbpFileSystemSearchResults}, My.Settings.FileSystemSearchTabNames)

        Panels.Output = New OutputPanel()
        Panels.Output.Options = My.Settings.DefaultOutputOptions
        _tbcOutput = New AppTabControl()

        _tbpOutput = New TabPage(My.Resources.Output) With {.Name = "OutputTab"}
        _tbpOutput.Controls.AddRange({_tbcTextOutput, Panels.Output._tsp})

        _tbpFileSystemSearch = New TabPage(My.Resources.FileSystemSearch) With {.Name = "FileSystemSearchTab"}
        _tbpFileSystemSearch.Controls.AddRange({_tbcFileSystemSearch, FileSystemSearchResults._tsp})

        AppUtility.AddTabPages(_tbcOutput, {_tbpOutput, _tbpFileSystemSearch}, My.Settings.OutputTabNames)

        _spcInputOutput.Panel2.Controls.Add(_tbcOutput)

    End Sub

    Private Sub SetOutputWordWrap()

        Dim btn = Panels.Output._tsp._btnWordWrap
        If _tbpText.IsSelected() Then
            btn.Checked = OutputText._rtb.WordWrap
            btn.Visible = True
        ElseIf _tbpSummary.IsSelected() Then
            btn.Checked = Summary._rtb.WordWrap
            btn.Visible = True
        Else
            btn.Checked = False
            btn.Visible = False
        End If

    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)

        AppUtility.CheckVersionAsync()
        MyBase.OnShown(e)

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        e.Cancel = False
#If DEBUG Then
        If e.CloseReason <> CloseReason.TaskManagerClosing AndAlso e.CloseReason <> CloseReason.WindowsShutDown Then
#Else
        If App.CriticalException = False AndAlso e.CloseReason <> CloseReason.TaskManagerClosing AndAlso e.CloseReason <> CloseReason.WindowsShutDown Then
#End If
            e.Cancel = Not Explorer.SaveManager.Save(True)
            If e.Cancel = False Then
                AppSettings.Save()
            End If
        End If
        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        Dim modifiers = keyData And Keys.Modifiers
        If modifiers = Keys.Control OrElse modifiers = (Keys.Control Or Keys.Shift) Then
            If (keyData And Keys.KeyCode) = Keys.Tab Then
                Switcher.Instance.Show()
                Return True
            End If
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

    <SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")>
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Select Case e.Modifiers
            Case Keys.None
                Select Case e.KeyCode
                    Case Keys.F1
                        AppForms.ShowGuideForm()
                        e.SuppressKeyPress = True
                    Case Keys.F5
                        If App.MainForm.IsFileSystemSearchTabSelected Then
                            FileSystemSearchResults.ExecuteSearch()
                            e.SuppressKeyPress = True
                        End If
                End Select
            Case Keys.Alt
                Select Case e.KeyCode
                    Case Keys.F2
                        Panels.Output.OutputEnabled = Not Panels.Output.OutputEnabled
                        e.SuppressKeyPress = True
                End Select
            Case Keys.Control
                Select Case e.KeyCode
                    Case Keys.G
                        Panels.Output.Highlight()
                        e.SuppressKeyPress = True
                    Case Keys.N
                        Explorer.AddNewProjectOrFolder()
                        e.SuppressKeyPress = True
                    Case Keys.O
                        Explorer.LoadProject()
                        e.SuppressKeyPress = True
                    Case Keys.S
                        If Info._pgd.ContainsFocus Then
                            Explorer.SaveProjectInfo()
                            e.SuppressKeyPress = True
                        End If
                    Case Keys.Oemtilde
                        If App.Mode = EvaluationMode.Match Then
                            Groups.ToggleGroupEnabled(0)
                            e.SuppressKeyPress = True
                        End If
                    Case Keys.OemMinus
                        Explorer.History.UndoIfCan()
                        e.SuppressKeyPress = True
                    Case Keys.F2
                        SelectNextMode()
                        e.SuppressKeyPress = True
                    Case Else
                        If App.Mode = EvaluationMode.Match AndAlso e.KeyValue >= 48 AndAlso e.KeyValue <= 57 Then
                            Groups.ToggleGroupEnabled(e.KeyValue - 48)
                            e.SuppressKeyPress = True
                        End If
                End Select
            Case Keys.Shift
                Select Case e.KeyCode
                    Case Keys.F1
                        AppForms.ShowUserGuideForm()
                        e.SuppressKeyPress = True
                End Select
            Case (Keys.Control Or Keys.Shift)
                Select Case e.KeyCode
                    Case Keys.G
                        Panels.Output.Highlight(HighlightOptions.None)
                        e.SuppressKeyPress = True
                    Case Keys.S
                        Explorer.SaveAll()
                        e.SuppressKeyPress = True
                    Case Keys.Oemtilde
                        Options.SetAllOrNone()
                        e.SuppressKeyPress = True
                    Case Keys.OemMinus
                        Explorer.History.RedoIfCan()
                        e.SuppressKeyPress = True
                    Case Keys.F2
                        SelectPreviousMode()
                        e.SuppressKeyPress = True
                    Case Else
                        If e.KeyValue >= 49 AndAlso e.KeyValue <= 57 Then
                            Options.ToggleItemEnabled(e.KeyValue - 49)
                            e.SuppressKeyPress = True
                        End If
                End Select
        End Select
        MyBase.OnKeyDown(e)

    End Sub

    Public Sub SwapPanels()

        SuspendLayout()
        SwapPanelsInternal()
        ResumeLayout(True)

    End Sub

    Private Sub SwapPanelsInternal()

        Dim c1 = _spc.Panel1.Controls
        Dim c2 = _spc.Panel2.Controls
        Dim pnl = Explorer._pnl
        If c1.Contains(pnl) Then
            c1.Remove(pnl)
            c2.Remove(_pnlRegex)
            _spc.SplitterDistance = _spc.Width - _spc.SplitterDistance
            c1.Add(_pnlRegex)
            c2.Add(pnl)
            My.Settings.ProjectExplorerOnLeft = False
        Else
            c1.Remove(_pnlRegex)
            c2.Remove(pnl)
            _spc.SplitterDistance = _spc.Width - _spc.SplitterDistance
            c1.Add(pnl)
            c2.Add(_pnlRegex)
            My.Settings.ProjectExplorerOnLeft = True
        End If

    End Sub

    Private Sub SetSplitterDistance()

        _spcInputOutput.Orientation = My.Settings.InputOutputOrientation
        Try
            If My.Settings.MainFormSplitterDistanceMain <> -1 Then
                _spc.SplitterDistance = My.Settings.MainFormSplitterDistanceMain
            End If
            _spcRegex.SplitterDistance = GetRegexSplitterDistance()
            _spcInputOutput.SplitterDistance = GetInputOutputSplitterDistance()
            If My.Settings.MainFormSplitterDistancePattern <> -1 Then
                _spcPattern.SplitterDistance = My.Settings.MainFormSplitterDistancePattern
            End If
        Catch ex As InvalidOperationException
            Debug.Assert(False)
        End Try

    End Sub

    Private Function GetInputOutputSplitterDistance() As Integer

        Dim spc = _spcInputOutput
        If spc.Orientation = Orientation.Horizontal Then
            If My.Settings.MainFormSplitterDistanceInputOutputHorizontal <> -1 Then
                Return My.Settings.MainFormSplitterDistanceInputOutputHorizontal
            End If
        Else
            If My.Settings.MainFormSplitterDistanceInputOutputVertical <> -1 Then
                Return My.Settings.MainFormSplitterDistanceInputOutputVertical
            End If
        End If
        Return CInt(If(spc.Orientation = Orientation.Horizontal, spc.Height, spc.Width) / 2 - (spc.SplitterWidth / 2))

    End Function

    Private Function GetRegexSplitterDistance() As Integer

        If _spcInputOutput.Orientation = Orientation.Horizontal Then
            If My.Settings.MainFormSplitterDistanceRegexHorizontal <> -1 Then
                Return My.Settings.MainFormSplitterDistanceRegexHorizontal
            End If
        Else
            If My.Settings.MainFormSplitterDistanceRegexVertical <> -1 Then
                Return My.Settings.MainFormSplitterDistanceRegexVertical
            End If
        End If
        Return _spcRegex.SplitterDistance

    End Function

    Public Sub ToggleInputOutputOrientation()

        If _spcInputOutput.Orientation = Orientation.Horizontal Then
            My.Settings.MainFormSplitterDistanceInputOutputHorizontal = _spcInputOutput.SplitterDistance
            My.Settings.MainFormSplitterDistanceRegexHorizontal = _spcRegex.SplitterDistance
        Else
            My.Settings.MainFormSplitterDistanceInputOutputVertical = _spcInputOutput.SplitterDistance
            My.Settings.MainFormSplitterDistanceRegexVertical = _spcRegex.SplitterDistance
        End If
        _spcInputOutput.Orientation = If(_spcInputOutput.Orientation = Orientation.Horizontal, Orientation.Vertical, Orientation.Horizontal)
        _spcRegex.SplitterDistance = GetRegexSplitterDistance()
        _spcInputOutput.SplitterDistance = GetInputOutputSplitterDistance()
        My.Settings.InputOutputOrientation = _spcInputOutput.Orientation

    End Sub

    Public ReadOnly Property IsTextTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbcTextOutput.SelectedTab, _tbpText)
        End Get
    End Property

    Public ReadOnly Property IsTableTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbcTextOutput.SelectedTab, _tbpTable)
        End Get
    End Property

    Public ReadOnly Property IsFileSystemSearchTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbcOutput.SelectedTab, _tbpFileSystemSearch)
        End Get
    End Property

    Public ReadOnly Property IsSummaryTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbcTextOutput.SelectedTab, _tbpSummary)
        End Get
    End Property

    Friend _tbcPattern As New ExtendedTabControl() With {.AllowDrop = False, .Dock = DockStyle.Fill, .Appearance = TabAppearance.FlatButtons}
    Friend _tbcInput As New ExtendedTabControl() With {.AllowDrop = False, .Dock = DockStyle.Fill, .Appearance = TabAppearance.FlatButtons}
    Friend _tbcOther As New ExtendedTabControl() With {.AllowDrop = True, .Dock = DockStyle.Fill, .Appearance = TabAppearance.FlatButtons}

    Friend _tbpPattern As New TabPage(My.Resources.Pattern) With {.Name = "PatternTab"}
    Friend _tbpInput As New TabPage(My.Resources.Input) With {.Name = "InputTab"}
    Friend _tbpOptions As New TabPage(My.Resources.RegexOptions) With {.Name = "OptionsTab"}
    Friend _tbpGroups As New TabPage(My.Resources.Groups) With {.Name = "GroupsTab"}
    Friend _tbpReplacement As New TabPage(My.Resources.Replacement) With {.Name = "ReplacementTab"}
    Friend _tbpInfo As New TabPage(My.Resources.Info) With {.Name = "ProjectInfoTab"}
    Friend _tbpExport As New TabPage(My.Resources.Export) With {.Name = "ExportTab"}
    Friend _tbpFindResults As New TabPage(My.Resources.FindResults) With {.Name = "FindResultsTab"}

    Friend _tbcOutput As AppTabControl
    Friend _tbcTextOutput As AppTabControl
    Friend _tbcFileSystemSearch As AppTabControl

    Friend _tbpOutput As TabPage
    Friend _tbpText As TabPage
    Friend _tbpTable As TabPage
    Friend _tbpSummary As TabPage
    Friend _tbpFileSystemSearch As New TabPage
    Friend _tbpFileSystemSearchResults As TabPage

#If TATA Then
    Friend _tbpFormatted As New TabPage(My.Resources.Formatted) With {.Name = "FormattedPatternTab"}
#End If

End Class