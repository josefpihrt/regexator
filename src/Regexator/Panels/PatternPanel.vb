Imports System.Diagnostics.CodeAnalysis
Imports System.Text.RegularExpressions
Imports Regexator
Imports Regexator.Output
Imports Regexator.Text.RegularExpressions
Imports Regexator.Windows.Forms

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class PatternPanel

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Public Sub New()

        _rtb = New PatternRichTextBox()
        _tsp = New AppToolStrip()
        _tbxCurrentLine = New CurrentLineBox(_rtb)
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .Enabled = False, .BackColor = _rtb.BackColor}
        _tspFind = New AppSearchToolStrip(_rtb)

        CreateToolStripItems()

        AddHandler _ddbExportImport.DropDownOpening, AddressOf ExportImport_DropDownOpening
        AddHandler _rtb.KeyDown, AddressOf RichTextBox_KeyDown
        AddHandler _rtb.TextChanged, AddressOf RichTextBox_TextChanged
        AddHandler _btnSave.Click, Sub() Explorer.SavePattern()
        AddHandler _btnUndo.Click, AddressOf Undo_Click
        AddHandler _btnRedo.Click, AddressOf Redo_Click
        AddHandler _rtb.CurrentTextChanged, AddressOf RichTextBox_CurrentTextChanged
        AddHandler _rtb.CurrentLineOnlyChanged, AddressOf RichTextBox_CurrentLineOnlyChanged
        AddHandler _rtb.SelectionOnlyChanged, Sub() _btnSelectionOnly.Checked = _rtb.SelectionOnly
        AddHandler _btnCurrentLineOnly.CheckedChanged, Sub() _rtb.CurrentLineOnly = _btnCurrentLineOnly.Checked
        AddHandler _btnSelectionOnly.CheckedChanged, Sub() _rtb.SelectionOnly = _btnSelectionOnly.Checked
        AddHandler _ddbFavoriteSnippets.MouseDown, AddressOf FavoriteDropDownButton_MouseDown
        AddHandler _rtb.Enter, AddressOf RichTextBox_Enter
        AddHandler _ddbFavoriteSnippets.DropDownOpening, Sub() _ddbFavoriteSnippets.DropDownItems.LoadItems(_rtb.Sense.Factory.CreateItems(Data.EnumerateFavoriteSnippets()))

        _tsp.Items.AddRange(EnumerateItems().ToArray())
        _pnlBox.Controls.Add(_rtb)

        _pnl.Controls.AddRange({_pnlBox, _tbxCurrentLine, _tspFind, _tsp})

        App.Formats.Text.Controls.Add(_rtb)
        App.Formats.Text.Controls.Add(_pnl)
        App.Formats.CurrentLineText.Controls.Add(_tbxCurrentLine)

    End Sub

    Private Sub CreateToolStripItems()

        _btnSave = New ToolStripButton(Nothing, My.Resources.IcoSave.ToBitmap()) With {.ToolTipText = My.Resources.SavePattern & " " & My.Resources.CtrlS.AddParentheses()}
        _btnUndo = New ToolStripButton(Nothing, My.Resources.IcoArrowLeft.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Undo & " " & My.Resources.CtrlZ.AddParentheses()}
        _btnRedo = New ToolStripButton(Nothing, My.Resources.IcoArrowRight.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Redo & " " & My.Resources.CtrlY.AddParentheses()}
        _btnSelectionOnly = New ToolStripButton(Nothing, My.Resources.IcoSelection.ToBitmap()) With {.ToolTipText = My.Resources.ToggleSelectionMode & " " & My.Resources.CtrlT.AddParentheses(), .CheckOnClick = True}
        _btnCurrentLineOnly = New ToolStripButton(Nothing, My.Resources.IcoCurrent.ToBitmap()) With {.ToolTipText = My.Resources.ToggleCurrentLineMode & " " & My.Resources.CtrlR.AddParentheses(), .CheckOnClick = True}
        _ddbFavoriteSnippets = New ToolStripDropDownButton(Nothing, My.Resources.IcoSnippet.ToBitmap()) With {.ToolTipText = My.Resources.FavoriteSnippets}
        _ddbExportImport = New ToolStripDropDownButton(Nothing, My.Resources.IcoExportImport.ToBitmap()) With {.ToolTipText = My.Resources.ExportImport}
        _ddbExportImport.DropDownItems.Add(New ToolStripSeparator())
        _btnWordWrap = New ToolStripButton(Nothing, My.Resources.IcoWordWrap.ToBitmap(), Sub() ToggleWordWrap()) With {.ToolTipText = My.Resources.ToggleWordWrap, .Checked = _rtb.WordWrap}

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _btnSave
        Yield New ToolStripSeparator()
        Yield _btnUndo
        Yield _btnRedo
        Yield New ToolStripSeparator()
        Yield _btnCurrentLineOnly
        Yield _btnSelectionOnly
        Yield _ddbFavoriteSnippets
        Yield _ddbExportImport
        Yield New ToolStripSeparator()
        Yield _btnWordWrap

    End Function

    Private Sub ExportImport_DropDownOpening(sender As Object, e As EventArgs)

        Dim items = _ddbExportImport.DropDownItems
        items.Clear()
        items.AddRange(_rtb.Exporter.CreateItems().ToArray())
        items.AddSeparator()
        items.AddRange(_rtb.Exporter.CreateDefaultItems().ToArray())
        items.AddSeparator()
        items.Add(Exporter.CreateDefaultModeItem())

    End Sub

    Private Sub RichTextBox_Enter(sender As Object, e As EventArgs)

        If My.Settings.TrackActiveItemInSolutionExplorer Then
            Explorer.SelectCurrentProjectNode()
        End If

    End Sub

    Public Sub Load(container As ProjectContainer)

        _rtb.BeginUpdate()
        Dim pattern = If(container, New ProjectContainer()).Pattern
        Attributes = If(container IsNot Nothing, container.Attributes, ItemAttributes.None)
        PatternOptions = pattern.PatternOptions
        Text = pattern.Text
        If pattern.CurrentLine >= _rtb.Lines.Length Then
            pattern.CurrentLine = 0
        End If
        If CurrentLineOnly Then
            If My.Settings.SelectEntireCurrentLineAfterLoad Then
                _rtb.SelectLine(pattern.CurrentLine, True)
            Else
                _rtb.SelectLineStart(pattern.CurrentLine)
            End If
        End If
        _pnl.Enabled = container IsNot Nothing
        _rtb.EndUpdate()

    End Sub

    Public Sub GoToCursor()

        _rtb.GoToCursor()

    End Sub

    Public Sub ToggleWordWrap()

        WordWrap = Not WordWrap

    End Sub

    Private Sub FavoriteDropDownButton_MouseDown(sender As Object, e As MouseEventArgs)

        If e.Button = MouseButtons.Right Then
            DirectCast(sender, ToolStripDropDownButton).ShowDropDown()
        End If

    End Sub

    Public Sub CreateRegex()

        _regexInfo = CreateRegexInfo(CurrentText, App.RegexOptionsManager.Value And Not RegexOptions.Compiled)

        Groups.Load(If(_regexInfo IsNot Nothing, _regexInfo.Groups.ToArray(), New GroupInfo() {}))

        App.MainForm._tslGroups.Text = " " & String.Join(", ", Groups.GroupNames().OrderBy(Function(f) f))

        Panels.Output.LoadData()

    End Sub

    Private Function CreateRegexInfo(pattern As String, options As RegexOptions) As RegexInfo

        _exception = Nothing

        Try
            Return New RegexInfo(New Regex(pattern, options))
        Catch ex As NullReferenceException
            _exception = ex
        Catch ex As IndexOutOfRangeException
            _exception = ex
        Catch ex As ArgumentException
            _exception = ex
        End Try

        Return Nothing

    End Function

    Private Sub RichTextBox_CurrentTextChanged(sender As Object, e As EventArgs)

        SetCurrentLineText()
