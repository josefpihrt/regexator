Imports System.Diagnostics.CodeAnalysis
Imports Regexator
Imports Regexator.Text
Imports Regexator.Text.RegularExpressions
Imports Regexator.Windows.Forms

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class ReplacementPanel

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Public Sub New()

        _rtb = New ReplacementRichTextBox() With {.NewLineMode = My.Settings.DefaultReplacementNewLine}
        _tsp = New AppToolStrip()
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _tbxCurrentLine = New CurrentLineBox(_rtb)
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .Enabled = False, .BackColor = _rtb.BackColor}
        _tspFind = New AppSearchToolStrip(_rtb)

        CreateToolStripItems()

        AddHandler _rtb.TextChanged, AddressOf RichTextBox_TextChanged
        AddHandler _rtb.KeyDown, AddressOf RichTextBox_KeyDown
        AddHandler _btnUndo.Click, AddressOf Undo_Click
        AddHandler _btnRedo.Click, AddressOf Redo_Click
        AddHandler _ddbSnippets.DropDownOpening, Sub() _ddbSnippets.DropDownItems.LoadItems(_rtb.Sense.Factory.CreateItems(Data.EnumerateSubstitutionSnippets().OrderBy(Function(f) f, New SubstitutionSnippetSorter()), SnippetOptions.None))
        AddHandler _ddbSnippets.MouseDown, AddressOf FavoriteSnippets_MouseDown
        AddHandler _rtb.CurrentTextChanged, AddressOf RichTextBox_CurrentTextChanged
        AddHandler _rtb.CurrentLineOnlyChanged, AddressOf RichTextBox_CurrentLineOnlyChanged
        AddHandler _rtb.SelectionOnlyChanged, Sub() _btnSelectionOnly.Checked = _rtb.SelectionOnly
        AddHandler _rtb.NewLineModeChanged, Sub() _btnNewLine.Image = If(NewLineMode = NewLineMode.Lf, My.Resources.IcoLf.ToBitmap(), My.Resources.IcoCrLf.ToBitmap())
        AddHandler _btnCurrentLineOnly.CheckedChanged, Sub() _rtb.CurrentLineOnly = _btnCurrentLineOnly.Checked
        AddHandler _btnSelectionOnly.CheckedChanged, Sub() _rtb.SelectionOnly = _btnSelectionOnly.Checked
        AddHandler _rtb.Enter,
            Sub()
                If My.Settings.TrackActiveItemInSolutionExplorer Then
                    Explorer.SelectCurrentProjectNode()
                End If
            End Sub

        _tsp.Items.AddRange(EnumerateItems().ToArray())
        _pnlBox.Controls.Add(_rtb)

        _pnl.Controls.AddRange({_pnlBox, _tbxCurrentLine, _tspFind, _tsp})

        App.Formats.Text.Controls.Add(_rtb)
        App.Formats.Text.Controls.Add(_pnl)
        App.Formats.CurrentLineText.Controls.Add(_tbxCurrentLine)

    End Sub

    Private Sub CreateToolStripItems()

        _btnSave = New ToolStripButton(Nothing, My.Resources.IcoSave.ToBitmap(), Sub() Explorer.SaveReplacement()) With {.ToolTipText = My.Resources.SaveReplacement & " " & My.Resources.CtrlS.AddParentheses()}
        _btnUndo = New ToolStripButton(Nothing, My.Resources.IcoArrowLeft.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Undo & " " & My.Resources.CtrlZ.AddParentheses()}
        _btnRedo = New ToolStripButton(Nothing, My.Resources.IcoArrowRight.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Redo & " " & My.Resources.CtrlY.AddParentheses()}
        _btnSelectionOnly = New ToolStripButton(Nothing, My.Resources.IcoSelection.ToBitmap()) With {.ToolTipText = My.Resources.ToggleSelectionMode & " " & My.Resources.CtrlT.AddParentheses(), .CheckOnClick = True}
        _btnCurrentLineOnly = New ToolStripButton(Nothing, My.Resources.IcoCurrent.ToBitmap()) With {.ToolTipText = My.Resources.ToggleCurrentLineMode & " " & My.Resources.CtrlR.AddParentheses(), .CheckOnClick = True}
        _ddbSnippets = New ToolStripDropDownButton(Nothing, My.Resources.IcoSnippet.ToBitmap()) With {.ToolTipText = My.Resources.SubstitutionSnippets}
        _btnNewLine = New ToolStripButton(Nothing, My.Resources.IcoCrLf.ToBitmap(), Sub() ToggleNewLineMode()) With {.ToolTipText = My.Resources.ToggleNewLine & " " & My.Resources.CtrlShiftN.AddParentheses()}
        _btnNewLine.Image = If(My.Settings.DefaultReplacementNewLine = NewLineMode.CrLf, My.Resources.IcoCrLf.ToBitmap(), My.Resources.IcoLf.ToBitmap())
        _btnToUpper = New ToolStripButton(Nothing, My.Resources.IcoAUpper.ToBitmap(), Sub() ReplacementMode = If(ReplacementMode = ReplacementMode.ToUpper, ReplacementMode.None, ReplacementMode.ToUpper)) With {.ToolTipText = My.Resources.ToUppercase}
        _btnToLower = New ToolStripButton(Nothing, My.Resources.IcoALower.ToBitmap(), Sub() ReplacementMode = If(ReplacementMode = ReplacementMode.ToLower, ReplacementMode.None, ReplacementMode.ToLower)) With {.ToolTipText = My.Resources.ToLowercase}
