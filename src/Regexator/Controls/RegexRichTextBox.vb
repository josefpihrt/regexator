Imports Regexator.Text
Imports System.Globalization
Imports Regexator.Windows.Forms

Public Class RegexRichTextBox
    Inherits ExtendedRichTextBox
    Implements INewLineMode

    Public Sub New()

        MyBase.New()
        _prevText = String.Empty
        _prevCurrentText = String.Empty
        _exporter = New Exporter(Me)
        DetectUrls = False
        HideSelection = False
        Dock = DockStyle.Fill
        BorderStyle = BorderStyle.None

    End Sub

    Public Sub GoToCursor()

        Me.Select()
        EnsureCaretVisible()

    End Sub

    Public Function DoWithoutCurrentTextChanged(executor As Func(Of Boolean), enforced As Boolean) As Boolean

        If executor Is Nothing Then Throw New ArgumentNullException("executor")
        Dim value = SuppressCurrentTextChanged
        Dim fMode = enforced OrElse CurrentLineOnly OrElse SelectionOnly
        If fMode Then
            SuppressCurrentTextChanged = True
        End If
        BeginUpdate()
        Dim success = executor()
        EndUpdate()
        If fMode Then
            SuppressCurrentTextChanged = value
        End If
        If success Then
            CheckCurrentTextChanged()
        End If
        Return success

    End Function

    Public Function DoWithoutCurrentTextChanged(executor As Func(Of Boolean)) As Boolean

        Return DoWithoutCurrentTextChanged(executor, False)

    End Function

    Protected Sub CheckCurrentTextChanged()

        If SuppressCurrentTextChanged = False Then
            Dim text As String = CurrentText
            Dim line = GetCurrentLine()
            If (_prevCurrentText <> text) OrElse (CurrentLineOnly AndAlso _prevCurrentLine <> line) Then
                _prevCurrentText = text
                _prevCurrentLine = line
                OnCurrentTextChanged(EventArgs.Empty)
            End If
        End If

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Select Case e.Modifiers
            Case Keys.Control
                Select Case e.KeyCode
                    Case Keys.R
                        CurrentLineOnly = Not CurrentLineOnly
                        e.SuppressKeyPress = True
                    Case Keys.T
                        If CurrentLineOnly = False Then
                            SelectionOnly = Not SelectionOnly
                        End If
                        e.SuppressKeyPress = True
                    Case Keys.U
                        DoWithoutCurrentTextChanged(Function() TextBoxUtility.SelectionToLower(Me, CultureInfo.CurrentCulture))
                End Select
            Case Keys.Alt
                Select Case e.KeyCode
                    Case Keys.Up
                        DoWithoutCurrentTextChanged(AddressOf MoveSelectionUp)
                        e.SuppressKeyPress = True
                    Case Keys.Down
                        DoWithoutCurrentTextChanged(AddressOf MoveSelectionDown)
                        e.SuppressKeyPress = True
                End Select
            Case (Keys.Control Or Keys.Shift)
                Select Case e.KeyCode
                    Case Keys.C
                        Exporter.ExportToClipboard()
                        e.Handled = True
                    Case Keys.U
                        DoWithoutCurrentTextChanged(Function() TextBoxUtility.SelectionToUpper(Me, CultureInfo.CurrentCulture))
                    Case Keys.V
                        Exporter.ImportFromClipboard()
                        e.Handled = True
                End Select
        End Select
        MyBase.OnKeyDown(e)

    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)

        If SuppressTextChanged = False AndAlso _inTextChanged = False Then
            _inTextChanged = True
            If _prevText <> Text Then
                _prevText = Text
                CheckCurrentTextChanged()
                MyBase.OnTextChanged(e)
            End If
            _inTextChanged = False
        End If

    End Sub

    Protected Overrides Sub OnSelectionChanged(e As EventArgs)

        If CurrentLineOnly OrElse SelectionOnly Then
            CheckCurrentTextChanged()
        End If
        MyBase.OnSelectionChanged(e)

    End Sub

    Protected Overridable Sub OnCurrentTextChanged(e As EventArgs)

        If SuppressCurrentTextChanged = False Then
            RaiseEvent CurrentTextChanged(Me, e)
        End If

    End Sub

    Protected Overridable Sub OnCurrentLineOnlyChanged(e As EventArgs)

        RaiseEvent CurrentLineOnlyChanged(Me, e)
        CheckCurrentTextChanged()

    End Sub

    Protected Overridable Sub OnSelectionOnlyChanged(e As EventArgs)

        RaiseEvent SelectionOnlyChanged(Me, e)
        CheckCurrentTextChanged()

    End Sub

    Protected Overridable Sub OnNewLineModeChanged(e As EventArgs)

        RaiseEvent NewLineModeChanged(Me, e)
        CheckCurrentTextChanged()

    End Sub

    Protected Overridable Sub OnCurrentLineIncludesNewLineChanged(e As EventArgs)

        RaiseEvent CurrentLineIncludesNewLineChanged(Me, e)
        If CurrentLineOnly Then
            CheckCurrentTextChanged()
        End If

    End Sub

    Protected Overrides Sub ProcessCopyCurrentLine()

        If (SelectionOnly = False OrElse CurrentLineOnly) Then
            MyBase.ProcessCopyCurrentLine()
        End If

    End Sub

    Protected Overrides Sub ProcessCutCurrentLine()

        If (SelectionOnly = False OrElse CurrentLineOnly) Then
            MyBase.ProcessCutCurrentLine()
        End If

    End Sub

    Public Sub ToggleNewLineMode() Implements INewLineMode.ToggleNewLine

        NewLineMode = If(NewLineMode = Regexator.Text.NewLineMode.Lf, NewLineMode.CrLf, NewLineMode.Lf)

    End Sub

    Private Function GetCurrentText(mode As NewLineMode) As String

        If CurrentLineOnly Then
            Dim index = GetCurrentLine()
            Dim lines = Me.Lines
            If lines.Length > 0 Then
                Return lines(index) & If(CurrentLineIncludesNewLine AndAlso index < lines.Length - 1, mode.GetString(), String.Empty)
            Else
                Return String.Empty
            End If
        Else
            If SelectionOnly AndAlso SelectionLength > 0 Then
                Return GetSelectedText(mode)
            Else
                Return GetText(mode)
            End If
        End If

    End Function

    Public ReadOnly Property CurrentText As String
        Get
            Return GetCurrentText(NewLineMode)
        End Get
    End Property

    Public ReadOnly Property CurrentText(mode As NewLineMode) As String
        Get
            Return GetCurrentText(mode)
        End Get
    End Property

    Public ReadOnly Property IndexOffset As Integer
        Get
            If CurrentLineOnly Then
                Return GetFirstCharIndexOfCurrentLineModified()
            ElseIf SelectionOnly AndAlso SelectionLength > 0 Then
                Return SelectionStart
            Else
                Return 0
            End If
        End Get
    End Property

    Public Property CurrentLineOnly As Boolean
        Get
            Return _currentLineOnly
        End Get
        Set(value As Boolean)
            If value <> _currentLineOnly Then
                _currentLineOnly = value
                OnCurrentLineOnlyChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public Property SelectionOnly As Boolean
        Get
            Return _selectionOnly
        End Get
        Set(value As Boolean)
            If value <> _selectionOnly Then
                _selectionOnly = value
                OnSelectionOnlyChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public Property NewLineMode As NewLineMode Implements INewLineMode.NewLineMode
        Get
            Return _newLineMode
        End Get
        Set(value As NewLineMode)
            If _newLineMode <> value Then
                _newLineMode = value
                OnNewLineModeChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public Property CurrentLineIncludesNewLine As Boolean
        Get
            Return _currentLineIncludesNewLine
        End Get
        Set(value As Boolean)
            If _currentLineIncludesNewLine <> value Then
                _currentLineIncludesNewLine = value
                OnCurrentLineIncludesNewLineChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public ReadOnly Property Exporter As Exporter
        Get
            Return _exporter
        End Get
    End Property

    Protected Property SuppressTextChanged As Boolean
    Protected Property SuppressCurrentTextChanged As Boolean

    Private _prevText As String
    Private _prevCurrentText As String
    Private _prevCurrentLine As Integer
    Private _inTextChanged As Boolean
    Private _currentLineOnly As Boolean
    Private _selectionOnly As Boolean
    Private _currentLineIncludesNewLine As Boolean
    Private _newLineMode As NewLineMode = NewLineMode.Lf
    Private ReadOnly _exporter As Exporter

    Public Event CurrentTextChanged As EventHandler
    Public Event SelectionOnlyChanged As EventHandler
    Public Event CurrentLineOnlyChanged As EventHandler
    Public Event NewLineModeChanged As EventHandler
    Public Event CurrentLineIncludesNewLineChanged As EventHandler

End Class