#If TATA Then
        App.FormattedPanel.LoadText(Me.CurrentText)
#End If
        CreateRegex()
        If ReferenceEquals(App.MainForm._tbcOther.SelectedTab, App.MainForm._tbpExport) Then
            Export.PatternText = CurrentText
        End If

    End Sub

    Private Sub RichTextBox_CurrentLineOnlyChanged(sender As Object, e As EventArgs)

        _btnCurrentLineOnly.Checked = _rtb.CurrentLineOnly
        _btnSelectionOnly.Enabled = Not _rtb.CurrentLineOnly
        SetCurrentLineText()
        _tbxCurrentLine.Visible = _rtb.CurrentLineOnly

    End Sub

    Private Sub RichTextBox_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.S Then
                Explorer.SavePattern()
            End If
        End If

    End Sub

    Private Sub RichTextBox_TextChanged(sender As Object, e As EventArgs)

        _btnUndo.Enabled = _rtb.CanUndo
        _btnRedo.Enabled = _rtb.CanRedo

    End Sub

    Private Sub SetCurrentLineText()

        _tbxCurrentLine.Text = If(CurrentLineOnly, CurrentText, String.Empty)

    End Sub

    Private Sub Undo_Click(sender As Object, e As EventArgs)

        _rtb.Undo()
        _btnUndo.Enabled = _rtb.CanUndo
        _btnRedo.Enabled = _rtb.CanRedo

    End Sub

    Private Sub Redo_Click(sender As Object, e As EventArgs)

        _rtb.Redo()
        _btnUndo.Enabled = _rtb.CanUndo
        _btnRedo.Enabled = _rtb.CanRedo

    End Sub

    Public Property CurrentLineOnly As Boolean
        Get
            Return _rtb.CurrentLineOnly
        End Get
        Set(value As Boolean)
            _rtb.CurrentLineOnly = value
        End Set
    End Property

    Public Property SelectionOnly As Boolean
        Get
            Return _rtb.SelectionOnly
        End Get
        Set(value As Boolean)
            _rtb.SelectionOnly = value
        End Set
    End Property

    Public ReadOnly Property CurrentText As String
        Get
            Return _rtb.CurrentText
        End Get
    End Property

    Public Property Text As String
        Get
            Return _rtb.Text
        End Get
        Set(value As String)
            _rtb.Text = value
        End Set
    End Property

    Public Property PatternOptions As PatternOptions
        Get
            Dim value As PatternOptions = PatternOptions.None
            If CurrentLineOnly Then
                value = value Or PatternOptions.CurrentLineOnly
            End If
            Return value
        End Get
        Set(value As PatternOptions)
            CurrentLineOnly = ((value And Regexator.PatternOptions.CurrentLineOnly) = Regexator.PatternOptions.CurrentLineOnly)
        End Set
    End Property

    Public ReadOnly Property CurrentLine As Integer
        Get
            Return _rtb.CurrentLine
        End Get
    End Property

    Public Property Attributes As ItemAttributes
        Get
            Return _attributes
        End Get
        Set(value As ItemAttributes)
            _attributes = value
        End Set
    End Property

    Public ReadOnly Property RegexInfo As RegexInfo
        Get
            Return _regexInfo
        End Get
    End Property

    Public ReadOnly Property Exception As Exception
        Get
            Return _exception
        End Get
    End Property

    Public Property WordWrap As Boolean
        Get
            Return _rtb.WordWrap
        End Get
        Set(value As Boolean)
            If _rtb.WordWrap <> value Then
                _rtb.WordWrap = value
                _btnWordWrap.Checked = value
                _btnUndo.Enabled = _rtb.CanUndo
                _btnRedo.Enabled = _rtb.CanRedo
                SetCurrentLineText()
            End If
        End Set
    End Property

    Private _regexInfo As RegexInfo
    Private _exception As Exception
    Private _attributes As ItemAttributes

    Friend _rtb As PatternRichTextBox
    Friend _tsp As AppToolStrip
    Friend _tbxCurrentLine As CurrentLineBox
    Friend _pnlBox As Panel
    Friend _pnl As Panel
    Friend _tspFind As AppSearchToolStrip

    Friend _btnSave As ToolStripButton
    Friend _btnUndo As ToolStripButton
    Friend _btnRedo As ToolStripButton
    Friend _btnSelectionOnly As ToolStripButton
    Friend _btnCurrentLineOnly As ToolStripButton
    Friend _ddbFavoriteSnippets As ToolStripDropDownButton
    Friend _ddbExportImport As ToolStripDropDownButton
    Friend _btnWordWrap As ToolStripButton

End Class