#If DEBUG Then
        _btnChar = New ToolStripButton("ch", Nothing, Sub() ReplacementMode = If(ReplacementMode = ReplacementMode.Char, ReplacementMode.None, ReplacementMode.Char))
#End If
        _btnWordWrap = New ToolStripButton(Nothing, My.Resources.IcoWordWrap.ToBitmap(), Sub() ToggleWordWrap()) With {.ToolTipText = My.Resources.ToggleWordWrap, .Checked = _rtb.WordWrap}

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _btnSave
        Yield New ToolStripSeparator()
        Yield _btnUndo
        Yield _btnRedo
        Yield New ToolStripSeparator()
        Yield _ddbSnippets
        Yield _btnCurrentLineOnly
        Yield _btnSelectionOnly
        Yield _btnNewLine
        Yield New ToolStripSeparator()
        Yield _btnToUpper
        Yield _btnToLower
#If DEBUG Then
        Yield _btnChar
#End If
        Yield New ToolStripSeparator()
        Yield _btnWordWrap

    End Function

    Public Sub Load(container As ProjectContainer)

        _rtb.BeginUpdate()
        Replacement = If(container IsNot Nothing, container.Replacement, New Replacement())
        If container IsNot Nothing Then
            If container.Replacement.CurrentLine >= _rtb.Lines.Length Then
                container.Replacement.CurrentLine = 0
            End If
            If CurrentLineOnly Then
                If My.Settings.SelectEntireCurrentLineAfterLoad Then
                    _rtb.SelectLine(container.Replacement.CurrentLine, True)
                Else
                    _rtb.SelectLineStart(container.Replacement.CurrentLine)
                End If
            End If
        End If
        _pnl.Enabled = container IsNot Nothing
        _rtb.EndUpdate()

    End Sub

    Private Sub FavoriteSnippets_MouseDown(sender As Object, e As MouseEventArgs)

        If e.Button = MouseButtons.Right Then
            DirectCast(sender, ToolStripDropDownButton).ShowDropDown()
        End If

    End Sub

    Private Sub RichTextBox_CurrentTextChanged(sender As Object, e As EventArgs)

        SetCurrentLineText()
        If App.Mode = EvaluationMode.Replace Then
            Panels.Output.LoadData()
        End If

    End Sub

    Private Sub RichTextBox_CurrentLineOnlyChanged(sender As Object, e As EventArgs)

        _btnCurrentLineOnly.Checked = _rtb.CurrentLineOnly
        _btnSelectionOnly.Enabled = Not _rtb.CurrentLineOnly
        SetCurrentLineText()
        _tbxCurrentLine.Visible = _rtb.CurrentLineOnly

    End Sub

    Private Sub SetCurrentLineText()

        _tbxCurrentLine.Text = If(CurrentLineOnly, _rtb.CurrentText, String.Empty)

    End Sub

    Private Sub RichTextBox_TextChanged(sender As Object, e As EventArgs)

        _btnUndo.Enabled = _rtb.CanUndo
        _btnRedo.Enabled = _rtb.CanRedo

    End Sub

    Private Sub RichTextBox_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = (Keys.Control Or Keys.Shift) Then
            If e.KeyCode = Keys.N Then
                ToggleNewLineMode()
            End If
        End If

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

    Public Sub ToggleNewLineMode()

        _rtb.ToggleNewLineMode()

    End Sub

    Public Sub ToggleWordWrap()

        WordWrap = Not WordWrap

    End Sub

    Public Iterator Function EnumerateReplacements() As IEnumerable(Of String)

        For Each line As String In _rtb.EnumerateLines()
            Yield line
        Next

    End Function

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

    Public Property Options As ReplacementOptions
        Get
            Dim value = ReplacementOptions.None
            If CurrentLineOnly Then
                value = value Or ReplacementOptions.CurrentLineOnly
                If _rtb.CurrentLineIncludesNewLine Then
                    value = value Or ReplacementOptions.CurrentLineIncludesNewLine
                End If
            End If
            If ReplacementMode = ReplacementMode.ToUpper Then
                value = value Or ReplacementOptions.ToUpper
            ElseIf ReplacementMode = ReplacementMode.ToLower Then
                value = value Or ReplacementOptions.ToLower
            End If
            Return value
        End Get
        Set(value As ReplacementOptions)
            CurrentLineOnly = ((value And ReplacementOptions.CurrentLineOnly) = ReplacementOptions.CurrentLineOnly)
            _rtb.CurrentLineIncludesNewLine = ((value And ReplacementOptions.CurrentLineIncludesNewLine) = ReplacementOptions.CurrentLineIncludesNewLine)
            If ((value And ReplacementOptions.ToUpper) = ReplacementOptions.ToUpper) Then
                ReplacementMode = ReplacementMode.ToUpper
            ElseIf ((value And ReplacementOptions.ToLower) = ReplacementOptions.ToLower) Then
                ReplacementMode = ReplacementMode.ToLower
            Else
                ReplacementMode = ReplacementMode.None
            End If
        End Set
    End Property

    Public Property Replacement As Replacement
        Get
            Return New Replacement(Text, Options, NewLineMode, If(CurrentLineOnly, _rtb.CurrentLine, 0))
        End Get
        Private Set(value As Replacement)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            Text = value.Text
            Options = value.Options
            NewLineMode = value.NewLine
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

    Public ReadOnly Property CurrentText As String
        Get
            Return _rtb.CurrentText
        End Get
    End Property

    Public Property NewLineMode As NewLineMode
        Get
            Return _rtb.NewLineMode
        End Get
        Set(value As NewLineMode)
            _rtb.NewLineMode = value
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

    Public Property ReplacementMode As ReplacementMode
        Get
            Return _replacementMode
        End Get
        Private Set(value As ReplacementMode)
            If value <> _replacementMode Then
                _replacementMode = value
                _btnToUpper.Checked = (value = ReplacementMode.ToUpper)
                _btnToLower.Checked = (value = ReplacementMode.ToLower)
#If DEBUG Then
                _btnChar.Checked = (value = ReplacementMode.Char)
#End If
                If App.Mode = EvaluationMode.Replace Then
                    Panels.Output.LoadData()
                End If
            End If
        End Set
    End Property

    Friend _rtb As ReplacementRichTextBox
    Friend _tsp As AppToolStrip
    Friend _pnlBox As Panel
    Friend _tbxCurrentLine As CurrentLineBox
    Friend _pnl As Panel
    Friend _tspFind As AppSearchToolStrip

    Private _btnSave As ToolStripButton
    Private _btnUndo As ToolStripButton
    Private _btnRedo As ToolStripButton
    Private _btnSelectionOnly As ToolStripButton
    Private _btnCurrentLineOnly As ToolStripButton
    Private _ddbSnippets As ToolStripDropDownButton
    Private _btnNewLine As ToolStripButton
    Private _btnWordWrap As ToolStripButton
    Private _btnToUpper As ToolStripButton
    Private _btnToLower As ToolStripButton
#If DEBUG Then
    Private _btnChar As ToolStripButton
#End If

    Private _replacementMode As ReplacementMode

End Class