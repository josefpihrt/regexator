Imports System.Globalization
Imports Regexator
Imports Regexator.Snippets
Imports Regexator.Text
Imports Regexator.Windows.Forms
Imports Regexator.Collections.Generic

Public Class GuideForm

    Public Sub New()

        InitializeComponent()
        Padding = New Padding(1, 1, 1, 1)
        Text = My.Resources.Guide
        StartPosition = FormStartPosition.Manual
        KeyPreview = True
        Icon = My.Resources.IcoRegexator

        SetLocationAndSize(My.Settings.GuideFormWindowState, My.Settings.GuideFormLocation, My.Settings.GuideFormSize)

        Try
            _spcMain.SplitterDistance = My.Settings.GuideFormSplitterDistanceMain
        Catch ex As InvalidOperationException
            Debug.Assert(False)
        End Try

        BeginUpdate()
        Initialize()
        EndUpdate()

        _dicUrl = Data.RegexCategoryInfos.ToDictionary(Function(f) f.Category, Function(f) f.Url)
        _guideSearchManager = New GuideSearchManager(Data.GuideItems.ToArray()) With {.SearchInDescription = My.Settings.GuideSearchDescription}
        _snippetSearchManager = New SnippetSearchManager(Data.EnumerateSnippets().ToArray()) With {.SearchInDescription = My.Settings.GuideSearchDescription}


    End Sub

    Private Sub Initialize()

        Dim cms = New ContextMenuStrip()
        _trv = New GuideTreeView() With {.TabIndex = 1}

        AddHandler cms.Closed, Sub() _trv.ContextMenuStrip = Nothing

        AddHandler _trv.AfterSelect, AddressOf TreeView_AfterSelect
        AddHandler _trv.KeyDown, AddressOf TreeView_KeyDown
        AddHandler _trv.MouseDown, Sub(sender As Object, e As MouseEventArgs)
                                       If ContextMenuStrip Is Nothing AndAlso
                                               e.Button = System.Windows.Forms.MouseButtons.Right AndAlso
                                               _trv.GetNodeAt(e.Location) IsNot Nothing Then
                                           _trv.ContextMenuStrip = cms
                                       End If
                                   End Sub

        cms.Items.Add(New ToolStripMenuItem(My.Resources.OpenExternalHelp, My.Resources.IcoOpenLink.ToBitmap(), Sub()
                                                                                                                    Dim node = _trv.SelectedNode
                                                                                                                    If node IsNot Nothing Then
                                                                                                                        Dim url As Uri = Nothing
                                                                                                                        If _dicUrl.TryGetValue(_trv.CurrentRegexCategory, url) Then
                                                                                                                            Process.Start(url.ToString())
                                                                                                                        End If
                                                                                                                    End If
                                                                                                                End Sub))

        _tbxSearch = New ExtendedTextBox() With {.Dock = DockStyle.Top, .TabIndex = 0}
        AddHandler _tbxSearch.KeyDown, AddressOf SearchBox_KeyDown

        _tsp = New AppToolStrip()

        _btnSearchInDescription = New ToolStripButton(Nothing, My.Resources.IcoDescription.ToBitmap()) With {.CheckOnClick = True, .Checked = My.Settings.GuideSearchDescription, .ToolTipText = My.Resources.SearchInDescription}
        AddHandler _btnSearchInDescription.Click,
            Sub()
                My.Settings.GuideSearchDescription = Not My.Settings.GuideSearchDescription
                LoadTreeView()
            End Sub

        _tbc = New TabControl() With {.Dock = DockStyle.Fill, .Appearance = TabAppearance.FlatButtons, .TabIndex = 2}

        _tbpGuide = New TabPage(My.Resources.Overview)

        _pnlGuide = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}

        _dgvGuide = New GuideDataGridView()
        _dgvGuide.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "CategoryText", .HeaderText = My.Resources.Category})
        _dgvGuide.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "Text", .HeaderText = My.Resources.Text})
        _dgvGuide.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "Description", .HeaderText = My.Resources.Description})
        AddHandler _dgvGuide.KeyDown, AddressOf GuideDataGridView_KeyDown

        _tbpSnippets = New TabPage(My.Resources.Snippets)

        _pnlSnippets = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}

        Dim style = New DataGridViewCellStyle()
        _dgvSnippets = New GuideDataGridView() With {.ReadOnly = False}
        _dgvSnippets.Columns.Add(New DataGridViewCheckBoxColumn() With {.DataPropertyName = "Favorite", .HeaderText = ChrW(&H2605)})
        _dgvSnippets.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "CategoryText", .HeaderText = My.Resources.Category})
        _dgvSnippets.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "Title", .HeaderText = My.Resources.Title})
        _dgvSnippets.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "CleanCode", .HeaderText = My.Resources.Code, .DefaultCellStyle = style})
        _dgvSnippets.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "ExtendedCleanCode", .HeaderText = My.Resources.ExtendedCode, .DefaultCellStyle = style})
        _dgvSnippets.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "Description", .HeaderText = My.Resources.Description})
        AddHandler _dgvSnippets.KeyDown, AddressOf SnippetDataGridView_KeyDown

        style.ApplyStyle(_dgvSnippets.DefaultCellStyle)
        style.WrapMode = DataGridViewTriState.True

        _tbpBrowser = New TabPage(My.Resources.Online)

        _tspBrowser = New ExtendedToolStrip() With {.Dock = DockStyle.Top, .ButtonBackColor = Color.Empty, .ItemAutoSize = False, .FirstItemHasLeftMargin = False, .RenderMode = ToolStripRenderMode.Professional, .Renderer = New AppToolStripRenderer()}

        _btnOpenInBrowser = New ToolStripButton(Nothing, My.Resources.IcoOpenLink.ToBitmap(), Sub() _web.OpenCurrentUrlInBrowser()) With {.ToolTipText = My.Resources.Open}

        _web = New GuideWebBrowser() With {.Dock = DockStyle.Fill, .ScriptErrorsSuppressed = True}
        AddHandler _web.StatusTextChanged, Sub() _lblBrowserStatus.Text = _web.StatusText

        _stsBrowser = New StatusStrip() With {.BackColor = SystemColors.Control}

        _lblBrowserStatus = New ToolStripStatusLabel() With {.TextAlign = ContentAlignment.MiddleLeft, .Spring = True}

        _tsp.Items.Add(_btnSearchInDescription)

        _spcMain.Panel1.Controls.Add(_trv)
        _spcMain.Panel1.Controls.Add(_tbxSearch)
        _spcMain.Panel1.Controls.Add(_tsp)

        _pnlGuide.Controls.Add(_dgvGuide)
        _tbpGuide.Controls.Add(_pnlGuide)

        _pnlSnippets.Controls.Add(_dgvSnippets)
        _tbpSnippets.Controls.Add(_pnlSnippets)

        _stsBrowser.Items.Add(_lblBrowserStatus)
        _tspBrowser.Items.Add(_btnOpenInBrowser)

        _tbpBrowser.Controls.Add(_stsBrowser)
        _tbpBrowser.Controls.Add(_web)
        _tbpBrowser.Controls.Add(_tspBrowser)

        _tbc.TabPages.Add(_tbpGuide)
        _tbc.TabPages.Add(_tbpSnippets)
        _tbc.TabPages.Add(_tbpBrowser)

        _spcMain.Panel2.Controls.Add(_tbc)
        AddHandler _tbc.SelectedIndexChanged, Sub() LoadSelected()

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        LoadTreeView()
        MyBase.OnLoad(e)

    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)

        ActiveControl = _tbxSearch
        MyBase.OnShown(e)

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        My.Settings.GuideFormLocation = Location
        My.Settings.GuideFormSize = Size
        My.Settings.GuideFormWindowState = WindowState
        My.Settings.GuideFormSplitterDistanceMain = _spcMain.SplitterDistance
        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.F Then
                ActiveControl = _tbxSearch
                _tbxSearch.SelectAll()
                e.SuppressKeyPress = True
            End If
        End If
        MyBase.OnKeyDown(e)

    End Sub

    Private Sub TreeView_AfterSelect(sender As Object, e As TreeViewEventArgs)

        _dgvGuide.DataSource = Nothing
        _dgvSnippets.DataSource = Nothing
        _web.Navigate(New Uri("about:blank"))
        _flgBrowser = True
        LoadSelected()

    End Sub

    Private Sub LoadSelected()

        If IsGuideTabSelected AndAlso _dgvGuide.DataSource Is Nothing Then
            LoadGuideItems()
        ElseIf IsSnippetTabSelected AndAlso _dgvSnippets.DataSource Is Nothing Then
            LoadSnippets()
        ElseIf IsBrowserTabSelected AndAlso _flgBrowser Then
            Dim url As Uri = Nothing
            If _dicUrl.TryGetValue(_trv.CurrentRegexCategory, url) Then
                _web.Navigate(url)
            End If
            _flgBrowser = False
        End If
        SetNodesText()

    End Sub

    Private Sub SetNodesText()

        Dim dic = If(IsSnippetTabSelected, _snippetCategories, _guideCategories)
        Dim none = RegexCategory.None.ToString()
        For Each node As TreeNode In _trv.Nodes
            Dim category = node.Tag.ToString()
            Dim isNone = String.Equals(category, none)
            Dim bold As Boolean = False
            Dim count As Integer = 0
            If isNone Then
                bold = True
                count = dic.Sum(Function(f) f.Value)
            ElseIf dic.TryGetValue(category, count) Then
                bold = True
            End If
            node.NodeFont = New Font(_trv.Font, If(bold, FontStyle.Bold, FontStyle.Regular))
            node.Text = If(isNone, My.Resources.AllCategories, TextUtility.SplitCamelCase(category))
            node.Text &= " " & count.ToString(CultureInfo.CurrentCulture).AddParentheses()
        Next

    End Sub

    Private Sub LoadGuideItems()

        Dim category = _trv.CurrentRegexCategory
        Dim flg = (category = RegexCategory.None)
        Dim items = _guideSearchManager.FilteredItems _
            .Where(Function(f) flg OrElse f.Category = category) _
            .OrderBy(Function(f) f.CategoryText) _
            .ThenBy(Function(f) f.Text)
        _dgvGuide.DataSource = New SortableBindingList(Of GuideItem)(items.ToList())

    End Sub

    Private Sub LoadSnippets()

        Dim category = _trv.CurrentRegexCategory
        Dim flg = (category = RegexCategory.None)
        Dim items = _snippetSearchManager.FilteredItems _
            .Where(Function(f) flg OrElse String.Equals(f.Category, _trv.CurrentCategory)) _
            .OrderBy(Function(f) f.FullName)
        _dgvSnippets.DataSource = New SortableBindingList(Of RegexSnippet)(items.ToList())

    End Sub

    Private Sub LoadTreeView()

        Dim value = _tbxSearch.Text.Trim()
        _guideSearchManager.SearchInDescription = My.Settings.GuideSearchDescription
        _guideSearchManager.FilterValue = value
        _snippetSearchManager.SearchInDescription = My.Settings.GuideSearchDescription
        _snippetSearchManager.FilterValue = value
        _guideCategories = _guideSearchManager.FilteredItems _
            .GroupBy(Function(f) f.Category) _
            .ToDictionary(Function(f) f.Key.ToString(), Function(f) f.Count())
        _snippetCategories = _snippetSearchManager.FilteredItems _
            .GroupBy(Function(f) f.Category) _
            .ToDictionary(Function(f) f.Key.ToString(), Function(f) f.Count())
        Dim nodes = _guideCategories.Keys _
            .Union(_snippetCategories.Keys) _
            .Distinct() _
            .Select(Function(f) New TreeNode(TextUtility.SplitCamelCase(f)) With {.Tag = f}) _
            .OrderBy(Function(f) f.Text) _
            .ToArray()
        _trv.BeginUpdate()
        Dim currentCategory = _trv.CurrentCategory
        _trv.Nodes.Clear()
        Dim firstNode = New TreeNode(My.Resources.AllCategories.ToUpper(CultureInfo.CurrentCulture)) With {.Tag = RegexCategory.None.ToString()}
        _trv.Nodes.Add(firstNode)
        _trv.Nodes.AddRange(nodes)
        _trv.SelectedNode = If(nodes.FirstOrDefault(Function(f) String.Equals(f.Tag.ToString(), currentCategory)), firstNode)
        _trv.ExpandAll()
        _trv.EndUpdate()

    End Sub

    Private Sub SearchBox_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Down Then
                ActiveControl = _trv
            ElseIf e.KeyCode = Keys.Enter Then
                LoadTreeView()
            ElseIf e.KeyCode = Keys.Escape Then
                LoadTreeView()
            End If
        End If

    End Sub

    Private Sub TreeView_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Up Then
                If _trv.SelectedNode IsNot Nothing AndAlso _trv.SelectedNode.Equals(_trv.FirstNodeOrDefault) Then
                    ActiveControl = _tbxSearch
                End If
            ElseIf e.KeyCode = Keys.Right Then
                If IsGuideTabSelected Then
                    ActiveControl = _dgvGuide
                ElseIf IsSnippetTabSelected Then
                    ActiveControl = _dgvSnippets
                End If
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub GuideDataGridView_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Left Then
                ActiveControl = _trv
            End If
        End If

    End Sub

    Private Sub SnippetDataGridView_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.D Then
                For Each item In _dgvSnippets.SelectedRows.Cast(Of DataGridViewRow).Select(Function(f) DirectCast(f.DataBoundItem, RegexSnippet))
                    item.Favorite = Not item.Favorite
                Next
            End If
        ElseIf e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Left Then
                ActiveControl = _trv
            End If
        End If

    End Sub

    Private ReadOnly Property IsGuideTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbc.SelectedTab, _tbpGuide)
        End Get
    End Property

    Private ReadOnly Property IsBrowserTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbc.SelectedTab, _tbpBrowser)
        End Get
    End Property

    Private ReadOnly Property IsSnippetTabSelected As Boolean
        Get
            Return ReferenceEquals(_tbc.SelectedTab, _tbpSnippets)
        End Get
    End Property

    Private _tbxSearch As ExtendedTextBox
    Private _trv As GuideTreeView
    Private _tbc As TabControl

    Private _tsp As AppToolStrip
    Private _btnSearchInDescription As ToolStripButton

    Private _tbpGuide As TabPage
    Private _pnlGuide As Panel
    Private _dgvGuide As GuideDataGridView

    Private _tbpBrowser As TabPage
    Private _web As GuideWebBrowser
    Private _stsBrowser As StatusStrip
    Private _lblBrowserStatus As ToolStripStatusLabel

    Private _tspBrowser As ExtendedToolStrip
    Private _btnOpenInBrowser As ToolStripButton

    Private _tbpSnippets As TabPage
    Private _pnlSnippets As Panel
    Private _dgvSnippets As GuideDataGridView

    Private ReadOnly _dicUrl As Dictionary(Of RegexCategory, Uri)
    Private _flgBrowser As Boolean
    Private ReadOnly _guideSearchManager As GuideSearchManager
    Private ReadOnly _snippetSearchManager As SnippetSearchManager
    Private _guideCategories As New Dictionary(Of String, Integer)
    Private _snippetCategories As New Dictionary(Of String, Integer)

    Private Class GuideWebBrowser
        Inherits WebBrowser

        Public Sub OpenCurrentUrlInBrowser()

            Dim url = Me.Url
            If url IsNot Nothing AndAlso url.IsAbsoluteUri AndAlso (url.Scheme = Uri.UriSchemeHttp OrElse url.Scheme = Uri.UriSchemeHttps) Then
                Process.Start(url.ToString())
            End If

        End Sub

    End Class

    Private Class GuideDataGridView
        Inherits ExtendedDataGridView

        Public Sub New()

            Dock = DockStyle.Fill
            BorderStyle = BorderStyle.None
            Me.ReadOnly = True
            RowHeadersVisible = True
            AllowUserToAddRows = False
            AllowUserToDeleteRows = False
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
            MultiSelect = True
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            AutoGenerateColumns = False
            _cmsCell = New System.Windows.Forms.ContextMenuStrip()

        End Sub

        Protected Overrides Sub OnCellContextMenuStripNeeded(e As DataGridViewCellContextMenuStripNeededEventArgs)

            If e Is Nothing Then Throw New ArgumentNullException("e")
            If e.RowIndex <> -1 AndAlso e.ColumnIndex <> -1 Then
                Dim cell = Item(e.ColumnIndex, e.RowIndex)
                If cell.FormattedValue IsNot Nothing AndAlso String.IsNullOrEmpty(cell.FormattedValue.ToString()) = False Then
                    _cmsCell.Items.Clear()
                    _cmsCell.Items.Add(New ToolStripMenuItem(My.Resources.CopyCellValue, My.Resources.IcoCopy.ToBitmap(), AddressOf CopyCellValueItem_Click) With {.Tag = cell})
                    e.ContextMenuStrip = _cmsCell
                End If
            End If
            MyBase.OnCellContextMenuStripNeeded(e)

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

        Protected Overrides Sub OnColumnHeaderMouseClick(e As DataGridViewCellMouseEventArgs)

            CurrentCell = Nothing
            _horizontalScrollingOffset = HorizontalScrollingOffset
            MyBase.OnColumnHeaderMouseClick(e)

        End Sub

        Protected Overrides Sub OnSorted(e As EventArgs)

            HorizontalScrollingOffset = _horizontalScrollingOffset
            MyBase.OnSorted(e)

        End Sub

        Protected Overrides Sub OnColumnAdded(e As DataGridViewColumnEventArgs)

            If e Is Nothing Then Throw New ArgumentNullException("e")
            e.Column.SortMode = DataGridViewColumnSortMode.Automatic
            e.Column.ReadOnly = Not (e.Column.DataPropertyName = "Favorite")
            MyBase.OnColumnAdded(e)

        End Sub

        Protected Overrides Sub OnCurrentCellDirtyStateChanged(e As EventArgs)

            If CurrentCell IsNot Nothing AndAlso CurrentCell.OwningColumn.DataPropertyName = "Favorite" Then
                CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
            MyBase.OnCurrentCellDirtyStateChanged(e)

        End Sub

        Protected Overrides Sub OnDataBindingComplete(e As DataGridViewBindingCompleteEventArgs)

            AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells, False)
            AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells, True)
            MyBase.OnDataBindingComplete(e)

        End Sub

        Private ReadOnly _cmsCell As ContextMenuStrip
        Private _horizontalScrollingOffset As Integer

    End Class

    Private Class GuideTreeView
        Inherits ExtendedTreeView

        Public Sub New()

            Dock = DockStyle.Fill
            HideSelection = False
            ShowLines = False
            FullRowSelect = True
            BorderStyle = BorderStyle.FixedSingle
            ShowPlusMinus = False

        End Sub

        Protected Overrides Sub OnBeforeCollapse(e As TreeViewCancelEventArgs)

            If e Is Nothing Then Throw New ArgumentNullException("e")
            e.Cancel = True
            MyBase.OnBeforeCollapse(e)

        End Sub

        Protected Overrides Sub OnHandleCreated(e As EventArgs)

            MyBase.OnHandleCreated(e)
#If DEBUG Then
            Debug.Assert(SetWindowTheme(Handle, "explorer", Nothing) <> 1)
#Else
        NativeMethods.SetWindowTheme(Me.Handle, "explorer", Nothing)
#End If

        End Sub

        Public ReadOnly Property CurrentRegexCategory As RegexCategory
            Get
                If SelectedNode IsNot Nothing Then
                    Dim category As RegexCategory
                    If [Enum].TryParse(SelectedNode.Tag.ToString(), category) Then
                        Return category
                    End If
                    Return RegexCategory.Custom
                End If
                Return RegexCategory.None
            End Get
        End Property

        Public ReadOnly Property CurrentCategory As String
            Get
                If SelectedNode IsNot Nothing Then
                    Return SelectedNode.Tag.ToString()
                End If
                Return Nothing
            End Get
        End Property

    End Class

End Class