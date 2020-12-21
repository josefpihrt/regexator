Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Text
Imports Regexator
Imports Regexator.Output
Imports Regexator.Text
Imports Regexator.Windows.Forms

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class InputPanel

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Public Sub New()

        _rtb = New InputRichTextBox() With {.NewLineMode = My.Settings.DefaultInputNewLine}
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _tsp = New AppToolStrip()
        _tbxCurrentLine = New CurrentLineBox(_rtb)
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .Enabled = False, .BackColor = _rtb.BackColor}
        _tspFind = New AppSearchToolStrip(_rtb)

        _tssSave = New ToolStripSplitButton(Nothing, My.Resources.IcoSave.ToBitmap()) With {.ToolTipText = My.Resources.SaveInput & " " & My.Resources.CtrlS.AddParentheses()}
        _btnUndo = New ToolStripButton(Nothing, My.Resources.IcoArrowLeft.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Undo & " " & My.Resources.CtrlZ.AddParentheses()}
        _btnRedo = New ToolStripButton(Nothing, My.Resources.IcoArrowRight.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Redo & " " & My.Resources.CtrlY.AddParentheses()}
        _tssUndo = New ToolStripSeparator() With {.Visible = False}
        _btnHighlight = New ToolStripButton(Nothing, My.Resources.IcoPaintRoller.ToBitmap()) With {.ToolTipText = My.Resources.ToggleHighlighting, .CheckOnClick = True}
        _btnSelectionOnly = New ToolStripButton(Nothing, My.Resources.IcoSelection.ToBitmap()) With {.ToolTipText = My.Resources.ToggleSelectionMode & " " & My.Resources.CtrlT.AddParentheses(), .CheckOnClick = True}
        _btnCurrentLineOnly = New ToolStripButton(Nothing, My.Resources.IcoCurrent.ToBitmap()) With {.ToolTipText = My.Resources.ToggleCurrentLineMode & " " & My.Resources.CtrlR.AddParentheses(), .CheckOnClick = True}
        _lblIndex = New ToolStripLabel() With {.Alignment = ToolStripItemAlignment.Right}
        _lblLine = New ToolStripLabel() With {.Alignment = ToolStripItemAlignment.Right}
        _lblLines = New ToolStripLabel() With {.Alignment = ToolStripItemAlignment.Right}
        _btnNewLine = New ToolStripButton(Nothing, My.Resources.IcoCrLf.ToBitmap(), Sub() ToggleNewLine()) With {.ToolTipText = My.Resources.ToggleNewLine & " " & My.Resources.CtrlShiftN.AddParentheses()}
        _btnNewLine.Image = If(My.Settings.DefaultInputNewLine = NewLineMode.CrLf, My.Resources.IcoCrLf.ToBitmap(), My.Resources.IcoLf.ToBitmap())
        _btnWordWrap = New ToolStripButton(Nothing, My.Resources.IcoWordWrap.ToBitmap(), Sub() ToggleWordWrap()) With {.ToolTipText = My.Resources.ToggleWordWrap, .Checked = _rtb.WordWrap}

        SetIndexAndLineInfo(0, 0)
        SetLinesInfo(1)
        Encoding = Input.DefaultEncoding

        AddHandler _tssSave.DropDownOpening, AddressOf Save_DropDownOpening
        AddHandler _tssSave.ButtonClick, Sub() Explorer.SaveInput()
        AddHandler _btnUndo.Click, AddressOf Undo_Click
        AddHandler _btnRedo.Click, AddressOf Redo_Click
        AddHandler _btnHighlight.Click, Sub() HighlightEnabled = _btnHighlight.Checked
        AddHandler _rtb.TextChanged, AddressOf RichTextBox_TextChanged
        AddHandler _rtb.SelectionChanged, AddressOf RichTextBox_SelectionChanged
        AddHandler _rtb.KeyDown, AddressOf RichTextBox_KeyDown
        AddHandler _rtb.CurrentTextChanged, AddressOf RichTextBox_CurrentTextChanged
        AddHandler _rtb.CurrentLineOnlyChanged, AddressOf RichTextBox_CurrentLineOnlyChanged
        AddHandler _rtb.SelectionOnlyChanged, Sub() _btnSelectionOnly.Checked = _rtb.SelectionOnly
        AddHandler _btnCurrentLineOnly.CheckedChanged, Sub() _rtb.CurrentLineOnly = _btnCurrentLineOnly.Checked
        AddHandler _btnSelectionOnly.CheckedChanged, Sub() _rtb.SelectionOnly = _btnSelectionOnly.Checked
        AddHandler _rtb.Enter, AddressOf RichTextBox_Enter
        AddHandler _rtb.NewLineModeChanged, Sub()
                                                _btnNewLine.Image = If(NewLineMode = NewLineMode.Lf, My.Resources.IcoLf.ToBitmap(), My.Resources.IcoCrLf.ToBitmap())
                                                SetIndexAndLineInfo()
                                            End Sub

        _tsp.Items.AddRange(EnumerateItems().ToArray())
        _pnlBox.Controls.Add(_rtb)

        _pnl.Controls.AddRange({_pnlBox, _tbxCurrentLine, _tspFind, _tsp})

        App.Formats.Text.Controls.Add(_rtb)
        App.Formats.Text.Controls.Add(_pnl)
        App.Formats.CurrentLineText.Controls.Add(_tbxCurrentLine)

    End Sub

    Private Sub Save_DropDownOpening(sender As Object, e As EventArgs)

        If _tssSave.DropDownItems.Count = 0 Then
            _tssSave.DropDownItems.AddRange(CreateSaveItems().ToArray())
        End If

    End Sub

    Private Iterator Function CreateSaveItems() As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.SaveText, Nothing, Sub() Explorer.SaveInputText())
        Yield New ToolStripMenuItem(My.Resources.SaveOutputText, Nothing, Sub() Commands.SaveOutputText())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.SaveAs & My.Resources.EllipsisStr, Nothing, Sub() Explorer.SaveInputAs(InputKind.Included))
        Yield New ToolStripMenuItem(My.Resources.SaveAsFile & My.Resources.EllipsisStr, Nothing, Sub() Explorer.SaveInputAs(InputKind.File))

    End Function

    Private Sub RichTextBox_Enter(sender As Object, e As EventArgs)

        If My.Settings.TrackActiveItemInSolutionExplorer Then
            Explorer.SelectCurrentInputNode()
        End If

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _tssSave
        Yield New ToolStripSeparator()
        Yield _btnUndo
        Yield _btnRedo
        Yield _tssUndo
        Yield _btnHighlight
        Yield _btnCurrentLineOnly
        Yield _btnSelectionOnly
        Yield _btnNewLine
        Yield New ToolStripSeparator()
        Yield _btnWordWrap
        Yield _lblLines
        Yield _lblLine
        Yield _lblIndex

    End Function

    Public Sub Unload()

        Load(Nothing)

    End Sub

    Public Sub Load(input As Input)

        _rtb.BeginUpdate()
        Me.Input = If(input, New Input())
        If input IsNot Nothing Then
            If input.CurrentLine >= _rtb.Lines.Length Then
                input.CurrentLine = 0
            End If
            If CurrentLineOnly Then
                If My.Settings.SelectEntireCurrentLineAfterLoad Then
                    _rtb.SelectLine(input.CurrentLine, True)
                Else
                    _rtb.SelectLineStart(input.CurrentLine)
                End If
            End If
        End If
        _pnl.Enabled = input IsNot Nothing
        _rtb.EndUpdate()

    End Sub

    Public Sub GoToCursor()

        _rtb.GoToCursor()

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

    Public Sub ToggleNewLine()

        _rtb.ToggleNewLineMode()

    End Sub

    Public Sub ToggleWordWrap()

        WordWrap = Not WordWrap

    End Sub

    Private Sub RichTextBox_CurrentTextChanged(sender As Object, e As EventArgs)

        If Suppressed = False Then
            _inCurrentTextChanged = True
            SetCurrentLineText()
            Panels.Output.LoadData()
            _inCurrentTextChanged = False
        End If

    End Sub

    Private Sub RichTextBox_CurrentLineOnlyChanged(sender As Object, e As EventArgs)

        _btnCurrentLineOnly.Checked = _rtb.CurrentLineOnly
        _btnSelectionOnly.Enabled = Not _rtb.CurrentLineOnly
        SetCurrentLineText()
        _tbxCurrentLine.Visible = _rtb.CurrentLineOnly

    End Sub

    <SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")>
    Public Sub Highlight(block As RegexBlock)

        If block Is Nothing Then Throw New ArgumentNullException("block")
        If Suppressed = False Then
            Suppressed = True
            Dim restore As Boolean = _inCurrentTextChanged OrElse _rtb.SelectionOnlyActive OrElse HighlightEnabled = False
            Using blocker As New ScrollBlocker(_rtb)
                Dim selection = block.InputSpan.Offset(_rtb.IndexOffset)
                If restore Then
                    blocker.Restore()
                ElseIf selection IsNot Nothing Then
                    blocker.Restore(selection)
                    If HighlightEnabled Then
                        _rtb.Select(selection)
                    End If
                End If
            End Using
            Suppressed = False
        End If

    End Sub

    Private Sub RichTextBox_TextChanged(sender As Object, e As EventArgs)

        _btnUndo.Enabled = _rtb.CanUndo
        _btnRedo.Enabled = _rtb.CanRedo
        SetLinesInfo(If(_rtb.Lines.Any(), _rtb.Lines.Length, 0))

    End Sub

    Private Sub RichTextBox_SelectionChanged(sender As Object, e As EventArgs)

        If Suppressed = False Then
            SetIndexAndLineInfo()
        End If

    End Sub

    Private Sub SetIndexAndLineInfo()

        Dim index = _rtb.SelectionStart
        If CurrentLineOnly Then
            index -= _rtb.IndexOffset
        End If
        Dim lineNum = _rtb.GetLineFromCharIndexModified(_rtb.SelectionStart)
        If CurrentLineOnly = False AndAlso NewLineMode = NewLineMode.CrLf Then
            index += lineNum
        End If
        SetIndexAndLineInfo(index, lineNum + 1)

    End Sub

    Private Sub SetIndexAndLineInfo(index As Integer, line As Integer)

        _lblIndex.Text = My.Resources.IndexAbbr & ": " & index.ToString("n0", CultureInfo.CurrentCulture)
        _lblLine.Text = My.Resources.LineAbbr & ": " & line.ToString("n0", CultureInfo.CurrentCulture)

    End Sub

    Private Sub SetLinesInfo(count As Integer)

        _lblLines.Text = My.Resources.LinesAbbr & ": " & count.ToString("n0", CultureInfo.CurrentCulture)

    End Sub

    Private Sub SetCurrentLineText()

        _tbxCurrentLine.Text = If(CurrentLineOnly, _rtb.CurrentText, String.Empty)

    End Sub

    Private Sub RichTextBox_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.S Then
                Explorer.SaveInput()
            End If
        ElseIf e.Modifiers = (Keys.Control Or Keys.Shift) Then
            If e.KeyCode = Keys.N Then
                ToggleNewLine()
            End If
        End If

    End Sub

    Public Sub SetText(value As String)

        _rtb.BeginUpdate()
        _rtb.SetAllText(value)
        _rtb.EndUpdate()

    End Sub

    Private Property Suppressed As Boolean
        Get
            Return _suppressed
        End Get
        Set(value As Boolean)
            If _suppressed <> value Then
                _suppressed = value
                If _suppressed = False Then
                    SetCurrentLineText()
                    SetIndexAndLineInfo()
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentText As String
        Get
            Return _rtb.CurrentText
        End Get
    End Property

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

    Public Property HighlightEnabled As Boolean
        Get
            Return _highlightEnabled
        End Get
        Private Set(value As Boolean)
            If value <> _highlightEnabled Then
                _highlightEnabled = value
                _btnHighlight.Checked = value
            End If
        End Set
    End Property

    Public Property Text As String
        Get
            Return _rtb.Text
        End Get
        Set(value As String)
            _rtb.Text = value
        End Set
    End Property

    Public Property NewLineMode As NewLineMode
        Get
            Return _rtb.NewLineMode
        End Get
        Set(value As NewLineMode)
            _rtb.NewLineMode = value
        End Set
    End Property

    Public Property Options As InputOptions
        Get
            Dim value As InputOptions = InputOptions.None
            If CurrentLineOnly Then
                value = value Or InputOptions.CurrentLineOnly
                If _rtb.CurrentLineIncludesNewLine Then
                    value = value Or InputOptions.CurrentLineIncludesNewLine
                End If
            End If
            If HighlightEnabled Then
                value = value Or InputOptions.Highlight
            End If
            Return value
        End Get
        Set(value As InputOptions)
            CurrentLineOnly = ((value And InputOptions.CurrentLineOnly) = InputOptions.CurrentLineOnly)
            HighlightEnabled = ((value And InputOptions.Highlight) = InputOptions.Highlight)
            _rtb.CurrentLineIncludesNewLine = ((value And InputOptions.CurrentLineIncludesNewLine) = InputOptions.CurrentLineIncludesNewLine)
        End Set
    End Property

    Public Property Input As Input
        Get
            Return New Input() With {
                .Options = Options,
                .NewLine = NewLineMode,
                .CurrentLine = If(CurrentLineOnly, _rtb.CurrentLine, 0),
                .Text = _rtb.GetTextCrLf(),
                .Encoding = _encoding,
                .Attributes = _attributes,
                .Kind = _kind,
                .Name = Explorer.CurrentInputFullName}
        End Get
        Private Set(value As Input)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            Options = value.Options
            NewLineMode = value.NewLine
            Text = value.Text
            Encoding = value.Encoding
            Attributes = value.Attributes
            _kind = value.Kind
            App.MainForm._tslEncoding.Visible = (value.Kind = InputKind.File)
        End Set
    End Property

    Public Property Encoding As Encoding
        Get
            Return _encoding
        End Get
        Set(value As Encoding)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            _encoding = value
            App.MainForm._tslEncoding.Text = _encoding.EncodingName
        End Set
    End Property

    Public Property Attributes As ItemAttributes
        Get
            Return _attributes
        End Get
        Set(value As ItemAttributes)
            _attributes = value
        End Set
    End Property

    Public ReadOnly Property Kind As InputKind
        Get
            Return _kind
        End Get
    End Property

    Public Property Searcher As IndexSearcher
        Get
            Return _rtb.Searcher
        End Get
        Set(value As IndexSearcher)
            _rtb.Searcher = value
        End Set
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

    Friend _rtb As InputRichTextBox
    Friend _pnlBox As Panel
    Friend _tsp As AppToolStrip
    Friend _tbxCurrentLine As CurrentLineBox
    Friend _pnl As Panel
    Friend _tspFind As AppSearchToolStrip

    Friend _tssSave As ToolStripSplitButton
    Friend _btnUndo As ToolStripButton
    Friend _btnRedo As ToolStripButton
    Friend _tssUndo As ToolStripSeparator
    Friend _btnHighlight As ToolStripButton
    Friend _btnSelectionOnly As ToolStripButton
    Friend _btnCurrentLineOnly As ToolStripButton
    Friend _lblIndex As ToolStripLabel
    Friend _lblLine As ToolStripLabel
    Friend _lblLines As ToolStripLabel
    Friend _btnNewLine As ToolStripButton
    Friend _btnWordWrap As ToolStripButton

    Private _suppressed As Boolean
    Private _inCurrentTextChanged As Boolean
    Private _highlightEnabled As Boolean
    Private _encoding As Encoding
    Private _attributes As ItemAttributes
    Private _kind As InputKind

End Class
