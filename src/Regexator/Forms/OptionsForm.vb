Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Drawing.Text
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Regexator
Imports Regexator.Drawing
Imports Regexator.Output
Imports Regexator.Text
Imports Regexator.Windows.Forms
Imports Regexator.Collections.Generic

Public Class OptionsForm

    Public Sub New()

        Me.New(OptionsFormTab.Application)

    End Sub

    Public Sub New(initialTab As OptionsFormTab)

        InitializeComponent()
        _initialTab = initialTab
        Text = My.Resources.Options
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedSingle
        KeyPreview = True
        Icon = My.Resources.IcoRegexator
        _btnOk.Text = My.Resources.OK
        _btnCancel.Text = My.Resources.Cancel
        _tbpApplication.Text = My.Resources.Application
        _tbpTextEditor.Text = My.Resources.TextEditor
        _tbpFormats.Text = My.Resources.FontsAndColors
        _tbpProjectExplorer.Text = My.Resources.ProjectExplorer
        _tbpSnippets.Text = My.Resources.Snippets
        _tbpOutput.Text = My.Resources.Output
        _tbpSymbols.Text = My.Resources.Symbols
        _tbpExport.Text = My.Resources.Export
        _tbpRegexOptions.Text = My.Resources.RegexOptions
        _tbpProjectDefaultValues.Text = My.Resources.ProjectDefaultValues
        _tbpInputDefaultValues.Text = My.Resources.InputDefaultValues
        _btnSetDefaultFavoriteSnippets.Text = My.Resources.SetDefaultFavoriteSnippets

        _loaders.Add(_tbpApplication, AddressOf LoadApplication)
        _loaders.Add(_tbpTextEditor, AddressOf LoadTextEditor)
        _loaders.Add(_tbpFormats, AddressOf LoadFormats)
        _loaders.Add(_tbpProjectExplorer, AddressOf LoadProjectExplorer)
        _loaders.Add(_tbpSnippets, AddressOf LoadSnippets)
        _loaders.Add(_tbpOutput, AddressOf LoadOutput)
        _loaders.Add(_tbpFileSystemSearch, AddressOf LoadFileSystemSearch)
        _loaders.Add(_tbpSymbols, AddressOf LoadSymbols)
        _loaders.Add(_tbpExport, AddressOf LoadExport)
        _loaders.Add(_tbpRegexOptions, AddressOf LoadRegexOptions)
        _loaders.Add(_tbpProjectDefaultValues, AddressOf LoadProjectDefaultValues)
        _loaders.Add(_tbpInputDefaultValues, AddressOf LoadInputDefaultValues)

        LoadTab(_tbpApplication)

        AddHandler _btnOk.Click,
            Sub()
                DialogResult = DialogResult.OK
                Close()
            End Sub
        AddHandler _btnCancel.Click,
            Sub()
                DialogResult = DialogResult.Cancel
                Close()
            End Sub
        AddHandler _btnSetDefaultFavoriteSnippets.Click,
            Sub()
                If MessageDialog.Question(My.Resources.DoYouWantToSetDefaultFavoriteSnippetsMsg) = System.Windows.Forms.DialogResult.Yes Then
                    _fSetDefaultSnippets = True
                End If
            End Sub
        AddHandler _tbc.Selected, Sub(sender As Object, e As TabControlEventArgs) LoadTab(e.TabPage)

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        Select Case _initialTab
            Case OptionsFormTab.Application
                _tbc.SelectedTab = _tbpApplication
            Case OptionsFormTab.DefaultValues
                _tbc.SelectedTab = _tbpProjectDefaultValues
            Case OptionsFormTab.Export
                _tbc.SelectedTab = _tbpExport
            Case OptionsFormTab.ProjectExplorer
                _tbc.SelectedTab = _tbpProjectExplorer
            Case OptionsFormTab.Snippets
                _tbc.SelectedTab = _tbpSnippets
            Case OptionsFormTab.RegexOptions
                _tbc.SelectedTab = _tbpRegexOptions
        End Select
        MyBase.OnLoad(e)

    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)

        Activate()
        ActiveControl = _tbpApplication
        MyBase.OnShown(e)

    End Sub

    Private Sub LoadTab(tbp As TabPage)

        If tbp IsNot Nothing AndAlso _loadedTabs.Contains(tbp) = False Then
            BeginUpdate()
            _loaders(tbp)()
            EndUpdate()
            _loadedTabs.Add(tbp)
        End If

    End Sub

    Private Sub LoadApplication()

        _lblRecentItemsMaxCount.Text = My.Resources.MaxNumberOfRecentItems & My.Resources.ColonStr
        _lblMatchTimeout.Text = My.Resources.RegexMatchTimeoutMsg & My.Resources.ColonStr
        _numRecentMaxCount.Increment = 1
        _numRecentMaxCount.Minimum = 4
        _numRecentMaxCount.Maximum = 64
        _numRecentMaxCount.Value = My.Settings.RecentItemsMaxCount
        _numMatchTimeout.Increment = 1
        _numMatchTimeout.Minimum = 0
        _numMatchTimeout.Maximum = 86400
        _numMatchTimeout.Value = CDec(My.Settings.MatchTimeout.Duration().TotalSeconds)
        _cbxCheckNewVersion.Text = My.Resources.CheckForUpdates
        _cbxCheckNewVersion.Checked = My.Settings.CheckAppVersion
        _cbxShowStatusBar.Text = My.Resources.ShowStatusBar
        _cbxShowStatusBar.Checked = My.Settings.MainFormStatusBarVisible

    End Sub

    Private Sub SaveApplication()

        If _loadedTabs.Contains(_tbpApplication) Then
            My.Settings.MatchTimeout = TimeSpan.FromSeconds(CDbl(_numMatchTimeout.Value))
            My.Settings.RecentItemsMaxCount = CInt(_numRecentMaxCount.Value)
            My.Settings.CheckAppVersion = _cbxCheckNewVersion.Checked
            My.Settings.MainFormStatusBarVisible = _cbxShowStatusBar.Checked
        End If

    End Sub

    Private Sub LoadTextEditor()

        _cbxPatternIndentNewLine.Text = My.Resources.IndentNewLinePatternEditorOnly
        _cbxPatternIndentNewLine.Checked = My.Settings.PatternIndentNewLine
        _cbxSelectEntireCurrentLineAfterLoad.Text = My.Resources.SelectEntireCurrentLineAfterLoad
        _cbxSelectEntireCurrentLineAfterLoad.Checked = My.Settings.SelectEntireCurrentLineAfterLoad

    End Sub

    Private Sub SaveTextEditor()

        If _loadedTabs.Contains(_tbpTextEditor) Then
            My.Settings.PatternIndentNewLine = _cbxPatternIndentNewLine.Checked
            My.Settings.SelectEntireCurrentLineAfterLoad = _cbxSelectEntireCurrentLineAfterLoad.Checked
        End If

    End Sub

    Private Sub LoadProjectExplorer()

        _cbxUseRecycleBin.Checked = My.Settings.FileSystemUseRecycleBin
        _cbxUseRecycleBin.Text = My.Resources.MoveDeletedItemsToRecycleBin
        _cbxShowHiddenFiles.Checked = My.Settings.FileSystemShowHidden
        _cbxShowHiddenFiles.Text = My.Resources.ShowHiddenFilesLC
        _cbxTrackActiveItem.Checked = My.Settings.TrackActiveItemInSolutionExplorer
        _cbxTrackActiveItem.Text = My.Resources.TrackActiveItem
        _cbxConfirmFileInputRemoval.Checked = My.Settings.ConfirmFileInputRemoval
        _cbxConfirmFileInputRemoval.Text = My.Resources.ConfirmFileInputRemoval

    End Sub

    Private Sub SaveProjectExplorer()

        If _loadedTabs.Contains(_tbpProjectExplorer) Then
            My.Settings.FileSystemUseRecycleBin = _cbxUseRecycleBin.Checked
            My.Settings.FileSystemShowHidden = _cbxShowHiddenFiles.Checked
            My.Settings.TrackActiveItemInSolutionExplorer = _cbxTrackActiveItem.Checked
            My.Settings.ConfirmFileInputRemoval = _cbxConfirmFileInputRemoval.Checked
        End If

    End Sub

    Private Sub LoadSnippets()

        _cbxSetSnippetOptions.Checked = My.Settings.SetOptionsIfRequiredBySnippet
        _cbxSetSnippetOptions.Text = My.Resources.SetRegexOptionsIfRequiredBySnippet
        _gbxHiddenCategories.Text = My.Resources.HiddenCategories
        _lsvHiddenCategories = New OptionsListView()
        _lsvHiddenCategories.Items.AddRange([Enum].GetValues(GetType(RegexCategory)) _
            .Cast(Of RegexCategory) _
            .Where(Function(f) f <> RegexCategory.None) _
            .Select(Function(f) New With {.Value = f, .Title = TextUtility.SplitCamelCase(f)}) _
            .OrderBy(Function(f) f.Title) _
            .Select(Function(f) New ListViewItem(f.Title) With {.Tag = f.Value, .Checked = Data.SnippetManager.HiddenCategories.Contains(f.Value)}) _
            .ToArray())
        _gbxHiddenCategories.Controls.Add(_lsvHiddenCategories)
        _lblHiddenCategoriesInfo.Text = My.Resources.ApplicableForPatternEditorSnippetList
        _btnViewSnippetErrorLog.Visible = Data.SnippetErrorLog.Any()
        _btnViewSnippetErrorLog.Text = My.Resources.ViewErrorLog
        AddHandler _btnViewSnippetErrorLog.Click, Sub(sender As Object, e As EventArgs) UI.ErrorInfoForm.ShowErrors(Data.SnippetErrorLog.ToArray(), My.Resources.FollowingSnippetsCouldNotBeLoadedMsg)
        _dgvSnippetDirectories = New SnippetDirectoryDataGridView()
        _pnlSnippetDirectories.Controls.Add(_dgvSnippetDirectories)
        Dim infos = AppSettings.SnippetDirectories.ToArray()
        _snippetDirectories = infos.Select(Function(f) f.FullName).ToArray()
        Dim lst = infos.Select(Function(f) New SnippetDirectory(f)).ToList()
        _dgvSnippetDirectories.DataSource = New BindingList(Of SnippetDirectory)(lst) With {.AllowEdit = True, .AllowNew = True, .AllowRemove = True}

    End Sub

    Private Sub SaveSnippets()

        If _loadedTabs.Contains(_tbpSnippets) Then
            My.Settings.SetOptionsIfRequiredBySnippet = _cbxSetSnippetOptions.Checked
            For Each item As ListViewItem In _lsvHiddenCategories.Items
                Dim category = DirectCast(item.Tag, RegexCategory)
                If item.Checked Then
                    Data.SnippetManager.HiddenCategories.Add(category)
                Else
                    Data.SnippetManager.HiddenCategories.Remove(category)
                End If
            Next
            AppSettings.SaveHiddenCategories()
            If _fSetDefaultSnippets Then
                AppSettings.SetDefaultFavoriteSnippets()
                Data.SnippetManager.SetFavoriteSnippets(My.Settings.FavoriteSnippets.Cast(Of String).ToArray())
            End If
            Dim infos = DirectCast(_dgvSnippetDirectories.DataSource, BindingList(Of SnippetDirectory)) _
                .Select(Function(f) f.Info) _
                .Where(Function(f) f IsNot Nothing) _
                .ToArray()
            If _snippetDirectories.SequenceEqualUnordered(infos.Select(Function(f) f.FullName)) = False Then
                Data.ResetSnippetManager()
                AppSettings.SnippetDirectories = infos
            End If
        End If

    End Sub

    Private Sub LoadExport()

        _lblDefaultExportMode.Text = My.Resources.Mode & My.Resources.ColonStr
        _cmbExportMode.ValueMember = "Value"
        _cmbExportMode.DisplayMember = "Title"
        _cmbExportMode.DataSource = [Enum].GetValues(GetType(ExportMode)) _
            .Cast(Of ExportMode) _
            .Select(Function(f) New With {.Value = f, .Title = Regexator.Text.EnumHelper.GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .ToList()

        AddHandler _cmbExportMode.SelectedValueChanged,
            Sub()
                _cbxMultiline.Enabled = (DirectCast(_cmbExportMode.SelectedValue, ExportMode) <> ExportMode.CSharp)
            End Sub

        _cmbExportMode.SelectedValue = My.Settings.CodeExportMode

        _cbxMultiline.Text = My.Resources.Multiline
        _cbxMultiline.Checked = My.Settings.CodeExportMultiline

    End Sub

    Private Sub SaveExport()

        If _loadedTabs.Contains(_tbpExport) Then
            My.Settings.CodeExportMode = DirectCast(_cmbExportMode.SelectedValue, ExportMode)
        End If

        My.Settings.CodeExportMultiline = _cbxMultiline.Checked

    End Sub

    Private Sub LoadRegexOptions()

        _cbxRegexOptionsHotkeyNumberVisible.Text = My.Resources.ShowHotkeyNumber
        _cbxRegexOptionsHotkeyNumberVisible.Checked = My.Settings.RegexOptionsHotkeyNumberVisible
        _cbxRegexOptionsDescriptionVisible.Text = My.Resources.ShowDescription
        _cbxRegexOptionsDescriptionVisible.Checked = My.Settings.RegexOptionsDescriptionVisible
        _lsvRegexOptionsVisibility = New OptionsListView()
        _lsvRegexOptionsVisibility.Items.AddRange(App.RegexOptionsManager.Items _
            .OrderBy(Function(f) f.Text) _
            .Select(Function(f) New ListViewItem(f.Text) With {.Tag = f, .Checked = f.Visible}) _
            .ToArray())
        _gbxRegexOptionsVisibility.Text = My.Resources.Visibility
        _gbxRegexOptionsVisibility.Controls.Add(_lsvRegexOptionsVisibility)

    End Sub

    Private Sub SaveRegexOptions()

        If _loadedTabs.Contains(_tbpRegexOptions) Then
            My.Settings.RegexOptionsHotkeyNumberVisible = _cbxRegexOptionsHotkeyNumberVisible.Checked
            My.Settings.RegexOptionsDescriptionVisible = _cbxRegexOptionsDescriptionVisible.Checked
            For Each item In _lsvRegexOptionsVisibility.Items _
                .Cast(Of ListViewItem) _
                .Select(Function(f) New With {.Checked = f.Checked, .Item = DirectCast(f.Tag, RegexOptionsItem)})
                item.Item.Visible = item.Checked
            Next
        End If

    End Sub

    Private Sub LoadOutput()

        _lblOutputLimit.Text = My.Resources.OutputItemsLimit & My.Resources.ColonStr
        _numOutputLimit.Minimum = 1
        _numOutputLimit.Maximum = 1000000000
        _numOutputLimit.Increment = 1000
        _numOutputLimit.Value = My.Settings.OutputLimit
        _numOutputLimit.ThousandsSeparator = True
        _lblNumberAlignment.Text = My.Resources.NumberAlignment & My.Resources.ColonStr
        Dim lst = [Enum].GetValues(GetType(TextAlignment)) _
            .Cast(Of TextAlignment) _
            .Select(Function(f) New With {.Value = f, .Title = GetDescription(f)}) _
            .ToList()
        _cmbNumberAlignment.ValueMember = "Value"
        _cmbNumberAlignment.DisplayMember = "Title"
        _cmbNumberAlignment.DataSource = lst
        _cmbNumberAlignment.SelectedValue = My.Settings.OutputNumberAlignment
        _cbxOutputOmitRepeatedInfo.Checked = My.Settings.OutputOmitRepeatedInfo
        _cbxOutputOmitRepeatedInfo.Text = My.Resources.OmitRepeatedInfo
        _cbxHighlightBeforeAfterResult.Checked = My.Settings.OutputHighlightBeforeAfterResult
        _cbxHighlightBeforeAfterResult.Text = My.Resources.HighlightBeforeAndAfterResult
        _gbxOutputText.Text = My.Resources.Text

    End Sub

    Private Sub SaveOutput()

        If _loadedTabs.Contains(_tbpOutput) Then
            My.Settings.OutputOmitRepeatedInfo = _cbxOutputOmitRepeatedInfo.Checked
            My.Settings.OutputLimit = CInt(_numOutputLimit.Value)
            My.Settings.OutputNumberAlignment = DirectCast(_cmbNumberAlignment.SelectedValue, TextAlignment)
            My.Settings.OutputHighlightBeforeAfterResult = _cbxHighlightBeforeAfterResult.Checked
        End If

    End Sub

    Private Sub LoadFileSystemSearch()

        _numFileSystemSearchMaxResultCount.Value = My.Settings.FileSystemSearchMaxCount

    End Sub

    Private Sub SaveFileSystemSearch()

        If _loadedTabs.Contains(_tbpFileSystemSearch) Then
            My.Settings.FileSystemSearchMaxCount = CInt(_numFileSystemSearchMaxResultCount.Value)
        End If

    End Sub

    Private Sub LoadProjectDefaultValues()

        _tbpPatternDefaults.Text = My.Resources.Pattern
        _gbxDefaultPatternOptions.Text = My.Resources.Options
        _gbxDefaultRegexOptions.Text = My.Resources.RegexOptions

        _lsvDefaultRegexOptions = New OptionsListView()
        _lsvDefaultRegexOptions.Items.AddRange(RegexOptionsUtility.Values _
            .Select(Function(f) New With {.Value = f, .Title = TextUtility.SplitCamelCase(f)}) _
            .OrderBy(Function(f) f.Title) _
            .Select(Function(f) New ListViewItem(f.Title) With {.Tag = f.Value, .Checked = ((My.Settings.DefaultRegexOptions And f.Value) = f.Value)}) _
            .ToArray())
        _gbxDefaultRegexOptions.Controls.Add(_lsvDefaultRegexOptions)

        _lsvDefaultPatternOptions = New OptionsListView()
        Dim patternOptions = EnumHelper.PatternOptionsValues.Where(Function(f) EnumHelper.IsBrowsable(f)).ToArray()
        _lsvDefaultPatternOptions.Items.AddRange(patternOptions _
            .Select(Function(f) New With {.Value = f, .Title = EnumHelper.GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .Select(Function(f) New ListViewItem(f.Title) With {.Tag = f.Value, .Checked = ((My.Settings.DefaultPatternOptions And f.Value) = f.Value)}) _
            .ToArray())
        _gbxDefaultPatternOptions.Controls.Add(_lsvDefaultPatternOptions)

        _tbpOutputDefaults.Text = My.Resources.Output
        _gbxDefaultOutputOptions.Text = My.Resources.Options

        _lsvDefaultOutputOptions = New OptionsListView()
        _lsvDefaultOutputOptions.Items.AddRange(EnumHelper.OutputOptionsValues _
            .Select(Function(f) New With {.Value = f, .Title = EnumHelper.GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .Select(Function(f) New ListViewItem(f.Title) With {.Tag = f.Value, .Checked = ((My.Settings.DefaultOutputOptions And f.Value) = f.Value)}) _
            .ToArray())
        _gbxDefaultOutputOptions.Controls.Add(_lsvDefaultOutputOptions)

        _tbpReplacementDefaults.Text = My.Resources.Replacement
        _gbxDefaultReplacementOptions.Text = My.Resources.Options
        _gbxReplacementNewLine.Text = My.Resources.NewLine
        _gbxReplacementText.Text = My.Resources.Text
        _tbxReplacementText.Text = My.Settings.DefaultReplacementText

        _lsvDefaultReplacementOptions = New OptionsListView()
        _lsvDefaultReplacementOptions.Items.AddRange(EnumHelper.ReplacementOptionsValues _
            .Where(Function(f) EnumHelper.IsBrowsable(f)) _
            .Select(Function(f) New With {.Value = f, .Title = EnumHelper.GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .Select(Function(f) New ListViewItem(f.Title) With {.Tag = f.Value, .Checked = ((My.Settings.DefaultReplacementOptions And f.Value) = f.Value)}) _
            .ToArray())
        _gbxDefaultReplacementOptions.Controls.Add(_lsvDefaultReplacementOptions)

        _lstReplacementNewLine.ValueMember = "Value"
        _lstReplacementNewLine.DisplayMember = "Title"
        _lstReplacementNewLine.DataSource = [Enum].GetValues(GetType(NewLineMode)) _
            .Cast(Of NewLineMode) _
            .Select(Function(f) New With {.Value = f, .Title = GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .ToList()
        _lstReplacementNewLine.SelectedValue = My.Settings.DefaultReplacementNewLine

    End Sub

    Private Sub SaveProjectDefaultValues()

        If _loadedTabs.Contains(_tbpProjectDefaultValues) Then
            Dim regexOptions As RegexOptions = RegexOptions.None
            For Each item In _lsvDefaultRegexOptions.Items.Cast(Of ListViewItem).Where(Function(f) f.Checked)
                regexOptions = regexOptions Or DirectCast(item.Tag, RegexOptions)
            Next
            My.Settings.DefaultRegexOptions = regexOptions
            Dim outputOptions As OutputOptions = OutputOptions.None
            For Each item In _lsvDefaultOutputOptions.Items.Cast(Of ListViewItem).Where(Function(f) f.Checked)
                outputOptions = outputOptions Or DirectCast(item.Tag, OutputOptions)
            Next
            My.Settings.DefaultOutputOptions = outputOptions
            Dim patternOptions As PatternOptions = PatternOptions.None
            For Each item In _lsvDefaultPatternOptions.Items.Cast(Of ListViewItem).Where(Function(f) f.Checked)
                patternOptions = patternOptions Or DirectCast(item.Tag, PatternOptions)
            Next
            My.Settings.DefaultPatternOptions = patternOptions
            Dim replacementOptions As ReplacementOptions = ReplacementOptions.None
            For Each item In _lsvDefaultReplacementOptions.Items.Cast(Of ListViewItem).Where(Function(f) f.Checked)
                replacementOptions = replacementOptions Or DirectCast(item.Tag, ReplacementOptions)
            Next
            My.Settings.DefaultReplacementOptions = replacementOptions
            My.Settings.DefaultReplacementNewLine = DirectCast(_lstReplacementNewLine.SelectedValue, NewLineMode)
            My.Settings.DefaultReplacementText = _tbxReplacementText.Text
        End If

    End Sub

    Private Sub LoadInputDefaultValues()

        _tbpInputDefaultValues.Text = My.Resources.InputDefaultValues
        _gbxDefaultInputOptions.Text = My.Resources.Options
        _gbxInputNewLine.Text = My.Resources.NewLine
        _lsvDefaultInputOptions = New OptionsListView()
        Dim inputOptions = EnumHelper.InputOptionsValues.Where(Function(f) EnumHelper.IsBrowsable(f)).ToArray()
        _lsvDefaultInputOptions.Items.AddRange(inputOptions _
            .Select(Function(f) New With {.Value = f, .Title = EnumHelper.GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .Select(Function(f) New ListViewItem(f.Title) With {.Tag = f.Value, .Checked = ((My.Settings.DefaultInputOptions And f.Value) = f.Value)}) _
            .ToArray())
        _gbxDefaultInputOptions.Controls.Add(_lsvDefaultInputOptions)
        _lstInputNewLine.ValueMember = "Value"
        _lstInputNewLine.DisplayMember = "Title"
        _lstInputNewLine.DataSource = [Enum].GetValues(GetType(NewLineMode)) _
            .Cast(Of NewLineMode) _
            .Select(Function(f) New With {.Value = f, .Title = GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .ToList()
        _lstInputNewLine.SelectedValue = My.Settings.DefaultInputNewLine
        _gbxInputEncoding.Text = My.Resources.Encoding
        _cmbInputEncoding.Items.AddRange(EncodingItem.EnumerateItems().ToArray())
        _cmbInputEncoding.SelectedItem = New EncodingItem(AppSettings.InputDefaultEncoding)
        _btnSetDefaultInputEncoding.Text = My.Resources.SetDefaultEncoding
        AddHandler _btnSetDefaultInputEncoding.Click, Sub(sender As Object, e As EventArgs) _cmbInputEncoding.SelectedItem = New EncodingItem(Input.DefaultEncoding)

    End Sub

    Private Sub SaveInputDefaultValues()

        If _loadedTabs.Contains(_tbpInputDefaultValues) Then
            Dim inputOptions As InputOptions = InputOptions.None
            For Each item In _lsvDefaultInputOptions.Items.Cast(Of ListViewItem).Where(Function(f) f.Checked)
                inputOptions = inputOptions Or DirectCast(item.Tag, InputOptions)
            Next
            My.Settings.DefaultInputOptions = inputOptions
            My.Settings.DefaultInputNewLine = DirectCast(_lstInputNewLine.SelectedValue, NewLineMode)
            Dim encodingItem = TryCast(_cmbInputEncoding.SelectedItem, EncodingItem)
            If encodingItem IsNot Nothing Then
                My.Settings.DefaultInputEncoding = encodingItem.Encoding.WebName
            End If
        End If

    End Sub

    Private Sub LoadSymbols()

        _symbols = New BindingList(Of SymbolItem) From {
            New SymbolItem(My.Resources.CarriageReturn, My.Settings.CarriageReturnSymbol, Symbols.DefaultCarriageReturn),
            New SymbolItem(My.Resources.Linefeed, My.Settings.LinefeedSymbol, Symbols.DefaultLinefeed),
            New SymbolItem(My.Resources.NoCapture, My.Settings.NoCaptureSymbol, Symbols.DefaultNoCapture),
            New SymbolItem(My.Resources.Tab, My.Settings.TabSymbol, Symbols.DefaultTab),
            New SymbolItem(My.Resources.Space, My.Settings.SpaceSymbol, Symbols.DefaultSpace)
        }

        _dgvSymbols.DataSource = _symbols
        _tbpSymbols.Controls.Add(_dgvSymbols)

    End Sub

    Private Sub SaveSymbols()

        If _loadedTabs.Contains(_tbpSymbols) Then
            For Each item In _symbols
                Select Case item.DefaultValue
                    Case Symbols.DefaultCarriageReturn
                        My.Settings.CarriageReturnSymbol = item.Value
                    Case Symbols.DefaultLinefeed
                        My.Settings.LinefeedSymbol = item.Value
                    Case Symbols.DefaultTab
                        My.Settings.TabSymbol = item.Value
                    Case Symbols.DefaultSpace
                        My.Settings.SpaceSymbol = item.Value
                    Case Symbols.DefaultNoCapture
                        My.Settings.NoCaptureSymbol = item.Value
                End Select
            Next
        End If

    End Sub

    Private Sub LoadFormats()

        _btnSetGroupDefaultFont.Text = My.Resources.SetDefaultFont
        _btnSetGroupDefaults.Text = My.Resources.SetGroupDefaults
        _btnSetGroupsDefaults.Text = My.Resources.SetGroupsDefaults
        _groups = App.Formats.Groups.ToClones().ToArray()
        _infos = App.Formats.Infos.Where(Function(f) f.Group IsNot Nothing).ToClones().ToArray()
        _cmbBackColor.LoadColors()
        _cmbForeColor.LoadColors()
        _dicGroups = _groups.GroupJoin(
                _infos,
                Function(g) g.Name,
                Function(i) i.Group.Name,
                Function(g, i) New With {.Group = g, .Infos = i}) _
            .ToDictionary(Function(f) f.Group, Function(g) g.Infos.ToArray())
        _cmbFontFamily.Items.AddRange(New InstalledFontCollection().Families.Select(Function(f) f.Name).ToArray())
        _cmbFontSize.Items.AddRange(Enumerable.Range(6, 19).Select(Function(f) f.ToString(CultureInfo.InvariantCulture)).ToArray())
        _lstFormatGroups.Items.AddRange(_dicGroups.Where(Function(f) f.Value.Length > 0).Select(Function(f) f.Key).OrderBy(Function(f) f.Text).ToArray())
        AddHandler _lstFormatGroups.SelectedIndexChanged, AddressOf FormatGroups_SelectedIndexChanged
        AddHandler _cmbFontFamily.SelectedIndexChanged, AddressOf FontFamily_SelectedIndexChanged
        AddHandler _cmbFontSize.SelectedIndexChanged, AddressOf FontSize_SelectedIndexChanged
        AddHandler _lstFormatInfos.SelectedIndexChanged, AddressOf FormatInfos_SelectedIndexChanged
        AddHandler _cmbBackColor.SelectedIndexChanged, AddressOf BackColor_SelectedIndexChanged
        AddHandler _btnBackColor.Click, AddressOf ChangeBackColor_Click
        AddHandler _cmbForeColor.SelectedIndexChanged, AddressOf ForeColor_SelectedIndexChanged
        AddHandler _btnForeColor.Click, AddressOf ChangeForeColor_Click
        AddHandler _cbxFontBold.CheckedChanged, AddressOf FontBold_CheckedChanged
        AddHandler _btnSetGroupDefaultFont.Click, AddressOf SetGroupDefaultFont_Click
        AddHandler _btnSetGroupDefaults.Click, AddressOf SetGroupDefaults_Click
        AddHandler _btnSetGroupsDefaults.Click, AddressOf SetGroupsDefaults_Click
        AddHandler _btnSetInfoDefaults.Click, AddressOf SetInfoDefaults_Click
        _lstFormatGroups.SelectedIndex = 0

    End Sub

    Private Sub SaveFormats()

        If _loadedTabs.Contains(_tbpFormats) Then
            For Each item In _groups.Join(
                    App.Formats.Groups,
                    Function(f) f.Name,
                    Function(g) g.Name,
                    Function(f, g) New With {.NewItem = f, .OldItem = g})
                item.OldItem.FontFamily = item.NewItem.FontFamily
                item.OldItem.FontSize = item.NewItem.FontSize
            Next
            For Each item In _infos.Join(
                    App.Formats.Infos,
                    Function(f) f.Name,
                    Function(g) g.Name,
                    Function(f, g) New With {.NewItem = f, .OldItem = g})
                item.OldItem.BackColor = item.NewItem.BackColor
                item.OldItem.ForeColor = item.NewItem.ForeColor
                item.OldItem.Bold = item.NewItem.Bold
            Next
        End If

    End Sub

    Private Shared Function GetDescription(value As TextAlignment) As String

        Select Case value
            Case TextAlignment.Left
                Return My.Resources.Left
            Case TextAlignment.Right
                Return My.Resources.Right
            Case Else
                Return value.ToString()
        End Select

    End Function

    Private Shared Function GetDescription(value As NewLineMode) As String

        Select Case value
            Case NewLineMode.CrLf
                Return My.Resources.CarriageReturnPlusLinefeed
            Case NewLineMode.Lf
                Return My.Resources.Linefeed
            Case Else
                Return value.ToString()
        End Select

    End Function

    Private Sub SetGroupDefaultFont_Click(sender As Object, e As EventArgs)

        If SelectedGroup IsNot Nothing AndAlso MessageDialog.Question(My.Resources.SetGroupDefaultFontMsg) = System.Windows.Forms.DialogResult.Yes Then
            App.Formats.SetDefaults(SelectedGroup)
            ResetGroupIndex()
        End If

    End Sub

    Private Sub SetGroupDefaults_Click(sender As Object, e As EventArgs)

        If SelectedGroup IsNot Nothing AndAlso MessageDialog.Question(My.Resources.SetGroupDefaultsMsg) = System.Windows.Forms.DialogResult.Yes Then
            App.Formats.SetDefaults(SelectedGroup)
            App.Formats.SetDefaults(_dicGroups(SelectedGroup))
            ResetGroupIndex()
        End If

    End Sub

    Private Sub SetGroupsDefaults_Click(sender As Object, e As EventArgs)

        If MessageDialog.Question(My.Resources.SetGroupsDefaultsMsg) = System.Windows.Forms.DialogResult.Yes Then
            App.Formats.SetDefaults(_dicGroups.Select(Function(f) f.Key))
            ResetGroupIndex()
        End If

    End Sub

    Private Sub SetInfoDefaults_Click(sender As Object, e As EventArgs)

        If SelectedInfo IsNot Nothing AndAlso MessageDialog.Question(My.Resources.SetItemDefaultsMsg) = System.Windows.Forms.DialogResult.Yes Then
            App.Formats.SetDefaults(SelectedInfo)
            Dim infoIndex = _lstFormatInfos.SelectedIndex
            _lstFormatInfos.SelectedIndex = -1
            _lstFormatInfos.SelectedIndex = infoIndex
        End If

    End Sub

    Private Sub ResetGroupIndex()

        Dim groupIndex = _lstFormatGroups.SelectedIndex
        Dim infoIndex = _lstFormatInfos.SelectedIndex
        _lstFormatGroups.SelectedIndex = -1
        _lstFormatGroups.SelectedIndex = groupIndex
        _lstFormatInfos.SelectedIndex = infoIndex

    End Sub

    Private Sub FormatGroups_SelectedIndexChanged(sender As Object, e As EventArgs)

        _btnSetGroupDefaults.Enabled = SelectedGroup IsNot Nothing
        _btnSetGroupDefaultFont.Enabled = SelectedGroup IsNot Nothing
        _lstFormatInfos.Items.Clear()
        If SelectedGroup IsNot Nothing Then
            _cmbFontFamily.SelectedItem = SelectedGroup.FontFamily.Name
            _cmbFontSize.SelectedItem = SelectedGroup.FontSize.ToString(CultureInfo.InvariantCulture)
            _lstFormatInfos.Items.AddRange(_dicGroups(SelectedGroup).OrderBy(Function(f) f.Text).ToArray())
            If _lstFormatInfos.Items.Count > 0 Then
                _lstFormatInfos.SelectedIndex = 0
            End If
        Else
            _cmbFontFamily.SelectedIndex = 0
            _cmbFontSize.SelectedIndex = 0
        End If
        If _lstFormatInfos.SelectedIndex = -1 Then
            _cmbBackColor.Enabled = False
            _cmbForeColor.Enabled = False
            _cbxFontBold.Enabled = False
            _btnBackColor.Enabled = False
            _btnForeColor.Enabled = False
        End If
        _btnSetInfoDefaults.Enabled = SelectedInfo IsNot Nothing

    End Sub

    Private Sub FontFamily_SelectedIndexChanged(sender As Object, e As EventArgs)

        If SelectedGroup IsNot Nothing AndAlso SelectedFontFamily IsNot Nothing Then
            SelectedGroup.FontFamily = SelectedFontFamily
        End If
        LoadFormatSample()

    End Sub

    Private Sub FontSize_SelectedIndexChanged(sender As Object, e As EventArgs)

        If SelectedGroup IsNot Nothing AndAlso SelectedFontSize > 0 Then
            SelectedGroup.FontSize = SelectedFontSize
        End If
        LoadFormatSample()

    End Sub

    Private Sub FormatInfos_SelectedIndexChanged(sender As Object, e As EventArgs)

        _cmbBackColor.Enabled = False
        _cmbForeColor.Enabled = False
        _cbxFontBold.Enabled = False
        _btnBackColor.Enabled = False
        _btnForeColor.Enabled = False
        _btnSetInfoDefaults.Enabled = SelectedInfo IsNot Nothing
        If SelectedInfo IsNot Nothing Then
            _cmbBackColor.SelectedColor = SelectedInfo.BackColor
            _cmbForeColor.SelectedColor = SelectedInfo.ForeColor
            _cmbBackColor.Enabled = SelectedInfo.BackColorEnabled
            _btnBackColor.Enabled = SelectedInfo.BackColorEnabled
            _cmbForeColor.Enabled = SelectedInfo.ForeColorEnabled
            _btnForeColor.Enabled = SelectedInfo.ForeColorEnabled
            _cbxFontBold.Checked = SelectedInfo.Bold
            _cbxFontBold.Enabled = SelectedInfo.BoldEnabled
        Else
            _cmbBackColor.SelectedIndex = 0
            _cmbForeColor.SelectedIndex = 0
            _cbxFontBold.Checked = False
        End If
        LoadFormatSample()

    End Sub

    Private Sub BackColor_SelectedIndexChanged(sender As Object, e As EventArgs)

        If _loadingColors = False Then
            If SelectedInfo IsNot Nothing AndAlso _cmbBackColor.SelectedColor <> Color.Empty Then
                SelectedInfo.BackColor = _cmbBackColor.SelectedColor
            End If
            LoadFormatSample()
        End If

    End Sub

    Private Sub ForeColor_SelectedIndexChanged(sender As Object, e As EventArgs)

        If _loadingColors = False Then
            If SelectedInfo IsNot Nothing AndAlso _cmbForeColor.SelectedColor <> Color.Empty Then
                SelectedInfo.ForeColor = _cmbForeColor.SelectedColor
            End If
            LoadFormatSample()
        End If

    End Sub

    Private Sub FontBold_CheckedChanged(sender As Object, e As EventArgs)

        If SelectedInfo IsNot Nothing Then
            SelectedInfo.Bold = _cbxFontBold.Checked
        End If
        LoadFormatSample()

    End Sub

    Private Sub LoadFormatSample()

        _tbxSample.Clear()
        _tbxSample.BackColor = SystemColors.Control
        _tbxSample.ForeColor = SystemColors.WindowText
        Dim fontFamily = SelectedFontFamily
        Dim fontSize = SelectedFontSize
        If fontFamily IsNot Nothing AndAlso fontSize > 0 Then
            _tbxSample.Text = My.Resources.Regexator
            If _cmbBackColor.SelectedColor <> Color.Empty Then
                _tbxSample.BackColor = _cmbBackColor.SelectedColor
            End If
            If _cmbForeColor.SelectedColor <> Color.Empty Then
                _tbxSample.ForeColor = _cmbForeColor.SelectedColor
            End If
            _tbxSample.Font = New Font(fontFamily, fontSize, If(_cbxFontBold.Checked, FontStyle.Bold, FontStyle.Regular))
        End If

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        If DialogResult = System.Windows.Forms.DialogResult.OK Then
            SaveApplication()
            SaveTextEditor()
            SaveProjectExplorer()
            SaveOutput()
            SaveFileSystemSearch()
            SaveExport()
            SaveRegexOptions()
            SaveProjectDefaultValues()
            SaveInputDefaultValues()
            SaveSymbols()
            SaveFormats()
            SaveSnippets()
            My.Settings.Save()
        End If
        MyBase.OnFormClosing(e)

    End Sub

    Private Sub ChangeBackColor_Click(sender As Object, e As EventArgs)

        Using dlg As New ColorPicker() With {.SelectedColor = _cmbBackColor.SelectedColor, .Icon = My.Resources.IcoRegexator, .Text = My.Resources.ColorPicker}
            Dim sorterKey As String = GetSorterKey()
            dlg.SorterKey = sorterKey
            If dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                _cmbBackColor.SelectedColor = dlg.SelectedColor
            End If
            If sorterKey <> dlg.SorterKey Then
                _colorSortMode = GetSortMode(dlg.SorterKey)
                ReloadColorComboBoxes()
            End If
        End Using

    End Sub

    Private Sub ChangeForeColor_Click(sender As Object, e As EventArgs)

        Using dlg As New ColorPicker() With {.SelectedColor = _cmbForeColor.SelectedColor, .Icon = My.Resources.IcoRegexator, .Text = My.Resources.ColorPicker}
            Dim sorterKey As String = GetSorterKey()
            dlg.SorterKey = sorterKey
            If dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                _cmbForeColor.SelectedColor = dlg.SelectedColor
            End If
            If sorterKey <> dlg.SorterKey Then
                _colorSortMode = GetSortMode(dlg.SorterKey)
                ReloadColorComboBoxes()
            End If
        End Using

    End Sub

    Private Function GetSorterKey() As String

        Select Case _colorSortMode
            Case ColorSortMode.Brightness
                Return "Brightness"
            Case ColorSortMode.Hue
                Return "Hue"
        End Select

        Return "Name"

    End Function

    <SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")>
    Private Function GetSortMode(sorterKey As String) As ColorSortMode

        Select Case sorterKey
            Case "Brightness"
                Return ColorSortMode.Brightness
            Case "Hue"
                Return ColorSortMode.Hue
        End Select

        Return ColorSortMode.Name

    End Function

    Private Sub ReloadColorComboBoxes()

        _loadingColors = True
        _cmbBackColor.LoadColors(_colorSortMode)
        _cmbForeColor.LoadColors(_colorSortMode)
        _loadingColors = False

    End Sub

    Private ReadOnly Property SelectedGroup As FormatGroup
        Get
            Return If(_lstFormatGroups.SelectedItem IsNot Nothing, DirectCast(_lstFormatGroups.SelectedItem, FormatGroup), Nothing)
        End Get
    End Property

    Private ReadOnly Property SelectedInfo As FormatInfo
        Get
            Return If(_lstFormatInfos.SelectedItem IsNot Nothing, DirectCast(_lstFormatInfos.SelectedItem, FormatInfo), Nothing)
        End Get
    End Property

    Private ReadOnly Property SelectedFontFamily As FontFamily
        Get
            Return If(_cmbFontFamily.SelectedItem IsNot Nothing, New FontFamily(_cmbFontFamily.SelectedItem.ToString()), Nothing)
        End Get
    End Property

    Private ReadOnly Property SelectedFontSize As Integer
        Get
            Return If(_cmbFontSize.SelectedItem IsNot Nothing, CInt(_cmbFontSize.SelectedItem), 0)
        End Get
    End Property

    Private _fSetDefaultSnippets As Boolean
    Private _lsvRegexOptionsVisibility As OptionsListView
    Private _lsvDefaultRegexOptions As OptionsListView
    Private _lsvDefaultInputOptions As OptionsListView
    Private _lsvDefaultOutputOptions As OptionsListView
    Private _lsvDefaultReplacementOptions As OptionsListView
    Private _lsvDefaultPatternOptions As OptionsListView
    Private _lsvHiddenCategories As OptionsListView
    Private ReadOnly _dgvSymbols As New SymbolGrid() With {.Margin = New Padding(0)}
    Private _symbols As BindingList(Of SymbolItem)
    Private _groups As FormatGroup()
    Private _infos As FormatInfo()
    Private _dicGroups As New Dictionary(Of FormatGroup, FormatInfo())
    Private _loadingColors As Boolean
    Private _colorSortMode As ColorSortMode = ColorSortMode.Hue
    Private _dgvSnippetDirectories As SnippetDirectoryDataGridView

    Private ReadOnly _loadedTabs As New HashSet(Of TabPage)
    Private ReadOnly _loaders As New Dictionary(Of TabPage, Action)
    Private ReadOnly _initialTab As OptionsFormTab
    Private _snippetDirectories As String()

    Private Class OptionsListView
        Inherits ExtendedListView

        Public Sub New()

            FullRowSelect = True
            HideSelection = False
            HeaderStyle = ColumnHeaderStyle.None
            CheckBoxes = True
            Columns.Add(String.Empty, -1)
            Dock = DockStyle.Fill
            Dim cms = New ContextMenuStrip()
            cms.Items.Add(My.Resources.CheckAll, Nothing, AddressOf CheckAll_Click)
            cms.Items.Add(My.Resources.CheckNone, Nothing, AddressOf CheckNone_Click)
            ContextMenuStrip = cms

        End Sub

        Private Sub CheckAll_Click(sender As Object, e As EventArgs)

            For Each item As ListViewItem In Items
                item.Checked = True
            Next

        End Sub

        Private Sub CheckNone_Click(sender As Object, e As EventArgs)

            For Each item As ListViewItem In Items
                item.Checked = False
            Next

        End Sub

    End Class

    Private Class SymbolGrid
        Inherits ExtendedDataGridView

        Public Sub New()

            Dock = DockStyle.Fill
            Me.ReadOnly = False
            MultiSelect = False
            AllowUserToAddRows = False
            AllowUserToDeleteRows = False
            EnableHeadersVisualStyles = True
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable
            SelectionMode = DataGridViewSelectionMode.CellSelect
            AutoGenerateColumns = False
            Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Name", .DataPropertyName = "Name", .HeaderText = My.Resources.Name, .ReadOnly = True, .SortMode = DataGridViewColumnSortMode.NotSortable})
            Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Value", .DataPropertyName = "Value", .HeaderText = My.Resources.Value, .SortMode = DataGridViewColumnSortMode.NotSortable})
            Columns.Add(New DataGridViewButtonColumn() With {.Name = "SetDefault", .HeaderText = My.Resources.Default_, .Text = My.Resources.Default_, .UseColumnTextForButtonValue = True})

        End Sub

        Protected Overrides Sub OnCellEndEdit(e As DataGridViewCellEventArgs)

            If e Is Nothing Then Throw New ArgumentNullException("e")
            Dim cell = Me.Item(e.ColumnIndex, e.RowIndex)
            Dim item = DirectCast(cell.OwningRow.DataBoundItem, SymbolItem)
            If cell.Value Is Nothing Then
                cell.Value = item.DefaultValue
            End If
            MyBase.OnCellEndEdit(e)

        End Sub

        Protected Overrides Sub OnCurrentCellDirtyStateChanged(e As EventArgs)

            If CurrentCell IsNot Nothing AndAlso CurrentCell.OwningColumn.DataPropertyName = "IsDefault" Then
                CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
            MyBase.OnCurrentCellDirtyStateChanged(e)

        End Sub

        Protected Overrides Sub OnCellClick(e As DataGridViewCellEventArgs)

            If e Is Nothing Then Throw New ArgumentNullException("e")
            If e.ColumnIndex = 2 AndAlso e.RowIndex <> -1 Then
                Dim item = DirectCast(Rows(e.RowIndex).DataBoundItem, SymbolItem)
                item.Value = item.DefaultValue
            End If
            MyBase.OnCellClick(e)

        End Sub

    End Class

    Private Class SymbolItem
        Implements INotifyPropertyChanged

        Public Sub New(name As String, value As String, defaultValue As String)

            If name Is Nothing Then Throw New ArgumentNullException("name")
            If defaultValue Is Nothing Then Throw New ArgumentNullException("defaultValue")
            Me.Name = name
            Me.Value = If(String.IsNullOrEmpty(value), defaultValue, value)
            _defaultValue = defaultValue

        End Sub

        <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                If _name <> value Then
                    _name = value
                    NotifyPropertyChanged("Name")
                End If
            End Set
        End Property

        Public Property Value As String
            Get
                Return _value
            End Get
            Set(value As String)
                If _value <> value Then
                    _value = value
                    NotifyPropertyChanged("Value")
                End If
            End Set
        End Property

        Public ReadOnly Property DefaultValue As String
            Get
                Return _defaultValue
            End Get
        End Property

        Private _name As String
        Private _value As String
        Private ReadOnly _defaultValue As String

        Private Sub NotifyPropertyChanged(propertyName As String)

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))

        End Sub

        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    End Class

End Class