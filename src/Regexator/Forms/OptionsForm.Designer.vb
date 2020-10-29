<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OptionsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me._btnCancel = New System.Windows.Forms.Button()
        Me._btnOk = New System.Windows.Forms.Button()
        Me._tbc = New System.Windows.Forms.TabControl()
        Me._tbpApplication = New System.Windows.Forms.TabPage()
        Me._cbxShowStatusBar = New System.Windows.Forms.CheckBox()
        Me._cbxCheckNewVersion = New System.Windows.Forms.CheckBox()
        Me._numRecentMaxCount = New System.Windows.Forms.NumericUpDown()
        Me._lblRecentItemsMaxCount = New System.Windows.Forms.Label()
        Me._lblMatchTimeout = New System.Windows.Forms.Label()
        Me._numMatchTimeout = New System.Windows.Forms.NumericUpDown()
        Me._tbpTextEditor = New System.Windows.Forms.TabPage()
        Me._cbxSelectEntireCurrentLineAfterLoad = New System.Windows.Forms.CheckBox()
        Me._cbxPatternIndentNewLine = New System.Windows.Forms.CheckBox()
        Me._tbpFormats = New System.Windows.Forms.TabPage()
        Me._btnSetGroupDefaultFont = New System.Windows.Forms.Button()
        Me._btnSetInfoDefaults = New System.Windows.Forms.Button()
        Me._btnSetGroupsDefaults = New System.Windows.Forms.Button()
        Me._btnSetGroupDefaults = New System.Windows.Forms.Button()
        Me._lstFormatGroups = New System.Windows.Forms.ListBox()
        Me._tbxSample = New System.Windows.Forms.TextBox()
        Me._lblSample = New System.Windows.Forms.Label()
        Me._cbxFontBold = New System.Windows.Forms.CheckBox()
        Me._btnForeColor = New System.Windows.Forms.Button()
        Me._btnBackColor = New System.Windows.Forms.Button()
        Me._lblForeColor = New System.Windows.Forms.Label()
        Me._lblBackColor = New System.Windows.Forms.Label()
        Me._lblItem = New System.Windows.Forms.Label()
        Me._cmbForeColor = New Regexator.Windows.Forms.ColorComboBox()
        Me._cmbBackColor = New Regexator.Windows.Forms.ColorComboBox()
        Me._lstFormatInfos = New System.Windows.Forms.ListBox()
        Me._cmbFontSize = New System.Windows.Forms.ComboBox()
        Me._lblFontSize = New System.Windows.Forms.Label()
        Me._cmbFontFamily = New System.Windows.Forms.ComboBox()
        Me._lblFont = New System.Windows.Forms.Label()
        Me._lblGroup = New System.Windows.Forms.Label()
        Me._tbpProjectExplorer = New System.Windows.Forms.TabPage()
        Me._cbxConfirmFileInputRemoval = New System.Windows.Forms.CheckBox()
        Me._cbxTrackActiveItem = New System.Windows.Forms.CheckBox()
        Me._cbxShowHiddenFiles = New System.Windows.Forms.CheckBox()
        Me._cbxUseRecycleBin = New System.Windows.Forms.CheckBox()
        Me._tbpSnippets = New System.Windows.Forms.TabPage()
        Me._cbxSetSnippetOptions = New System.Windows.Forms.CheckBox()
        Me._gbxSnippetDirectories = New System.Windows.Forms.GroupBox()
        Me._pnlSnippetDirectories = New System.Windows.Forms.Panel()
        Me._btnViewSnippetErrorLog = New System.Windows.Forms.Button()
        Me._lblHiddenCategoriesInfo = New System.Windows.Forms.Label()
        Me._btnSetDefaultFavoriteSnippets = New System.Windows.Forms.Button()
        Me._gbxHiddenCategories = New System.Windows.Forms.GroupBox()
        Me._tbpOutput = New System.Windows.Forms.TabPage()
        Me._gbxOutputText = New System.Windows.Forms.GroupBox()
        Me._cbxHighlightBeforeAfterResult = New System.Windows.Forms.CheckBox()
        Me._cbxOutputOmitRepeatedInfo = New System.Windows.Forms.CheckBox()
        Me._cmbNumberAlignment = New System.Windows.Forms.ComboBox()
        Me._lblNumberAlignment = New System.Windows.Forms.Label()
        Me._numOutputLimit = New System.Windows.Forms.NumericUpDown()
        Me._lblOutputLimit = New System.Windows.Forms.Label()
        Me._tbpFileSystemSearch = New System.Windows.Forms.TabPage()
        Me._numFileSystemSearchMaxResultCount = New System.Windows.Forms.NumericUpDown()
        Me._lblFileSystemSearchMaxResultCount = New System.Windows.Forms.Label()
        Me._tbpSymbols = New System.Windows.Forms.TabPage()
        Me._tbpExport = New System.Windows.Forms.TabPage()
        Me._cbxMultiline = New System.Windows.Forms.CheckBox()
        Me._lblDefaultExportMode = New System.Windows.Forms.Label()
        Me._cmbExportMode = New System.Windows.Forms.ComboBox()
        Me._tbpRegexOptions = New System.Windows.Forms.TabPage()
        Me._cbxRegexOptionsDescriptionVisible = New System.Windows.Forms.CheckBox()
        Me._gbxRegexOptionsVisibility = New System.Windows.Forms.GroupBox()
        Me._cbxRegexOptionsHotkeyNumberVisible = New System.Windows.Forms.CheckBox()
        Me._tbpProjectDefaultValues = New System.Windows.Forms.TabPage()
        Me._pnlDefaultValues = New System.Windows.Forms.Panel()
        Me._tbcProjectDefaultValues = New System.Windows.Forms.TabControl()
        Me._tbpPatternDefaults = New System.Windows.Forms.TabPage()
        Me._gbxDefaultRegexOptions = New System.Windows.Forms.GroupBox()
        Me._gbxDefaultPatternOptions = New System.Windows.Forms.GroupBox()
        Me._tbpReplacementDefaults = New System.Windows.Forms.TabPage()
        Me._gbxReplacementText = New System.Windows.Forms.GroupBox()
        Me._tbxReplacementText = New Regexator.Windows.Forms.ExtendedTextBox()
        Me._gbxReplacementNewLine = New System.Windows.Forms.GroupBox()
        Me._lstReplacementNewLine = New System.Windows.Forms.ListBox()
        Me._gbxDefaultReplacementOptions = New System.Windows.Forms.GroupBox()
        Me._tbpOutputDefaults = New System.Windows.Forms.TabPage()
        Me._gbxDefaultOutputOptions = New System.Windows.Forms.GroupBox()
        Me._tbpInputDefaultValues = New System.Windows.Forms.TabPage()
        Me._gbxInputEncoding = New System.Windows.Forms.GroupBox()
        Me._cmbInputEncoding = New System.Windows.Forms.ComboBox()
        Me._btnSetDefaultInputEncoding = New System.Windows.Forms.Button()
        Me._gbxInputNewLine = New System.Windows.Forms.GroupBox()
        Me._lstInputNewLine = New System.Windows.Forms.ListBox()
        Me._gbxDefaultInputOptions = New System.Windows.Forms.GroupBox()
        Me._tbc.SuspendLayout()
        Me._tbpApplication.SuspendLayout()
        CType(Me._numRecentMaxCount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._numMatchTimeout, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._tbpTextEditor.SuspendLayout()
        Me._tbpFormats.SuspendLayout()
        Me._tbpProjectExplorer.SuspendLayout()
        Me._tbpSnippets.SuspendLayout()
        Me._gbxSnippetDirectories.SuspendLayout()
        Me._tbpOutput.SuspendLayout()
        Me._gbxOutputText.SuspendLayout()
        CType(Me._numOutputLimit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._tbpFileSystemSearch.SuspendLayout()
        CType(Me._numFileSystemSearchMaxResultCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._tbpExport.SuspendLayout()
        Me._tbpRegexOptions.SuspendLayout()
        Me._tbpProjectDefaultValues.SuspendLayout()
        Me._pnlDefaultValues.SuspendLayout()
        Me._tbcProjectDefaultValues.SuspendLayout()
        Me._tbpPatternDefaults.SuspendLayout()
        Me._tbpReplacementDefaults.SuspendLayout()
        Me._gbxReplacementText.SuspendLayout()
        Me._gbxReplacementNewLine.SuspendLayout()
        Me._tbpOutputDefaults.SuspendLayout()
        Me._tbpInputDefaultValues.SuspendLayout()
        Me._gbxInputEncoding.SuspendLayout()
        Me._gbxInputNewLine.SuspendLayout()
        Me.SuspendLayout()
        '
        '_btnCancel
        '
        Me._btnCancel.Location = New System.Drawing.Point(669, 512)
        Me._btnCancel.Margin = New System.Windows.Forms.Padding(6)
        Me._btnCancel.Name = "_btnCancel"
        Me._btnCancel.Size = New System.Drawing.Size(100, 30)
        Me._btnCancel.TabIndex = 2
        Me._btnCancel.Text = Global.Regexator.My.Resources.Resources.Cancel
        Me._btnCancel.UseVisualStyleBackColor = True
        '
        '_btnOk
        '
        Me._btnOk.Location = New System.Drawing.Point(557, 512)
        Me._btnOk.Margin = New System.Windows.Forms.Padding(6)
        Me._btnOk.Name = "_btnOk"
        Me._btnOk.Size = New System.Drawing.Size(100, 30)
        Me._btnOk.TabIndex = 1
        Me._btnOk.Text = "OK"
        Me._btnOk.UseVisualStyleBackColor = True
        '
        '_tbc
        '
        Me._tbc.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
        Me._tbc.Controls.Add(Me._tbpApplication)
        Me._tbc.Controls.Add(Me._tbpTextEditor)
        Me._tbc.Controls.Add(Me._tbpFormats)
        Me._tbc.Controls.Add(Me._tbpProjectExplorer)
        Me._tbc.Controls.Add(Me._tbpSnippets)
        Me._tbc.Controls.Add(Me._tbpOutput)
        Me._tbc.Controls.Add(Me._tbpFileSystemSearch)
        Me._tbc.Controls.Add(Me._tbpSymbols)
        Me._tbc.Controls.Add(Me._tbpExport)
        Me._tbc.Controls.Add(Me._tbpRegexOptions)
        Me._tbc.Controls.Add(Me._tbpProjectDefaultValues)
        Me._tbc.Controls.Add(Me._tbpInputDefaultValues)
        Me._tbc.Location = New System.Drawing.Point(12, 12)
        Me._tbc.Multiline = True
        Me._tbc.Name = "_tbc"
        Me._tbc.SelectedIndex = 0
        Me._tbc.Size = New System.Drawing.Size(757, 491)
        Me._tbc.TabIndex = 0
        '
        '_tbpApplication
        '
        Me._tbpApplication.Controls.Add(Me._cbxShowStatusBar)
        Me._tbpApplication.Controls.Add(Me._cbxCheckNewVersion)
        Me._tbpApplication.Controls.Add(Me._numRecentMaxCount)
        Me._tbpApplication.Controls.Add(Me._lblRecentItemsMaxCount)
        Me._tbpApplication.Controls.Add(Me._lblMatchTimeout)
        Me._tbpApplication.Controls.Add(Me._numMatchTimeout)
        Me._tbpApplication.Location = New System.Drawing.Point(4, 53)
        Me._tbpApplication.Name = "_tbpApplication"
        Me._tbpApplication.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpApplication.Size = New System.Drawing.Size(749, 434)
        Me._tbpApplication.TabIndex = 0
        Me._tbpApplication.Text = "Application"
        Me._tbpApplication.UseVisualStyleBackColor = True
        '
        '_cbxShowStatusBar
        '
        Me._cbxShowStatusBar.AutoSize = True
        Me._cbxShowStatusBar.Location = New System.Drawing.Point(29, 80)
        Me._cbxShowStatusBar.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxShowStatusBar.Name = "_cbxShowStatusBar"
        Me._cbxShowStatusBar.Size = New System.Drawing.Size(109, 19)
        Me._cbxShowStatusBar.TabIndex = 4
        Me._cbxShowStatusBar.Text = "Show status bar"
        Me._cbxShowStatusBar.UseVisualStyleBackColor = True
        '
        '_cbxCheckNewVersion
        '
        Me._cbxCheckNewVersion.AutoSize = True
        Me._cbxCheckNewVersion.Location = New System.Drawing.Point(29, 111)
        Me._cbxCheckNewVersion.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxCheckNewVersion.Name = "_cbxCheckNewVersion"
        Me._cbxCheckNewVersion.Size = New System.Drawing.Size(122, 19)
        Me._cbxCheckNewVersion.TabIndex = 5
        Me._cbxCheckNewVersion.Text = "Check for updates"
        Me._cbxCheckNewVersion.UseVisualStyleBackColor = True
        '
        '_numRecentMaxCount
        '
        Me._numRecentMaxCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._numRecentMaxCount.Location = New System.Drawing.Point(648, 51)
        Me._numRecentMaxCount.Margin = New System.Windows.Forms.Padding(10, 6, 6, 6)
        Me._numRecentMaxCount.Name = "_numRecentMaxCount"
        Me._numRecentMaxCount.Size = New System.Drawing.Size(75, 23)
        Me._numRecentMaxCount.TabIndex = 3
        Me._numRecentMaxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        '_lblRecentItemsMaxCount
        '
        Me._lblRecentItemsMaxCount.AutoSize = True
        Me._lblRecentItemsMaxCount.Location = New System.Drawing.Point(26, 53)
        Me._lblRecentItemsMaxCount.Margin = New System.Windows.Forms.Padding(6, 6, 50, 6)
        Me._lblRecentItemsMaxCount.Name = "_lblRecentItemsMaxCount"
        Me._lblRecentItemsMaxCount.Size = New System.Drawing.Size(182, 15)
        Me._lblRecentItemsMaxCount.TabIndex = 2
        Me._lblRecentItemsMaxCount.Text = "Maximal number of recent items:"
        '
        '_lblMatchTimeout
        '
        Me._lblMatchTimeout.AutoSize = True
        Me._lblMatchTimeout.Location = New System.Drawing.Point(26, 26)
        Me._lblMatchTimeout.Margin = New System.Windows.Forms.Padding(6, 6, 100, 6)
        Me._lblMatchTimeout.Name = "_lblMatchTimeout"
        Me._lblMatchTimeout.Size = New System.Drawing.Size(540, 15)
        Me._lblMatchTimeout.TabIndex = 0
        Me._lblMatchTimeout.Text = "Regular expression match time out (in seconds, application restart needed for cha" & _
    "nges to take effect):"
        '
        '_numMatchTimeout
        '
        Me._numMatchTimeout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._numMatchTimeout.Location = New System.Drawing.Point(648, 24)
        Me._numMatchTimeout.Margin = New System.Windows.Forms.Padding(10, 6, 6, 6)
        Me._numMatchTimeout.Name = "_numMatchTimeout"
        Me._numMatchTimeout.Size = New System.Drawing.Size(75, 23)
        Me._numMatchTimeout.TabIndex = 1
        Me._numMatchTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        '_tbpTextEditor
        '
        Me._tbpTextEditor.Controls.Add(Me._cbxSelectEntireCurrentLineAfterLoad)
        Me._tbpTextEditor.Controls.Add(Me._cbxPatternIndentNewLine)
        Me._tbpTextEditor.Location = New System.Drawing.Point(4, 53)
        Me._tbpTextEditor.Name = "_tbpTextEditor"
        Me._tbpTextEditor.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpTextEditor.Size = New System.Drawing.Size(749, 434)
        Me._tbpTextEditor.TabIndex = 13
        Me._tbpTextEditor.Text = "Text Editor"
        Me._tbpTextEditor.UseVisualStyleBackColor = True
        '
        '_cbxSelectEntireCurrentLineAfterLoad
        '
        Me._cbxSelectEntireCurrentLineAfterLoad.AutoSize = True
        Me._cbxSelectEntireCurrentLineAfterLoad.Location = New System.Drawing.Point(26, 57)
        Me._cbxSelectEntireCurrentLineAfterLoad.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxSelectEntireCurrentLineAfterLoad.Name = "_cbxSelectEntireCurrentLineAfterLoad"
        Me._cbxSelectEntireCurrentLineAfterLoad.Size = New System.Drawing.Size(206, 19)
        Me._cbxSelectEntireCurrentLineAfterLoad.TabIndex = 1
        Me._cbxSelectEntireCurrentLineAfterLoad.Text = "Select entire current line after load"
        Me._cbxSelectEntireCurrentLineAfterLoad.UseVisualStyleBackColor = True
        '
        '_cbxPatternIndentNewLine
        '
        Me._cbxPatternIndentNewLine.AutoSize = True
        Me._cbxPatternIndentNewLine.Location = New System.Drawing.Point(26, 26)
        Me._cbxPatternIndentNewLine.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxPatternIndentNewLine.Name = "_cbxPatternIndentNewLine"
        Me._cbxPatternIndentNewLine.Size = New System.Drawing.Size(216, 19)
        Me._cbxPatternIndentNewLine.TabIndex = 0
        Me._cbxPatternIndentNewLine.Text = "Indent new line (pattern editor only)"
        Me._cbxPatternIndentNewLine.UseVisualStyleBackColor = True
        '
        '_tbpFormats
        '
        Me._tbpFormats.Controls.Add(Me._btnSetGroupDefaultFont)
        Me._tbpFormats.Controls.Add(Me._btnSetInfoDefaults)
        Me._tbpFormats.Controls.Add(Me._btnSetGroupsDefaults)
        Me._tbpFormats.Controls.Add(Me._btnSetGroupDefaults)
        Me._tbpFormats.Controls.Add(Me._lstFormatGroups)
        Me._tbpFormats.Controls.Add(Me._tbxSample)
        Me._tbpFormats.Controls.Add(Me._lblSample)
        Me._tbpFormats.Controls.Add(Me._cbxFontBold)
        Me._tbpFormats.Controls.Add(Me._btnForeColor)
        Me._tbpFormats.Controls.Add(Me._btnBackColor)
        Me._tbpFormats.Controls.Add(Me._lblForeColor)
        Me._tbpFormats.Controls.Add(Me._lblBackColor)
        Me._tbpFormats.Controls.Add(Me._lblItem)
        Me._tbpFormats.Controls.Add(Me._cmbForeColor)
        Me._tbpFormats.Controls.Add(Me._cmbBackColor)
        Me._tbpFormats.Controls.Add(Me._lstFormatInfos)
        Me._tbpFormats.Controls.Add(Me._cmbFontSize)
        Me._tbpFormats.Controls.Add(Me._lblFontSize)
        Me._tbpFormats.Controls.Add(Me._cmbFontFamily)
        Me._tbpFormats.Controls.Add(Me._lblFont)
        Me._tbpFormats.Controls.Add(Me._lblGroup)
        Me._tbpFormats.Location = New System.Drawing.Point(4, 53)
        Me._tbpFormats.Name = "_tbpFormats"
        Me._tbpFormats.Padding = New System.Windows.Forms.Padding(10)
        Me._tbpFormats.Size = New System.Drawing.Size(749, 434)
        Me._tbpFormats.TabIndex = 8
        Me._tbpFormats.Text = "Fonts and Colors"
        Me._tbpFormats.UseVisualStyleBackColor = True
        '
        '_btnSetGroupDefaultFont
        '
        Me._btnSetGroupDefaultFont.Enabled = False
        Me._btnSetGroupDefaultFont.Location = New System.Drawing.Point(244, 74)
        Me._btnSetGroupDefaultFont.Margin = New System.Windows.Forms.Padding(6)
        Me._btnSetGroupDefaultFont.Name = "_btnSetGroupDefaultFont"
        Me._btnSetGroupDefaultFont.Size = New System.Drawing.Size(150, 30)
        Me._btnSetGroupDefaultFont.TabIndex = 6
        Me._btnSetGroupDefaultFont.Text = "Set Default Font"
        Me._btnSetGroupDefaultFont.UseVisualStyleBackColor = True
        '
        '_btnSetInfoDefaults
        '
        Me._btnSetInfoDefaults.Enabled = False
        Me._btnSetInfoDefaults.Location = New System.Drawing.Point(244, 386)
        Me._btnSetInfoDefaults.Margin = New System.Windows.Forms.Padding(6)
        Me._btnSetInfoDefaults.Name = "_btnSetInfoDefaults"
        Me._btnSetInfoDefaults.Size = New System.Drawing.Size(150, 30)
        Me._btnSetInfoDefaults.TabIndex = 18
        Me._btnSetInfoDefaults.Text = "Set Item Defaults"
        Me._btnSetInfoDefaults.UseVisualStyleBackColor = True
        '
        '_btnSetGroupsDefaults
        '
        Me._btnSetGroupsDefaults.Location = New System.Drawing.Point(244, 158)
        Me._btnSetGroupsDefaults.Margin = New System.Windows.Forms.Padding(6)
        Me._btnSetGroupsDefaults.Name = "_btnSetGroupsDefaults"
        Me._btnSetGroupsDefaults.Size = New System.Drawing.Size(150, 30)
        Me._btnSetGroupsDefaults.TabIndex = 8
        Me._btnSetGroupsDefaults.Text = "Set Groups Defaults"
        Me._btnSetGroupsDefaults.UseVisualStyleBackColor = True
        '
        '_btnSetGroupDefaults
        '
        Me._btnSetGroupDefaults.Enabled = False
        Me._btnSetGroupDefaults.Location = New System.Drawing.Point(244, 116)
        Me._btnSetGroupDefaults.Margin = New System.Windows.Forms.Padding(6)
        Me._btnSetGroupDefaults.Name = "_btnSetGroupDefaults"
        Me._btnSetGroupDefaults.Size = New System.Drawing.Size(150, 30)
        Me._btnSetGroupDefaults.TabIndex = 7
        Me._btnSetGroupDefaults.Text = "Set Group Defaults"
        Me._btnSetGroupDefaults.UseVisualStyleBackColor = True
        '
        '_lstFormatGroups
        '
        Me._lstFormatGroups.FormattingEnabled = True
        Me._lstFormatGroups.ItemHeight = 15
        Me._lstFormatGroups.Location = New System.Drawing.Point(16, 34)
        Me._lstFormatGroups.Margin = New System.Windows.Forms.Padding(5)
        Me._lstFormatGroups.Name = "_lstFormatGroups"
        Me._lstFormatGroups.Size = New System.Drawing.Size(200, 154)
        Me._lstFormatGroups.TabIndex = 1
        '
        '_tbxSample
        '
        Me._tbxSample.BackColor = System.Drawing.SystemColors.Window
        Me._tbxSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._tbxSample.Location = New System.Drawing.Point(483, 238)
        Me._tbxSample.Name = "_tbxSample"
        Me._tbxSample.ReadOnly = True
        Me._tbxSample.Size = New System.Drawing.Size(211, 23)
        Me._tbxSample.TabIndex = 20
        Me._tbxSample.TabStop = False
        Me._tbxSample.Text = "Regexator"
        Me._tbxSample.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        '_lblSample
        '
        Me._lblSample.AutoSize = True
        Me._lblSample.Location = New System.Drawing.Point(480, 217)
        Me._lblSample.Margin = New System.Windows.Forms.Padding(3)
        Me._lblSample.Name = "_lblSample"
        Me._lblSample.Size = New System.Drawing.Size(49, 15)
        Me._lblSample.TabIndex = 19
        Me._lblSample.Text = "Sample:"
        '
        '_cbxFontBold
        '
        Me._cbxFontBold.AutoSize = True
        Me._cbxFontBold.Enabled = False
        Me._cbxFontBold.Location = New System.Drawing.Point(244, 326)
        Me._cbxFontBold.Margin = New System.Windows.Forms.Padding(3, 10, 3, 3)
        Me._cbxFontBold.Name = "_cbxFontBold"
        Me._cbxFontBold.Size = New System.Drawing.Size(50, 19)
        Me._cbxFontBold.TabIndex = 17
        Me._cbxFontBold.Text = "Bold"
        Me._cbxFontBold.UseVisualStyleBackColor = True
        '
        '_btnForeColor
        '
        Me._btnForeColor.Enabled = False
        Me._btnForeColor.Location = New System.Drawing.Point(425, 289)
        Me._btnForeColor.Name = "_btnForeColor"
        Me._btnForeColor.Size = New System.Drawing.Size(30, 24)
        Me._btnForeColor.TabIndex = 16
        Me._btnForeColor.Text = "…"
        Me._btnForeColor.UseVisualStyleBackColor = True
        '
        '_btnBackColor
        '
        Me._btnBackColor.Enabled = False
        Me._btnBackColor.Location = New System.Drawing.Point(425, 238)
        Me._btnBackColor.Name = "_btnBackColor"
        Me._btnBackColor.Size = New System.Drawing.Size(30, 24)
        Me._btnBackColor.TabIndex = 13
        Me._btnBackColor.Text = "…"
        Me._btnBackColor.UseVisualStyleBackColor = True
        '
        '_lblForeColor
        '
        Me._lblForeColor.AutoSize = True
        Me._lblForeColor.Location = New System.Drawing.Point(241, 268)
        Me._lblForeColor.Margin = New System.Windows.Forms.Padding(3)
        Me._lblForeColor.Name = "_lblForeColor"
        Me._lblForeColor.Size = New System.Drawing.Size(97, 15)
        Me._lblForeColor.TabIndex = 14
        Me._lblForeColor.Text = "Item foreground:"
        '
        '_lblBackColor
        '
        Me._lblBackColor.AutoSize = True
        Me._lblBackColor.Location = New System.Drawing.Point(241, 217)
        Me._lblBackColor.Margin = New System.Windows.Forms.Padding(3)
        Me._lblBackColor.Name = "_lblBackColor"
        Me._lblBackColor.Size = New System.Drawing.Size(101, 15)
        Me._lblBackColor.TabIndex = 11
        Me._lblBackColor.Text = "Item background:"
        '
        '_lblItem
        '
        Me._lblItem.AutoSize = True
        Me._lblItem.Location = New System.Drawing.Point(13, 196)
        Me._lblItem.Margin = New System.Windows.Forms.Padding(3)
        Me._lblItem.Name = "_lblItem"
        Me._lblItem.Size = New System.Drawing.Size(34, 15)
        Me._lblItem.TabIndex = 9
        Me._lblItem.Text = "Item:"
        '
        '_cmbForeColor
        '
        Me._cmbForeColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me._cmbForeColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbForeColor.Enabled = False
        Me._cmbForeColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbForeColor.FormattingEnabled = True
        Me._cmbForeColor.Location = New System.Drawing.Point(244, 289)
        Me._cmbForeColor.MaxDropDownItems = 20
        Me._cmbForeColor.Name = "_cmbForeColor"
        Me._cmbForeColor.SelectedColor = System.Drawing.Color.Empty
        Me._cmbForeColor.Size = New System.Drawing.Size(175, 24)
        Me._cmbForeColor.TabIndex = 15
        '
        '_cmbBackColor
        '
        Me._cmbBackColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me._cmbBackColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbBackColor.Enabled = False
        Me._cmbBackColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbBackColor.FormattingEnabled = True
        Me._cmbBackColor.Location = New System.Drawing.Point(244, 238)
        Me._cmbBackColor.MaxDropDownItems = 20
        Me._cmbBackColor.Name = "_cmbBackColor"
        Me._cmbBackColor.SelectedColor = System.Drawing.Color.Empty
        Me._cmbBackColor.Size = New System.Drawing.Size(175, 24)
        Me._cmbBackColor.TabIndex = 12
        '
        '_lstFormatInfos
        '
        Me._lstFormatInfos.FormattingEnabled = True
        Me._lstFormatInfos.ItemHeight = 15
        Me._lstFormatInfos.Location = New System.Drawing.Point(16, 217)
        Me._lstFormatInfos.Margin = New System.Windows.Forms.Padding(5)
        Me._lstFormatInfos.Name = "_lstFormatInfos"
        Me._lstFormatInfos.Size = New System.Drawing.Size(200, 199)
        Me._lstFormatInfos.TabIndex = 10
        '
        '_cmbFontSize
        '
        Me._cmbFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbFontSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbFontSize.FormattingEnabled = True
        Me._cmbFontSize.Location = New System.Drawing.Point(483, 34)
        Me._cmbFontSize.Margin = New System.Windows.Forms.Padding(25, 3, 3, 3)
        Me._cmbFontSize.MaxDropDownItems = 20
        Me._cmbFontSize.Name = "_cmbFontSize"
        Me._cmbFontSize.Size = New System.Drawing.Size(50, 23)
        Me._cmbFontSize.TabIndex = 5
        '
        '_lblFontSize
        '
        Me._lblFontSize.AutoSize = True
        Me._lblFontSize.Location = New System.Drawing.Point(480, 13)
        Me._lblFontSize.Margin = New System.Windows.Forms.Padding(3)
        Me._lblFontSize.Name = "_lblFontSize"
        Me._lblFontSize.Size = New System.Drawing.Size(30, 15)
        Me._lblFontSize.TabIndex = 4
        Me._lblFontSize.Text = "Size:"
        '
        '_cmbFontFamily
        '
        Me._cmbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbFontFamily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbFontFamily.FormattingEnabled = True
        Me._cmbFontFamily.Location = New System.Drawing.Point(244, 34)
        Me._cmbFontFamily.MaxDropDownItems = 20
        Me._cmbFontFamily.Name = "_cmbFontFamily"
        Me._cmbFontFamily.Size = New System.Drawing.Size(211, 23)
        Me._cmbFontFamily.TabIndex = 3
        '
        '_lblFont
        '
        Me._lblFont.AutoSize = True
        Me._lblFont.Location = New System.Drawing.Point(241, 13)
        Me._lblFont.Margin = New System.Windows.Forms.Padding(3)
        Me._lblFont.Name = "_lblFont"
        Me._lblFont.Size = New System.Drawing.Size(34, 15)
        Me._lblFont.TabIndex = 2
        Me._lblFont.Text = "Font:"
        '
        '_lblGroup
        '
        Me._lblGroup.AutoSize = True
        Me._lblGroup.Location = New System.Drawing.Point(13, 13)
        Me._lblGroup.Margin = New System.Windows.Forms.Padding(3)
        Me._lblGroup.Name = "_lblGroup"
        Me._lblGroup.Size = New System.Drawing.Size(43, 15)
        Me._lblGroup.TabIndex = 0
        Me._lblGroup.Text = "Group:"
        '
        '_tbpProjectExplorer
        '
        Me._tbpProjectExplorer.Controls.Add(Me._cbxConfirmFileInputRemoval)
        Me._tbpProjectExplorer.Controls.Add(Me._cbxTrackActiveItem)
        Me._tbpProjectExplorer.Controls.Add(Me._cbxShowHiddenFiles)
        Me._tbpProjectExplorer.Controls.Add(Me._cbxUseRecycleBin)
        Me._tbpProjectExplorer.Location = New System.Drawing.Point(4, 53)
        Me._tbpProjectExplorer.Name = "_tbpProjectExplorer"
        Me._tbpProjectExplorer.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpProjectExplorer.Size = New System.Drawing.Size(749, 434)
        Me._tbpProjectExplorer.TabIndex = 1
        Me._tbpProjectExplorer.Text = "Project Explorer"
        Me._tbpProjectExplorer.UseVisualStyleBackColor = True
        '
        '_cbxConfirmFileInputRemoval
        '
        Me._cbxConfirmFileInputRemoval.AutoSize = True
        Me._cbxConfirmFileInputRemoval.Location = New System.Drawing.Point(26, 119)
        Me._cbxConfirmFileInputRemoval.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxConfirmFileInputRemoval.Name = "_cbxConfirmFileInputRemoval"
        Me._cbxConfirmFileInputRemoval.Size = New System.Drawing.Size(166, 19)
        Me._cbxConfirmFileInputRemoval.TabIndex = 5
        Me._cbxConfirmFileInputRemoval.Text = "Confirm file input removal"
        Me._cbxConfirmFileInputRemoval.UseVisualStyleBackColor = True
        '
        '_cbxTrackActiveItem
        '
        Me._cbxTrackActiveItem.AutoSize = True
        Me._cbxTrackActiveItem.Location = New System.Drawing.Point(26, 88)
        Me._cbxTrackActiveItem.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxTrackActiveItem.Name = "_cbxTrackActiveItem"
        Me._cbxTrackActiveItem.Size = New System.Drawing.Size(116, 19)
        Me._cbxTrackActiveItem.TabIndex = 2
        Me._cbxTrackActiveItem.Text = "Track active item"
        Me._cbxTrackActiveItem.UseVisualStyleBackColor = True
        '
        '_cbxShowHiddenFiles
        '
        Me._cbxShowHiddenFiles.AutoSize = True
        Me._cbxShowHiddenFiles.Location = New System.Drawing.Point(26, 57)
        Me._cbxShowHiddenFiles.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxShowHiddenFiles.Name = "_cbxShowHiddenFiles"
        Me._cbxShowHiddenFiles.Size = New System.Drawing.Size(119, 19)
        Me._cbxShowHiddenFiles.TabIndex = 1
        Me._cbxShowHiddenFiles.Text = "Show hidden files"
        Me._cbxShowHiddenFiles.UseVisualStyleBackColor = True
        '
        '_cbxUseRecycleBin
        '
        Me._cbxUseRecycleBin.AutoSize = True
        Me._cbxUseRecycleBin.Location = New System.Drawing.Point(26, 26)
        Me._cbxUseRecycleBin.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxUseRecycleBin.Name = "_cbxUseRecycleBin"
        Me._cbxUseRecycleBin.Size = New System.Drawing.Size(204, 19)
        Me._cbxUseRecycleBin.TabIndex = 0
        Me._cbxUseRecycleBin.Text = Global.Regexator.My.Resources.Resources.MoveDeletedItemsToRecycleBin
        Me._cbxUseRecycleBin.UseVisualStyleBackColor = True
        '
        '_tbpSnippets
        '
        Me._tbpSnippets.Controls.Add(Me._cbxSetSnippetOptions)
        Me._tbpSnippets.Controls.Add(Me._gbxSnippetDirectories)
        Me._tbpSnippets.Controls.Add(Me._btnViewSnippetErrorLog)
        Me._tbpSnippets.Controls.Add(Me._lblHiddenCategoriesInfo)
        Me._tbpSnippets.Controls.Add(Me._btnSetDefaultFavoriteSnippets)
        Me._tbpSnippets.Controls.Add(Me._gbxHiddenCategories)
        Me._tbpSnippets.Location = New System.Drawing.Point(4, 53)
        Me._tbpSnippets.Name = "_tbpSnippets"
        Me._tbpSnippets.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpSnippets.Size = New System.Drawing.Size(749, 434)
        Me._tbpSnippets.TabIndex = 3
        Me._tbpSnippets.Text = "Snippets"
        Me._tbpSnippets.UseVisualStyleBackColor = True
        '
        '_cbxSetSnippetOptions
        '
        Me._cbxSetSnippetOptions.AutoSize = True
        Me._cbxSetSnippetOptions.Location = New System.Drawing.Point(25, 388)
        Me._cbxSetSnippetOptions.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxSetSnippetOptions.Name = "_cbxSetSnippetOptions"
        Me._cbxSetSnippetOptions.Size = New System.Drawing.Size(200, 19)
        Me._cbxSetSnippetOptions.TabIndex = 2
        Me._cbxSetSnippetOptions.Text = "Set options if required by snippet"
        Me._cbxSetSnippetOptions.UseVisualStyleBackColor = True
        '
        '_gbxSnippetDirectories
        '
        Me._gbxSnippetDirectories.Controls.Add(Me._pnlSnippetDirectories)
        Me._gbxSnippetDirectories.Location = New System.Drawing.Point(260, 25)
        Me._gbxSnippetDirectories.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxSnippetDirectories.Name = "_gbxSnippetDirectories"
        Me._gbxSnippetDirectories.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxSnippetDirectories.Size = New System.Drawing.Size(464, 242)
        Me._gbxSnippetDirectories.TabIndex = 3
        Me._gbxSnippetDirectories.TabStop = False
        Me._gbxSnippetDirectories.Text = "Snippet Directories"
        '
        '_pnlSnippetDirectories
        '
        Me._pnlSnippetDirectories.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._pnlSnippetDirectories.Location = New System.Drawing.Point(13, 29)
        Me._pnlSnippetDirectories.Name = "_pnlSnippetDirectories"
        Me._pnlSnippetDirectories.Size = New System.Drawing.Size(438, 200)
        Me._pnlSnippetDirectories.TabIndex = 0
        '
        '_btnViewSnippetErrorLog
        '
        Me._btnViewSnippetErrorLog.Location = New System.Drawing.Point(524, 278)
        Me._btnViewSnippetErrorLog.Margin = New System.Windows.Forms.Padding(6)
        Me._btnViewSnippetErrorLog.Name = "_btnViewSnippetErrorLog"
        Me._btnViewSnippetErrorLog.Size = New System.Drawing.Size(200, 30)
        Me._btnViewSnippetErrorLog.TabIndex = 6
        Me._btnViewSnippetErrorLog.Text = "View Error Log"
        Me._btnViewSnippetErrorLog.UseVisualStyleBackColor = True
        Me._btnViewSnippetErrorLog.Visible = False
        '
        '_lblHiddenCategoriesInfo
        '
        Me._lblHiddenCategoriesInfo.AutoSize = True
        Me._lblHiddenCategoriesInfo.Location = New System.Drawing.Point(22, 361)
        Me._lblHiddenCategoriesInfo.Margin = New System.Windows.Forms.Padding(6, 6, 100, 6)
        Me._lblHiddenCategoriesInfo.Name = "_lblHiddenCategoriesInfo"
        Me._lblHiddenCategoriesInfo.Size = New System.Drawing.Size(219, 15)
        Me._lblHiddenCategoriesInfo.TabIndex = 1
        Me._lblHiddenCategoriesInfo.Text = "Applicable for pattern editor snippet list."
        '
        '_btnSetDefaultFavoriteSnippets
        '
        Me._btnSetDefaultFavoriteSnippets.Location = New System.Drawing.Point(261, 278)
        Me._btnSetDefaultFavoriteSnippets.Margin = New System.Windows.Forms.Padding(6)
        Me._btnSetDefaultFavoriteSnippets.Name = "_btnSetDefaultFavoriteSnippets"
        Me._btnSetDefaultFavoriteSnippets.Size = New System.Drawing.Size(200, 30)
        Me._btnSetDefaultFavoriteSnippets.TabIndex = 4
        Me._btnSetDefaultFavoriteSnippets.Text = "Set Default Favorite Snippets"
        Me._btnSetDefaultFavoriteSnippets.UseVisualStyleBackColor = True
        '
        '_gbxHiddenCategories
        '
        Me._gbxHiddenCategories.Location = New System.Drawing.Point(25, 25)
        Me._gbxHiddenCategories.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxHiddenCategories.Name = "_gbxHiddenCategories"
        Me._gbxHiddenCategories.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxHiddenCategories.Size = New System.Drawing.Size(225, 325)
        Me._gbxHiddenCategories.TabIndex = 0
        Me._gbxHiddenCategories.TabStop = False
        Me._gbxHiddenCategories.Text = "Hidden Categories"
        '
        '_tbpOutput
        '
        Me._tbpOutput.Controls.Add(Me._gbxOutputText)
        Me._tbpOutput.Controls.Add(Me._cmbNumberAlignment)
        Me._tbpOutput.Controls.Add(Me._lblNumberAlignment)
        Me._tbpOutput.Controls.Add(Me._numOutputLimit)
        Me._tbpOutput.Controls.Add(Me._lblOutputLimit)
        Me._tbpOutput.Location = New System.Drawing.Point(4, 53)
        Me._tbpOutput.Name = "_tbpOutput"
        Me._tbpOutput.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpOutput.Size = New System.Drawing.Size(749, 434)
        Me._tbpOutput.TabIndex = 7
        Me._tbpOutput.Text = "Output"
        Me._tbpOutput.UseVisualStyleBackColor = True
        '
        '_gbxOutputText
        '
        Me._gbxOutputText.Controls.Add(Me._cbxHighlightBeforeAfterResult)
        Me._gbxOutputText.Controls.Add(Me._cbxOutputOmitRepeatedInfo)
        Me._gbxOutputText.Location = New System.Drawing.Point(25, 25)
        Me._gbxOutputText.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxOutputText.Name = "_gbxOutputText"
        Me._gbxOutputText.Size = New System.Drawing.Size(250, 150)
        Me._gbxOutputText.TabIndex = 0
        Me._gbxOutputText.TabStop = False
        Me._gbxOutputText.Text = "Text"
        '
        '_cbxHighlightBeforeAfterResult
        '
        Me._cbxHighlightBeforeAfterResult.AutoSize = True
        Me._cbxHighlightBeforeAfterResult.Location = New System.Drawing.Point(9, 56)
        Me._cbxHighlightBeforeAfterResult.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxHighlightBeforeAfterResult.Name = "_cbxHighlightBeforeAfterResult"
        Me._cbxHighlightBeforeAfterResult.Size = New System.Drawing.Size(195, 19)
        Me._cbxHighlightBeforeAfterResult.TabIndex = 1
        Me._cbxHighlightBeforeAfterResult.Text = "Highlight before and after result"
        Me._cbxHighlightBeforeAfterResult.UseVisualStyleBackColor = True
        '
        '_cbxOutputOmitRepeatedInfo
        '
        Me._cbxOutputOmitRepeatedInfo.AutoSize = True
        Me._cbxOutputOmitRepeatedInfo.Location = New System.Drawing.Point(9, 25)
        Me._cbxOutputOmitRepeatedInfo.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxOutputOmitRepeatedInfo.Name = "_cbxOutputOmitRepeatedInfo"
        Me._cbxOutputOmitRepeatedInfo.Size = New System.Drawing.Size(126, 19)
        Me._cbxOutputOmitRepeatedInfo.TabIndex = 0
        Me._cbxOutputOmitRepeatedInfo.Text = "Omit repeated info"
        Me._cbxOutputOmitRepeatedInfo.UseVisualStyleBackColor = True
        '
        '_cmbNumberAlignment
        '
        Me._cmbNumberAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbNumberAlignment.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbNumberAlignment.FormattingEnabled = True
        Me._cmbNumberAlignment.Location = New System.Drawing.Point(188, 219)
        Me._cmbNumberAlignment.Margin = New System.Windows.Forms.Padding(6)
        Me._cmbNumberAlignment.Name = "_cmbNumberAlignment"
        Me._cmbNumberAlignment.Size = New System.Drawing.Size(76, 23)
        Me._cmbNumberAlignment.TabIndex = 5
        '
        '_lblNumberAlignment
        '
        Me._lblNumberAlignment.AutoSize = True
        Me._lblNumberAlignment.Location = New System.Drawing.Point(22, 222)
        Me._lblNumberAlignment.Margin = New System.Windows.Forms.Padding(6, 6, 50, 6)
        Me._lblNumberAlignment.Name = "_lblNumberAlignment"
        Me._lblNumberAlignment.Size = New System.Drawing.Size(111, 15)
        Me._lblNumberAlignment.TabIndex = 4
        Me._lblNumberAlignment.Text = "Number alignment:"
        '
        '_numOutputLimit
        '
        Me._numOutputLimit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._numOutputLimit.Location = New System.Drawing.Point(189, 184)
        Me._numOutputLimit.Margin = New System.Windows.Forms.Padding(10, 6, 6, 6)
        Me._numOutputLimit.Name = "_numOutputLimit"
        Me._numOutputLimit.Size = New System.Drawing.Size(75, 23)
        Me._numOutputLimit.TabIndex = 3
        Me._numOutputLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        '_lblOutputLimit
        '
        Me._lblOutputLimit.AutoSize = True
        Me._lblOutputLimit.Location = New System.Drawing.Point(22, 186)
        Me._lblOutputLimit.Margin = New System.Windows.Forms.Padding(6, 6, 50, 6)
        Me._lblOutputLimit.Name = "_lblOutputLimit"
        Me._lblOutputLimit.Size = New System.Drawing.Size(107, 15)
        Me._lblOutputLimit.TabIndex = 2
        Me._lblOutputLimit.Text = "Output items limit:"
        '
        '_tbpFileSystemSearch
        '
        Me._tbpFileSystemSearch.Controls.Add(Me._numFileSystemSearchMaxResultCount)
        Me._tbpFileSystemSearch.Controls.Add(Me._lblFileSystemSearchMaxResultCount)
        Me._tbpFileSystemSearch.Location = New System.Drawing.Point(4, 53)
        Me._tbpFileSystemSearch.Name = "_tbpFileSystemSearch"
        Me._tbpFileSystemSearch.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpFileSystemSearch.Size = New System.Drawing.Size(749, 434)
        Me._tbpFileSystemSearch.TabIndex = 14
        Me._tbpFileSystemSearch.Text = "File System Search"
        Me._tbpFileSystemSearch.UseVisualStyleBackColor = True
        '
        '_numFileSystemSearchMaxResultCount
        '
        Me._numFileSystemSearchMaxResultCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._numFileSystemSearchMaxResultCount.Location = New System.Drawing.Point(246, 24)
        Me._numFileSystemSearchMaxResultCount.Margin = New System.Windows.Forms.Padding(10, 6, 6, 6)
        Me._numFileSystemSearchMaxResultCount.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me._numFileSystemSearchMaxResultCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me._numFileSystemSearchMaxResultCount.Name = "_numFileSystemSearchMaxResultCount"
        Me._numFileSystemSearchMaxResultCount.Size = New System.Drawing.Size(75, 23)
        Me._numFileSystemSearchMaxResultCount.TabIndex = 3
        Me._numFileSystemSearchMaxResultCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me._numFileSystemSearchMaxResultCount.ThousandsSeparator = True
        Me._numFileSystemSearchMaxResultCount.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        '_lblFileSystemSearchMaxResultCount
        '
        Me._lblFileSystemSearchMaxResultCount.AutoSize = True
        Me._lblFileSystemSearchMaxResultCount.Location = New System.Drawing.Point(26, 26)
        Me._lblFileSystemSearchMaxResultCount.Margin = New System.Windows.Forms.Padding(6, 6, 50, 6)
        Me._lblFileSystemSearchMaxResultCount.Name = "_lblFileSystemSearchMaxResultCount"
        Me._lblFileSystemSearchMaxResultCount.Size = New System.Drawing.Size(160, 15)
        Me._lblFileSystemSearchMaxResultCount.TabIndex = 2
        Me._lblFileSystemSearchMaxResultCount.Text = "Maximum number of results:"
        '
        '_tbpSymbols
        '
        Me._tbpSymbols.Location = New System.Drawing.Point(4, 53)
        Me._tbpSymbols.Name = "_tbpSymbols"
        Me._tbpSymbols.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpSymbols.Size = New System.Drawing.Size(749, 434)
        Me._tbpSymbols.TabIndex = 9
        Me._tbpSymbols.Text = "Symbols"
        Me._tbpSymbols.UseVisualStyleBackColor = True
        '
        '_tbpExport
        '
        Me._tbpExport.Controls.Add(Me._cbxMultiline)
        Me._tbpExport.Controls.Add(Me._lblDefaultExportMode)
        Me._tbpExport.Controls.Add(Me._cmbExportMode)
        Me._tbpExport.Location = New System.Drawing.Point(4, 53)
        Me._tbpExport.Name = "_tbpExport"
        Me._tbpExport.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpExport.Size = New System.Drawing.Size(749, 434)
        Me._tbpExport.TabIndex = 4
        Me._tbpExport.Text = "Export"
        Me._tbpExport.UseVisualStyleBackColor = True
        '
        '_cbxMultiline
        '
        Me._cbxMultiline.AutoSize = True
        Me._cbxMultiline.Enabled = False
        Me._cbxMultiline.Location = New System.Drawing.Point(29, 58)
        Me._cbxMultiline.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxMultiline.Name = "_cbxMultiline"
        Me._cbxMultiline.Size = New System.Drawing.Size(73, 19)
        Me._cbxMultiline.TabIndex = 9
        Me._cbxMultiline.Text = "Multiline"
        Me._cbxMultiline.UseVisualStyleBackColor = True
        '
        '_lblDefaultExportMode
        '
        Me._lblDefaultExportMode.AutoSize = True
        Me._lblDefaultExportMode.Location = New System.Drawing.Point(26, 26)
        Me._lblDefaultExportMode.Margin = New System.Windows.Forms.Padding(6, 6, 50, 6)
        Me._lblDefaultExportMode.Name = "_lblDefaultExportMode"
        Me._lblDefaultExportMode.Size = New System.Drawing.Size(41, 15)
        Me._lblDefaultExportMode.TabIndex = 7
        Me._lblDefaultExportMode.Text = "Mode:"
        '
        '_cmbExportMode
        '
        Me._cmbExportMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbExportMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbExportMode.FormattingEnabled = True
        Me._cmbExportMode.Location = New System.Drawing.Point(123, 23)
        Me._cmbExportMode.Margin = New System.Windows.Forms.Padding(6)
        Me._cmbExportMode.Name = "_cmbExportMode"
        Me._cmbExportMode.Size = New System.Drawing.Size(200, 23)
        Me._cmbExportMode.TabIndex = 8
        '
        '_tbpRegexOptions
        '
        Me._tbpRegexOptions.Controls.Add(Me._cbxRegexOptionsDescriptionVisible)
        Me._tbpRegexOptions.Controls.Add(Me._gbxRegexOptionsVisibility)
        Me._tbpRegexOptions.Controls.Add(Me._cbxRegexOptionsHotkeyNumberVisible)
        Me._tbpRegexOptions.Location = New System.Drawing.Point(4, 53)
        Me._tbpRegexOptions.Name = "_tbpRegexOptions"
        Me._tbpRegexOptions.Padding = New System.Windows.Forms.Padding(20)
        Me._tbpRegexOptions.Size = New System.Drawing.Size(749, 434)
        Me._tbpRegexOptions.TabIndex = 12
        Me._tbpRegexOptions.Text = "Regex Options"
        Me._tbpRegexOptions.UseVisualStyleBackColor = True
        '
        '_cbxRegexOptionsDescriptionVisible
        '
        Me._cbxRegexOptionsDescriptionVisible.AutoSize = True
        Me._cbxRegexOptionsDescriptionVisible.Location = New System.Drawing.Point(26, 317)
        Me._cbxRegexOptionsDescriptionVisible.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxRegexOptionsDescriptionVisible.Name = "_cbxRegexOptionsDescriptionVisible"
        Me._cbxRegexOptionsDescriptionVisible.Size = New System.Drawing.Size(117, 19)
        Me._cbxRegexOptionsDescriptionVisible.TabIndex = 3
        Me._cbxRegexOptionsDescriptionVisible.Text = "Show description"
        Me._cbxRegexOptionsDescriptionVisible.UseVisualStyleBackColor = True
        '
        '_gbxRegexOptionsVisibility
        '
        Me._gbxRegexOptionsVisibility.Location = New System.Drawing.Point(25, 25)
        Me._gbxRegexOptionsVisibility.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxRegexOptionsVisibility.Name = "_gbxRegexOptionsVisibility"
        Me._gbxRegexOptionsVisibility.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxRegexOptionsVisibility.Size = New System.Drawing.Size(225, 250)
        Me._gbxRegexOptionsVisibility.TabIndex = 0
        Me._gbxRegexOptionsVisibility.TabStop = False
        Me._gbxRegexOptionsVisibility.Text = "Visibility"
        '
        '_cbxRegexOptionsHotkeyNumberVisible
        '
        Me._cbxRegexOptionsHotkeyNumberVisible.AutoSize = True
        Me._cbxRegexOptionsHotkeyNumberVisible.Location = New System.Drawing.Point(26, 286)
        Me._cbxRegexOptionsHotkeyNumberVisible.Margin = New System.Windows.Forms.Padding(6)
        Me._cbxRegexOptionsHotkeyNumberVisible.Name = "_cbxRegexOptionsHotkeyNumberVisible"
        Me._cbxRegexOptionsHotkeyNumberVisible.Size = New System.Drawing.Size(139, 19)
        Me._cbxRegexOptionsHotkeyNumberVisible.TabIndex = 2
        Me._cbxRegexOptionsHotkeyNumberVisible.Text = "Show hotkey number"
        Me._cbxRegexOptionsHotkeyNumberVisible.UseVisualStyleBackColor = True
        '
        '_tbpProjectDefaultValues
        '
        Me._tbpProjectDefaultValues.Controls.Add(Me._pnlDefaultValues)
        Me._tbpProjectDefaultValues.Location = New System.Drawing.Point(4, 53)
        Me._tbpProjectDefaultValues.Name = "_tbpProjectDefaultValues"
        Me._tbpProjectDefaultValues.Padding = New System.Windows.Forms.Padding(3)
        Me._tbpProjectDefaultValues.Size = New System.Drawing.Size(749, 434)
        Me._tbpProjectDefaultValues.TabIndex = 10
        Me._tbpProjectDefaultValues.Text = Global.Regexator.My.Resources.Resources.ProjectDefaultValues
        Me._tbpProjectDefaultValues.UseVisualStyleBackColor = True
        '
        '_pnlDefaultValues
        '
        Me._pnlDefaultValues.Controls.Add(Me._tbcProjectDefaultValues)
        Me._pnlDefaultValues.Dock = System.Windows.Forms.DockStyle.Fill
        Me._pnlDefaultValues.Location = New System.Drawing.Point(3, 3)
        Me._pnlDefaultValues.Name = "_pnlDefaultValues"
        Me._pnlDefaultValues.Padding = New System.Windows.Forms.Padding(3)
        Me._pnlDefaultValues.Size = New System.Drawing.Size(743, 428)
        Me._pnlDefaultValues.TabIndex = 0
        '
        '_tbcProjectDefaultValues
        '
        Me._tbcProjectDefaultValues.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
        Me._tbcProjectDefaultValues.Controls.Add(Me._tbpPatternDefaults)
        Me._tbcProjectDefaultValues.Controls.Add(Me._tbpReplacementDefaults)
        Me._tbcProjectDefaultValues.Controls.Add(Me._tbpOutputDefaults)
        Me._tbcProjectDefaultValues.Dock = System.Windows.Forms.DockStyle.Fill
        Me._tbcProjectDefaultValues.Location = New System.Drawing.Point(3, 3)
        Me._tbcProjectDefaultValues.Margin = New System.Windows.Forms.Padding(6)
        Me._tbcProjectDefaultValues.Name = "_tbcProjectDefaultValues"
        Me._tbcProjectDefaultValues.SelectedIndex = 0
        Me._tbcProjectDefaultValues.Size = New System.Drawing.Size(737, 422)
        Me._tbcProjectDefaultValues.TabIndex = 1
        '
        '_tbpPatternDefaults
        '
        Me._tbpPatternDefaults.Controls.Add(Me._gbxDefaultRegexOptions)
        Me._tbpPatternDefaults.Controls.Add(Me._gbxDefaultPatternOptions)
        Me._tbpPatternDefaults.Location = New System.Drawing.Point(4, 27)
        Me._tbpPatternDefaults.Name = "_tbpPatternDefaults"
        Me._tbpPatternDefaults.Padding = New System.Windows.Forms.Padding(3)
        Me._tbpPatternDefaults.Size = New System.Drawing.Size(729, 391)
        Me._tbpPatternDefaults.TabIndex = 0
        Me._tbpPatternDefaults.Text = "Pattern"
        Me._tbpPatternDefaults.UseVisualStyleBackColor = True
        '
        '_gbxDefaultRegexOptions
        '
        Me._gbxDefaultRegexOptions.Location = New System.Drawing.Point(243, 8)
        Me._gbxDefaultRegexOptions.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxDefaultRegexOptions.Name = "_gbxDefaultRegexOptions"
        Me._gbxDefaultRegexOptions.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxDefaultRegexOptions.Size = New System.Drawing.Size(225, 250)
        Me._gbxDefaultRegexOptions.TabIndex = 5
        Me._gbxDefaultRegexOptions.TabStop = False
        Me._gbxDefaultRegexOptions.Text = "Regex Options"
        '
        '_gbxDefaultPatternOptions
        '
        Me._gbxDefaultPatternOptions.Location = New System.Drawing.Point(8, 8)
        Me._gbxDefaultPatternOptions.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxDefaultPatternOptions.Name = "_gbxDefaultPatternOptions"
        Me._gbxDefaultPatternOptions.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxDefaultPatternOptions.Size = New System.Drawing.Size(225, 187)
        Me._gbxDefaultPatternOptions.TabIndex = 4
        Me._gbxDefaultPatternOptions.TabStop = False
        Me._gbxDefaultPatternOptions.Text = "Options"
        '
        '_tbpReplacementDefaults
        '
        Me._tbpReplacementDefaults.Controls.Add(Me._gbxReplacementText)
        Me._tbpReplacementDefaults.Controls.Add(Me._gbxReplacementNewLine)
        Me._tbpReplacementDefaults.Controls.Add(Me._gbxDefaultReplacementOptions)
        Me._tbpReplacementDefaults.Location = New System.Drawing.Point(4, 27)
        Me._tbpReplacementDefaults.Name = "_tbpReplacementDefaults"
        Me._tbpReplacementDefaults.Padding = New System.Windows.Forms.Padding(3)
        Me._tbpReplacementDefaults.Size = New System.Drawing.Size(729, 391)
        Me._tbpReplacementDefaults.TabIndex = 1
        Me._tbpReplacementDefaults.Text = "Replacement"
        Me._tbpReplacementDefaults.UseVisualStyleBackColor = True
        '
        '_gbxReplacementText
        '
        Me._gbxReplacementText.Controls.Add(Me._tbxReplacementText)
        Me._gbxReplacementText.Location = New System.Drawing.Point(243, 118)
        Me._gbxReplacementText.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxReplacementText.Name = "_gbxReplacementText"
        Me._gbxReplacementText.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxReplacementText.Size = New System.Drawing.Size(225, 77)
        Me._gbxReplacementText.TabIndex = 7
        Me._gbxReplacementText.TabStop = False
        Me._gbxReplacementText.Text = "Text"
        '
        '_tbxReplacementText
        '
        Me._tbxReplacementText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._tbxReplacementText.Location = New System.Drawing.Point(13, 29)
        Me._tbxReplacementText.Name = "_tbxReplacementText"
        Me._tbxReplacementText.Size = New System.Drawing.Size(199, 23)
        Me._tbxReplacementText.TabIndex = 0
        '
        '_gbxReplacementNewLine
        '
        Me._gbxReplacementNewLine.Controls.Add(Me._lstReplacementNewLine)
        Me._gbxReplacementNewLine.Location = New System.Drawing.Point(243, 8)
        Me._gbxReplacementNewLine.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxReplacementNewLine.Name = "_gbxReplacementNewLine"
        Me._gbxReplacementNewLine.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxReplacementNewLine.Size = New System.Drawing.Size(225, 100)
        Me._gbxReplacementNewLine.TabIndex = 6
        Me._gbxReplacementNewLine.TabStop = False
        Me._gbxReplacementNewLine.Text = "New Line"
        '
        '_lstReplacementNewLine
        '
        Me._lstReplacementNewLine.Dock = System.Windows.Forms.DockStyle.Fill
        Me._lstReplacementNewLine.FormattingEnabled = True
        Me._lstReplacementNewLine.ItemHeight = 15
        Me._lstReplacementNewLine.Location = New System.Drawing.Point(10, 26)
        Me._lstReplacementNewLine.Name = "_lstReplacementNewLine"
        Me._lstReplacementNewLine.Size = New System.Drawing.Size(205, 64)
        Me._lstReplacementNewLine.TabIndex = 0
        '
        '_gbxDefaultReplacementOptions
        '
        Me._gbxDefaultReplacementOptions.Location = New System.Drawing.Point(8, 8)
        Me._gbxDefaultReplacementOptions.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxDefaultReplacementOptions.Name = "_gbxDefaultReplacementOptions"
        Me._gbxDefaultReplacementOptions.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxDefaultReplacementOptions.Size = New System.Drawing.Size(225, 187)
        Me._gbxDefaultReplacementOptions.TabIndex = 5
        Me._gbxDefaultReplacementOptions.TabStop = False
        Me._gbxDefaultReplacementOptions.Text = "Options"
        '
        '_tbpOutputDefaults
        '
        Me._tbpOutputDefaults.Controls.Add(Me._gbxDefaultOutputOptions)
        Me._tbpOutputDefaults.Location = New System.Drawing.Point(4, 27)
        Me._tbpOutputDefaults.Name = "_tbpOutputDefaults"
        Me._tbpOutputDefaults.Padding = New System.Windows.Forms.Padding(3)
        Me._tbpOutputDefaults.Size = New System.Drawing.Size(729, 391)
        Me._tbpOutputDefaults.TabIndex = 4
        Me._tbpOutputDefaults.Text = "Output"
        Me._tbpOutputDefaults.UseVisualStyleBackColor = True
        '
        '_gbxDefaultOutputOptions
        '
        Me._gbxDefaultOutputOptions.Location = New System.Drawing.Point(8, 8)
        Me._gbxDefaultOutputOptions.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxDefaultOutputOptions.Name = "_gbxDefaultOutputOptions"
        Me._gbxDefaultOutputOptions.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxDefaultOutputOptions.Size = New System.Drawing.Size(225, 250)
        Me._gbxDefaultOutputOptions.TabIndex = 2
        Me._gbxDefaultOutputOptions.TabStop = False
        Me._gbxDefaultOutputOptions.Text = "Options"
        '
        '_tbpInputDefaultValues
        '
        Me._tbpInputDefaultValues.Controls.Add(Me._gbxInputEncoding)
        Me._tbpInputDefaultValues.Controls.Add(Me._gbxInputNewLine)
        Me._tbpInputDefaultValues.Controls.Add(Me._gbxDefaultInputOptions)
        Me._tbpInputDefaultValues.Location = New System.Drawing.Point(4, 53)
        Me._tbpInputDefaultValues.Name = "_tbpInputDefaultValues"
        Me._tbpInputDefaultValues.Padding = New System.Windows.Forms.Padding(3)
        Me._tbpInputDefaultValues.Size = New System.Drawing.Size(749, 434)
        Me._tbpInputDefaultValues.TabIndex = 11
        Me._tbpInputDefaultValues.Text = "Input Default Values"
        Me._tbpInputDefaultValues.UseVisualStyleBackColor = True
        '
        '_gbxInputEncoding
        '
        Me._gbxInputEncoding.Controls.Add(Me._cmbInputEncoding)
        Me._gbxInputEncoding.Controls.Add(Me._btnSetDefaultInputEncoding)
        Me._gbxInputEncoding.Location = New System.Drawing.Point(478, 8)
        Me._gbxInputEncoding.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxInputEncoding.Name = "_gbxInputEncoding"
        Me._gbxInputEncoding.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxInputEncoding.Size = New System.Drawing.Size(225, 125)
        Me._gbxInputEncoding.TabIndex = 5
        Me._gbxInputEncoding.TabStop = False
        Me._gbxInputEncoding.Text = "New Line"
        '
        '_cmbInputEncoding
        '
        Me._cmbInputEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._cmbInputEncoding.DropDownWidth = 250
        Me._cmbInputEncoding.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._cmbInputEncoding.FormattingEnabled = True
        Me._cmbInputEncoding.Location = New System.Drawing.Point(13, 29)
        Me._cmbInputEncoding.MaxDropDownItems = 20
        Me._cmbInputEncoding.Name = "_cmbInputEncoding"
        Me._cmbInputEncoding.Size = New System.Drawing.Size(200, 23)
        Me._cmbInputEncoding.TabIndex = 3
        '
        '_btnSetDefaultInputEncoding
        '
        Me._btnSetDefaultInputEncoding.Location = New System.Drawing.Point(13, 61)
        Me._btnSetDefaultInputEncoding.Margin = New System.Windows.Forms.Padding(6)
        Me._btnSetDefaultInputEncoding.Name = "_btnSetDefaultInputEncoding"
        Me._btnSetDefaultInputEncoding.Size = New System.Drawing.Size(150, 30)
        Me._btnSetDefaultInputEncoding.TabIndex = 4
        Me._btnSetDefaultInputEncoding.Text = "Set Default Encoding"
        Me._btnSetDefaultInputEncoding.UseVisualStyleBackColor = True
        '
        '_gbxInputNewLine
        '
        Me._gbxInputNewLine.Controls.Add(Me._lstInputNewLine)
        Me._gbxInputNewLine.Location = New System.Drawing.Point(243, 8)
        Me._gbxInputNewLine.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxInputNewLine.Name = "_gbxInputNewLine"
        Me._gbxInputNewLine.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxInputNewLine.Size = New System.Drawing.Size(225, 125)
        Me._gbxInputNewLine.TabIndex = 1
        Me._gbxInputNewLine.TabStop = False
        Me._gbxInputNewLine.Text = "New Line"
        '
        '_lstInputNewLine
        '
        Me._lstInputNewLine.Dock = System.Windows.Forms.DockStyle.Fill
        Me._lstInputNewLine.FormattingEnabled = True
        Me._lstInputNewLine.ItemHeight = 15
        Me._lstInputNewLine.Location = New System.Drawing.Point(10, 26)
        Me._lstInputNewLine.Name = "_lstInputNewLine"
        Me._lstInputNewLine.Size = New System.Drawing.Size(205, 89)
        Me._lstInputNewLine.TabIndex = 0
        '
        '_gbxDefaultInputOptions
        '
        Me._gbxDefaultInputOptions.Location = New System.Drawing.Point(8, 8)
        Me._gbxDefaultInputOptions.Margin = New System.Windows.Forms.Padding(5)
        Me._gbxDefaultInputOptions.Name = "_gbxDefaultInputOptions"
        Me._gbxDefaultInputOptions.Padding = New System.Windows.Forms.Padding(10)
        Me._gbxDefaultInputOptions.Size = New System.Drawing.Size(225, 187)
        Me._gbxDefaultInputOptions.TabIndex = 0
        Me._gbxDefaultInputOptions.TabStop = False
        Me._gbxDefaultInputOptions.Text = "Options"
        '
        'OptionsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me._btnOk)
        Me.Controls.Add(Me._btnCancel)
        Me.Controls.Add(Me._tbc)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.Name = "OptionsForm"
        Me._tbc.ResumeLayout(False)
        Me._tbpApplication.ResumeLayout(False)
        Me._tbpApplication.PerformLayout()
        CType(Me._numRecentMaxCount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._numMatchTimeout, System.ComponentModel.ISupportInitialize).EndInit()
        Me._tbpTextEditor.ResumeLayout(False)
        Me._tbpTextEditor.PerformLayout()
        Me._tbpFormats.ResumeLayout(False)
        Me._tbpFormats.PerformLayout()
        Me._tbpProjectExplorer.ResumeLayout(False)
        Me._tbpProjectExplorer.PerformLayout()
        Me._tbpSnippets.ResumeLayout(False)
        Me._tbpSnippets.PerformLayout()
        Me._gbxSnippetDirectories.ResumeLayout(False)
        Me._tbpOutput.ResumeLayout(False)
        Me._tbpOutput.PerformLayout()
        Me._gbxOutputText.ResumeLayout(False)
        Me._gbxOutputText.PerformLayout()
        CType(Me._numOutputLimit, System.ComponentModel.ISupportInitialize).EndInit()
        Me._tbpFileSystemSearch.ResumeLayout(False)
        Me._tbpFileSystemSearch.PerformLayout()
        CType(Me._numFileSystemSearchMaxResultCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me._tbpExport.ResumeLayout(False)
        Me._tbpExport.PerformLayout()
        Me._tbpRegexOptions.ResumeLayout(False)
        Me._tbpRegexOptions.PerformLayout()
        Me._tbpProjectDefaultValues.ResumeLayout(False)
        Me._pnlDefaultValues.ResumeLayout(False)
        Me._tbcProjectDefaultValues.ResumeLayout(False)
        Me._tbpPatternDefaults.ResumeLayout(False)
        Me._tbpReplacementDefaults.ResumeLayout(False)
        Me._gbxReplacementText.ResumeLayout(False)
        Me._gbxReplacementText.PerformLayout()
        Me._gbxReplacementNewLine.ResumeLayout(False)
        Me._tbpOutputDefaults.ResumeLayout(False)
        Me._tbpInputDefaultValues.ResumeLayout(False)
        Me._gbxInputEncoding.ResumeLayout(False)
        Me._gbxInputNewLine.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents _tbc As System.Windows.Forms.TabControl
    Friend WithEvents _tbpApplication As System.Windows.Forms.TabPage
    Friend WithEvents _btnCancel As System.Windows.Forms.Button
    Friend WithEvents _btnOk As System.Windows.Forms.Button
    Friend WithEvents _tbpProjectExplorer As System.Windows.Forms.TabPage
    Friend WithEvents _lblMatchTimeout As System.Windows.Forms.Label
    Friend WithEvents _numMatchTimeout As System.Windows.Forms.NumericUpDown
    Friend WithEvents _cbxUseRecycleBin As System.Windows.Forms.CheckBox
    Friend WithEvents _numRecentMaxCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents _lblRecentItemsMaxCount As System.Windows.Forms.Label
    Friend WithEvents _tbpSnippets As System.Windows.Forms.TabPage
    Friend WithEvents _tbpExport As System.Windows.Forms.TabPage
    Friend WithEvents _cbxShowHiddenFiles As System.Windows.Forms.CheckBox
    Friend WithEvents _gbxHiddenCategories As System.Windows.Forms.GroupBox
    Friend WithEvents _tbpOutput As System.Windows.Forms.TabPage
    Friend WithEvents _numOutputLimit As System.Windows.Forms.NumericUpDown
    Friend WithEvents _lblOutputLimit As System.Windows.Forms.Label
    Friend WithEvents _btnSetDefaultFavoriteSnippets As System.Windows.Forms.Button
    Friend WithEvents _tbpFormats As System.Windows.Forms.TabPage
    Friend WithEvents _cmbFontSize As System.Windows.Forms.ComboBox
    Friend WithEvents _lblFontSize As System.Windows.Forms.Label
    Friend WithEvents _cmbFontFamily As System.Windows.Forms.ComboBox
    Friend WithEvents _lblFont As System.Windows.Forms.Label
    Friend WithEvents _cbxFontBold As System.Windows.Forms.CheckBox
    Friend WithEvents _btnForeColor As System.Windows.Forms.Button
    Friend WithEvents _btnBackColor As System.Windows.Forms.Button
    Friend WithEvents _lblForeColor As System.Windows.Forms.Label
    Friend WithEvents _lblBackColor As System.Windows.Forms.Label
    Friend WithEvents _lblItem As System.Windows.Forms.Label
    Friend WithEvents _lstFormatInfos As System.Windows.Forms.ListBox
    Friend WithEvents _cmbForeColor As Regexator.Windows.Forms.ColorComboBox
    Friend WithEvents _cmbBackColor As Regexator.Windows.Forms.ColorComboBox
    Friend WithEvents _lblGroup As System.Windows.Forms.Label
    Friend WithEvents _tbxSample As System.Windows.Forms.TextBox
    Friend WithEvents _lblSample As System.Windows.Forms.Label
    Friend WithEvents _tbpSymbols As System.Windows.Forms.TabPage
    Friend WithEvents _lstFormatGroups As System.Windows.Forms.ListBox
    Friend WithEvents _btnSetGroupDefaults As System.Windows.Forms.Button
    Friend WithEvents _btnSetGroupsDefaults As System.Windows.Forms.Button
    Friend WithEvents _btnSetInfoDefaults As System.Windows.Forms.Button
    Friend WithEvents _tbpProjectDefaultValues As System.Windows.Forms.TabPage
    Friend WithEvents _pnlDefaultValues As System.Windows.Forms.Panel
    Friend WithEvents _tbcProjectDefaultValues As System.Windows.Forms.TabControl
    Friend WithEvents _tbpPatternDefaults As System.Windows.Forms.TabPage
    Friend WithEvents _tbpReplacementDefaults As System.Windows.Forms.TabPage
    Friend WithEvents _tbpOutputDefaults As System.Windows.Forms.TabPage
    Friend WithEvents _gbxDefaultPatternOptions As System.Windows.Forms.GroupBox
    Friend WithEvents _gbxDefaultReplacementOptions As System.Windows.Forms.GroupBox
    Friend WithEvents _gbxDefaultOutputOptions As System.Windows.Forms.GroupBox
    Friend WithEvents _gbxReplacementNewLine As System.Windows.Forms.GroupBox
    Friend WithEvents _lblHiddenCategoriesInfo As System.Windows.Forms.Label
    Friend WithEvents _lstReplacementNewLine As System.Windows.Forms.ListBox
    Friend WithEvents _gbxDefaultRegexOptions As System.Windows.Forms.GroupBox
    Friend WithEvents _tbpInputDefaultValues As System.Windows.Forms.TabPage
    Friend WithEvents _gbxInputNewLine As System.Windows.Forms.GroupBox
    Friend WithEvents _lstInputNewLine As System.Windows.Forms.ListBox
    Friend WithEvents _gbxDefaultInputOptions As System.Windows.Forms.GroupBox
    Friend WithEvents _tbpRegexOptions As System.Windows.Forms.TabPage
    Friend WithEvents _btnSetGroupDefaultFont As System.Windows.Forms.Button
    Friend WithEvents _gbxRegexOptionsVisibility As System.Windows.Forms.GroupBox
    Friend WithEvents _tbpTextEditor As System.Windows.Forms.TabPage
    Friend WithEvents _cbxSelectEntireCurrentLineAfterLoad As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxPatternIndentNewLine As System.Windows.Forms.CheckBox
    Friend WithEvents _btnViewSnippetErrorLog As System.Windows.Forms.Button
    Friend WithEvents _cmbInputEncoding As System.Windows.Forms.ComboBox
    Friend WithEvents _btnSetDefaultInputEncoding As System.Windows.Forms.Button
    Friend WithEvents _gbxSnippetDirectories As System.Windows.Forms.GroupBox
    Friend WithEvents _gbxInputEncoding As System.Windows.Forms.GroupBox
    Friend WithEvents _pnlSnippetDirectories As System.Windows.Forms.Panel
    Friend WithEvents _cbxSetSnippetOptions As System.Windows.Forms.CheckBox
    Friend WithEvents _lblDefaultExportMode As System.Windows.Forms.Label
    Friend WithEvents _cmbExportMode As System.Windows.Forms.ComboBox
    Friend WithEvents _cbxCheckNewVersion As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxTrackActiveItem As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxConfirmFileInputRemoval As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxRegexOptionsDescriptionVisible As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxRegexOptionsHotkeyNumberVisible As System.Windows.Forms.CheckBox
    Friend WithEvents _gbxReplacementText As System.Windows.Forms.GroupBox
    Friend WithEvents _tbxReplacementText As Regexator.Windows.Forms.ExtendedTextBox
    Friend WithEvents _cbxShowStatusBar As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxMultiline As System.Windows.Forms.CheckBox
    Friend WithEvents _gbxOutputText As System.Windows.Forms.GroupBox
    Friend WithEvents _cbxHighlightBeforeAfterResult As System.Windows.Forms.CheckBox
    Friend WithEvents _cbxOutputOmitRepeatedInfo As System.Windows.Forms.CheckBox
    Friend WithEvents _cmbNumberAlignment As System.Windows.Forms.ComboBox
    Friend WithEvents _lblNumberAlignment As System.Windows.Forms.Label
    Friend WithEvents _tbpFileSystemSearch As System.Windows.Forms.TabPage
    Friend WithEvents _numFileSystemSearchMaxResultCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents _lblFileSystemSearchMaxResultCount As System.Windows.Forms.Label

End Class
